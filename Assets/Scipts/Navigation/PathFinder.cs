using UnityEngine;
using System.Collections;
using Algorithms;
using System.Collections.Generic;

public class PathFinder {
    Maze maze;
    List<Point2D> matrix = new List<Point2D>();
    Dictionary<int, Dictionary<int, PathElement>> pathDict = new Dictionary<int, Dictionary<int, PathElement>>();

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

        for (int i = 0; i < intersections.Count; i++)
        {
            Point2D closestPoint = GetClosestPoint(intersections[i]);
            
        }
    }
}
