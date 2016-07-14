using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class Maze
    {
        private MazeCell[,] _Field;

        public int Width
        {
            get
            {
                return _Field.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return _Field.GetLength(1);
            }
        }

        public MazeCell this[int x, int y]
        {
            get
            {
                return _Field[x, y];
            }
        }

        public Maze(int width, int height)
        {
            _Field = new MazeCell[width * 2 + 1, height * 2 + 1];
            Generate();
        }

        public void Solve()
        {
            Random random = new Random();

            Point2D point;

            do
            {
                point = new Point2D(random.Next(0, Width), random.Next(0, Height));
            } while (_Field[point.X, point.Y] == MazeCell.Wall);

            Queue<Point2D> points = new Queue<Point2D>();
            points.Enqueue(point);
            _Field[point.X,point.Y] = MazeCell.Visited;

            do
            {
                Point2D newPoint;

                point = points.Dequeue();

                if (point.X > 0)
                {
                    newPoint = new Point2D(point.X - 1, point.Y);

                    if (_Field[newPoint.X,newPoint.Y] == MazeCell.Void)
                    {
                        points.Enqueue(newPoint);
                        _Field[newPoint.X, newPoint.Y] = MazeCell.Visited;
                    }
                }

                if (point.X < Width - 1)
                {
                    newPoint = new Point2D(point.X + 1, point.Y);

                    if (_Field[newPoint.X, newPoint.Y] == MazeCell.Void)
                    {
                        points.Enqueue(newPoint);
                        _Field[newPoint.X, newPoint.Y] = MazeCell.Visited;
                    }
                }

                if (point.Y > 0)
                {
                    newPoint = new Point2D(point.X, point.Y - 1);

                    if (_Field[newPoint.X, newPoint.Y] == MazeCell.Void)
                    {
                        points.Enqueue(newPoint);
                        _Field[newPoint.X, newPoint.Y] = MazeCell.Visited;
                    }
                }

                if (point.Y < Height - 1)
                {
                    newPoint = new Point2D(point.X, point.Y + 1);

                    if (_Field[newPoint.X, newPoint.Y] == MazeCell.Void)
                    {
                        points.Enqueue(newPoint);
                        _Field[newPoint.X, newPoint.Y] = MazeCell.Visited;
                    }
                }

            } while (points.Count > 0);
        }

        private void Generate()
        {
            Random random = new Random();

            IList<Point2D> emptyList = new List<Point2D>();
            IList<Point2D> walls = new List<Point2D>();

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        _Field[i, j] = MazeCell.Wall;

                        if (i > 0 && i < Width - 1 && j > 0 && j < Height - 1)
                        {
                            walls.Add(new Point2D(i, j));
                        }
                    }
                    else
                    {
                        _Field[i, j] = MazeCell.Void;
                        emptyList.Add(new Point2D(i, j));
                    }
                }
            }

            DisjointForest<Point2D> forest = new DisjointForest<Point2D>(emptyList, new Point2DEqualityComparer());

            for (int i = 0; i < walls.Count; i++)
            {
                int index = random.Next(i, walls.Count);

                Point2D temp = walls[i];
                walls[i] = walls[index];
                walls[index] = temp;

                Point2D wall = walls[i];

                if (wall.X % 2 == 0 && wall.Y % 2 == 1)
                {
                    Point2D pointA = new Point2D(wall.X - 1, wall.Y);
                    Point2D pointB = new Point2D(wall.X + 1, wall.Y);

                    if (!forest.IsInTheSameSet(pointA, pointB))
                    {
                        _Field[wall.X, wall.Y] = MazeCell.Void;
                        forest.Union(pointA, pointB);
                    }
                }
                else if (wall.X % 2 == 1 && wall.Y % 2 == 0)
                {
                    Point2D pointA = new Point2D(wall.X, wall.Y - 1);
                    Point2D pointB = new Point2D(wall.X, wall.Y + 1);

                    if (!forest.IsInTheSameSet(pointA, pointB))
                    {
                        _Field[wall.X, wall.Y] = MazeCell.Void;
                        forest.Union(pointA, pointB);
                    }
                }
            }
        }
    }
}
