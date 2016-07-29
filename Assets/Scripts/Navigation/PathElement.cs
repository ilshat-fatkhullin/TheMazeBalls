using System.Collections;
using Algorithms;

public class PathElement {
    public int Length;
    public Point2D From;

    public PathElement(int length, Point2D from)
    {
        Length = length;
        From = from;
    }
}
