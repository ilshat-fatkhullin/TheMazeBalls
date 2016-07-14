using UnityEngine;
using System.Collections;
using Algorithms;

public class PathElement {
    public int Length;
    public Point2D Point;

    public PathElement(int length, Point2D point)
    {
        Length = length;
        Point = point;
    }
}
