using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class Maze
    {
        private MazeCell[,] _Field;
        private MatrixCell[,] _Matrix;
        private IDictionary<Point2D, int> _IntersectionIndexMap;
        private Point2D[] _IntersectionArray;
        private IEqualityComparer<Point2D> _Comparer = new Point2DEqualityComparer();

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
            set
            {
                _Field[x, y] = value;
            }
        }

        public Maze(int width, int height)
        {
            _Field = new MazeCell[width * 2 + 1, height * 2 + 1];
            Generate();
            FindPaths();
        }

        private IEnumerable<Point2D> GetNearestIntersections(Point2D point)
        {
            if (_Field[point.X, point.Y] != MazeCell.Void)
            {
                yield break;
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0 || i != 0 && j != 0)
                    {
                        continue;
                    }

                    Point2D newPoint = point;

                    do
                    {
                        newPoint = new Point2D(newPoint.X + i, newPoint.Y + j);
                    } while (_Field[newPoint.X, newPoint.Y] == MazeCell.Void && !_IntersectionIndexMap.ContainsKey(newPoint));

                    if (_Field[newPoint.X, newPoint.Y] != MazeCell.Wall)
                    {
                        yield return newPoint;
                    }
                }
            }
        }

        private void FindPaths()
        {
            _IntersectionIndexMap = new Dictionary<Point2D, int>(_Comparer);
            IList<Point2D> intersectionList = new List<Point2D>();

            for (int i = 1; i < Width - 1; i++)
            {
                for (int j = 1; j < Height - 1; j++)
                {
                    if (_Field[i, j] != MazeCell.Wall)
                    {
                        bool verticalWay = _Field[i, j - 1] != MazeCell.Wall || _Field[i, j + 1] != MazeCell.Wall;
                        bool horizontalWay = _Field[i - 1, j] != MazeCell.Wall || _Field[i + 1, j] != MazeCell.Wall;

                        bool crossroad = verticalWay && horizontalWay;

                        if (crossroad)
                        {
                            Point2D point = new Point2D(i, j);

                            _IntersectionIndexMap[point] = intersectionList.Count;
                            intersectionList.Add(point);
                        }
                    }
                }
            }

            _IntersectionArray = intersectionList.ToArray();

            _Matrix = new MatrixCell[_IntersectionIndexMap.Count, _IntersectionIndexMap.Count];

            for (int i = 0; i < _Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < _Matrix.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        _Matrix[i, j] = new MatrixCell { From = i, Length = 0 };
                    }
                    else
                    {
                        _Matrix[i, j] = new MatrixCell { From = -1, Length = -1 };
                    }
                }
            }

            foreach (KeyValuePair<Point2D, int> pair in _IntersectionIndexMap)
            {
                foreach (Point2D point in GetNearestIntersections(pair.Key))
                {
                    int length = Math.Abs(pair.Key.X - point.X) + Math.Abs(pair.Key.Y - point.Y);

                    int from = pair.Value;
                    int to = _IntersectionIndexMap[point];

                    MatrixCell cell = _Matrix[from, to];

                    cell.Length = length;
                    cell.From = from;
                }
            }

            for (int k = 0; k < _IntersectionIndexMap.Count; k++)
            {
                for (int i = 0; i < _IntersectionIndexMap.Count; i++)
                {
                    for (int j = 0; j < _IntersectionIndexMap.Count; j++)
                    {
                        MatrixCell matrixIk = _Matrix[i, k];
                        MatrixCell matrixKj = _Matrix[k, j];
                        MatrixCell matrixIj = _Matrix[i, j];

                        if (matrixIk.Length >= 0 && matrixKj.Length >= 0)
                        {
                            if (matrixIj.Length < 0 || matrixIj.Length > matrixIk.Length + matrixKj.Length)
                            {
                                matrixIj.Length = matrixIk.Length + matrixKj.Length;
                                matrixIj.From = matrixKj.From;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<Point2D> GetIntersections(Point2D point)
        {
            if (_IntersectionIndexMap.ContainsKey(point))
            {
                yield return point;
                yield break;
            }

            foreach (Point2D intersection in GetNearestIntersections(point))
            {
                yield return intersection;
            }
        }

        public int GetLength(Point2D point1, Point2D point2)
        {
            Point2D temp1;
            Point2D temp2;

            return GetLength(point1, point2, out temp1, out temp2);
        }

        private int GetLength(Point2D point1, Point2D point2, out Point2D minimumIntersection1, out Point2D minimumIntersection2)
        {
            int minimumLength = -1;
            minimumIntersection1 = null;
            minimumIntersection2 = null;

            if (_Field[point1.X, point1.Y] == MazeCell.Wall || _Field[point2.X, point2.Y] == MazeCell.Wall)
            {
                return minimumLength;
            }

            Point2D[] intersections1 = GetIntersections(point1).ToArray();
            Point2D[] intersections2 = GetIntersections(point2).ToArray();

            foreach (Point2D intersection1 in intersections1)
            {
                foreach (Point2D intersection2 in intersections2)
                {
                    int length1 = Math.Abs(intersection1.X - point1.X) + Math.Abs(intersection1.Y - point1.Y);
                    int length2 = Math.Abs(intersection2.X - point2.X) + Math.Abs(intersection2.Y - point2.Y);

                    int length = length1 + length2;

                    int from = _IntersectionIndexMap[intersection1];
                    int to = _IntersectionIndexMap[intersection2];

                    MatrixCell matrixCell = _Matrix[from, to];

                    if (matrixCell.Length < 0)
                    {
                        continue;
                    }

                    if (minimumLength < 0 || length + matrixCell.Length < minimumLength)
                    {
                        minimumLength = length + matrixCell.Length;
                        minimumIntersection1 = intersection1;
                        minimumIntersection2 = intersection2;
                    }
                }
            }

            if (minimumLength >= 0 && _Comparer.Equals(minimumIntersection1, minimumIntersection2))
            {
                if (point1.X == point2.X)
                {
                    if (minimumIntersection1.Y <= point1.Y && minimumIntersection1.Y <= point2.Y || minimumIntersection1.Y >= point1.Y && minimumIntersection1.Y >= point2.Y)
                    {
                        minimumIntersection1 = null;
                        minimumIntersection2 = null;
                        minimumLength = Math.Abs(point1.Y - point2.Y);
                    }
                }
                else if (point1.Y == point2.Y)
                {
                    if (minimumIntersection1.X <= point1.X && minimumIntersection1.X <= point2.X || minimumIntersection1.X >= point1.X && minimumIntersection1.X >= point2.X)
                    {
                        minimumIntersection1 = null;
                        minimumIntersection2 = null;
                        minimumLength = Math.Abs(point1.X - point2.X);
                    }
                }
            }

            return minimumLength;
        }

        public Point2D FindNext(Point2D point1, Point2D point2)
        {
            if (point1.X < 0 || point1.X >= Width || point1.Y < 0 || point1.Y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (point2.X < 0 || point2.X >= Width || point2.Y < 0 || point2.Y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            Point2D minimumIntersection1;
            Point2D minimumIntersection2;

            int minimumLength = GetLength(point1, point2, out minimumIntersection1, out minimumIntersection2);

            if (minimumLength < 0)
            {
                return null;
            }

            int max = 2;

            Point2D current = null;

            if (minimumIntersection1 == null || minimumIntersection2 == null)
            {
                int lengthX = Math.Abs(point2.X - point1.X);
                int signX = Math.Sign(point2.X - point1.X);
                int lengthY = Math.Abs(point2.Y - point1.Y);
                int signY = Math.Sign(point2.Y - point1.Y);

                for (int i = 0; i <= lengthX; i++)
                {
                    for (int j = 0; j <= lengthY; j++)
                    {
                        current = new Point2D(point1.X + signX * i, point1.Y + signY * j);
                        max--;

                        if (max <= 0)
                        {
                            return current;
                        }
                    }
                }
            }
            else
            {
                Point2D previous = null;

                foreach (Point2D pathPoint in PopulatePath(point1, minimumIntersection1))
                {
                    previous = current;
                    current = pathPoint;
                    max--;

                    if (max <= 0)
                    {
                        return current;
                    }
                }

                max++;
                current = previous;

                Point2D point = minimumIntersection1;

                int rowIndex = _IntersectionIndexMap[minimumIntersection2];
                int columnIndex;

                do
                {
                    columnIndex = _IntersectionIndexMap[point];

                    Point2D newPoint = _IntersectionArray[_Matrix[rowIndex, columnIndex].From];

                    foreach (Point2D pathPoint in PopulatePath(point, newPoint))
                    {
                        previous = current;
                        current = pathPoint;
                        max--;

                        if (max <= 0)
                        {
                            return current;
                        }
                    }

                    max++;
                    current = previous;

                    point = newPoint;
                } while (_Matrix[rowIndex, columnIndex].From != columnIndex);

                foreach (Point2D pathPoint in PopulatePath(minimumIntersection2, point2))
                {
                    previous = current;
                    current = pathPoint;
                    max--;

                    if (max <= 0)
                    {
                        return current;
                    }
                }
            }

            return current;
        }

        public Point2D[] FindPath(Point2D point1, Point2D point2)
        {
            if (point1.X < 0 || point1.X >= Width || point1.Y < 0 || point1.Y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (point2.X < 0 || point2.X >= Width || point2.Y < 0 || point2.Y >= Height)
            {
                throw new ArgumentOutOfRangeException();
            }

            Point2D minimumIntersection1;
            Point2D minimumIntersection2;

            int minimumLength = GetLength(point1, point2, out minimumIntersection1, out minimumIntersection2);

            if (minimumLength < 0)
            {
                return null;
            }

            List<Point2D> path = new List<Point2D>();

            if (minimumIntersection1 == null || minimumIntersection2 == null)
            {
                int lengthX = Math.Abs(point2.X - point1.X);
                int signX = Math.Sign(point2.X - point1.X);
                int lengthY = Math.Abs(point2.Y - point1.Y);
                int signY = Math.Sign(point2.Y - point1.Y);

                for (int i = 0; i <= lengthX; i++)
                {
                    for (int j = 0; j <= lengthY; j++)
                    {
                        path.Add(new Point2D(point1.X + signX * i, point1.Y + signY * j));
                    }
                }
            }
            else
            {
                path.AddRange(PopulatePath(point1, minimumIntersection1));

                path.RemoveAt(path.Count - 1);

                Point2D point = minimumIntersection1;

                int rowIndex = _IntersectionIndexMap[minimumIntersection2];
                int columnIndex;

                do
                {
                    columnIndex = _IntersectionIndexMap[point];

                    Point2D newPoint = _IntersectionArray[_Matrix[rowIndex, columnIndex].From];

                    path.AddRange(PopulatePath(point, newPoint));
                    path.RemoveAt(path.Count - 1);

                    point = newPoint;
                } while (_Matrix[rowIndex, columnIndex].From != columnIndex);

                path.AddRange(PopulatePath(minimumIntersection2, point2));
            }

            return path.ToArray();
        }

        private static IEnumerable<Point2D> PopulatePath(Point2D point1, Point2D point2)
        {
            int lengthX = Math.Abs(point2.X - point1.X);
            int signX = Math.Sign(point2.X - point1.X);
            int lengthY = Math.Abs(point2.Y - point1.Y);
            int signY = Math.Sign(point2.Y - point1.Y);

            for (int i = 0; i <= lengthX; i++)
            {
                for (int j = 0; j <= lengthY; j++)
                {
                    yield return new Point2D(point1.X + signX * i, point1.Y + signY * j);
                }
            }
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
            IList<Point2D> points = new List<Point2D>();

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
