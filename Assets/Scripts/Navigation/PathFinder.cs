using System.Collections;
using Algorithms;
using System.Collections.Generic;
using System;

public class PathFinder {
    Maze maze;
    List<Point2D> matrix = new List<Point2D>();
    Dictionary<int, Dictionary<int, PathElement>> pathDict = new Dictionary<int, Dictionary<int, PathElement>>();
    int[,] Wmatrix;

    private bool IsIntersection(int x, int z)
    {
        bool x0 = false,
            x1 = false,
            z0 = false,
            z1 = false;

        if (x + 1 < Map.XDemension)
            if (maze[x + 1, z] != MazeCell.Wall)
                x0 = true;

        if (x - 1 >= 0)
            if (maze[x, z] != MazeCell.Wall)
                x1 = true;

        if (z + 1 < Map.ZDemension)
            if (maze[x, z + 1] != MazeCell.Wall)
                z0 = true;

        if (z - 1 >= 0)
            if (maze[x, z - 1] != MazeCell.Wall)
                z1 = true;

        return ((x0 || x1) && (z0 || z1));        
    }

    private bool IsTwoPointOnLine(Point2D a, Point2D b)
    {
        if ((a.X != b.X) && (a.Y != b.Y))
            return false;

        Point2D c = a, d = a;

        for (int i = 0; i < Map.XDemension; i++)
        {
            if (a.X == b.X)
            {
                c = new Point2D(c.X - 1, c.Y);
                d = new Point2D(d.X + 1, d.Y);

                if (c.X < 0 || c.X >= Map.XDemension ||
                    d.X < 0 || d.X >= Map.XDemension)
                    return false;
            }
            if (a.Y == b.Y)
            {
                c = new Point2D(c.X, c.Y - 1);
                d = new Point2D(d.X, d.Y + 1);

                if (c.Y < 0 || c.Y >= Map.ZDemension ||
                    d.Y < 0 || d.Y >= Map.ZDemension)
                    return false;
            }

            if (c == b || d == b)
            {
                return true;
            }
        }

        return false;
    }

    private Point2D GetClosestPoint(Point2D point)
    {
        Point2D x0 = point, x1 = point, y0 = point, y1 = point;

        for (int i = 0; i < Map.XDemension; i++)
        {
            if (maze[point.X - 1, point.Y] == MazeCell.Void)
                x0 = new Point2D(x0.X - 1, x0.Y);
            if (maze[point.X + 1, point.Y] == MazeCell.Void)
                x1 = new Point2D(x1.X + 1, x1.Y);
            if (maze[point.X, point.Y - 1] == MazeCell.Void)
                y0 = new Point2D(y0.X, y0.Y - 1);
            if (maze[point.X, point.Y + 1] == MazeCell.Void)
                y1 = new Point2D(y1.X, y1.Y + 1);

            if (IsIntersection(x0.X, x0.Y))
            {
                return x0;
            }
            if (IsIntersection(x1.X, x1.Y))
            {
                return x1;
            }
            if (IsIntersection(y0.X, y0.Y))
            {
                return y0;
            }
            if (IsIntersection(y1.X, y1.Y))
            {
                return y1;
            }
        }

        return null;
    }

    public PathFinder(Maze in_maze)
    {
        maze = in_maze;

        List<Point2D> intersections = new List<Point2D>();

        for (int x = 1; x < Map.XDemension; x+=2)
            for (int z = 1; z < Map.ZDemension; z+=2)
            {
                if (IsIntersection(x, z))
                {
                    intersections.Add(new Point2D(x, z));
                }
            }

        Wmatrix = new int[intersections.Count, intersections.Count];

        for (int i = 0; i < Wmatrix.GetLength(0); i++)
        {
            pathDict.Add(i, new Dictionary<int, PathElement>());
            for (int j = 0; j < Wmatrix.GetLength(0); j++)
            {
                PathElement pathElement = new PathElement(int.MaxValue, intersections[i]);
                pathDict[i].Add(j, pathElement);
                Wmatrix[i, j] = int.MaxValue;
                if (i == j)
                {
                    Wmatrix[i, j] = 0;
                }
            }
        }

        for (int k = 0; k < intersections.Count; k++)
            for (int i = 0; i < intersections.Count; i++)
                for (int j = 0; j < intersections.Count; j++)
                {
                    if (IsTwoPointOnLine(intersections[i], intersections[j]))
                    {
                        if (Wmatrix[i, k] + Wmatrix[k, j] < Wmatrix[i, j])
                        {
                            Wmatrix[i, j] = Wmatrix[i, k] + Wmatrix[k, j];
                            pathDict[i][j] = new PathElement(Wmatrix[i, j], intersections[k]);
                        }
                        else
                        {
                            pathDict[i][j] = new PathElement(Wmatrix[i, j], intersections[j]);
                        }
                    }
                }
    }
}
