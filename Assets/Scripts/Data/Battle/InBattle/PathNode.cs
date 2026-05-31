using UnityEngine;

public class PathNode
{
    public Vector2Int pos;

    public int gCost;
    public int hCost;

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public PathNode parent;

    public PathNode(Vector2Int pos)
    {
        this.pos = pos;
    }
}