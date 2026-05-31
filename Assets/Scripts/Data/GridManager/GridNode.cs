using UnityEngine;

public class GridNode
{
    public Vector3Int cell;

    public bool walkable;

    public int gCost;
    public int hCost;

    public int fCost => gCost + hCost;

    public GridNode parent;

    public GridNode(Vector3Int c, bool walk)
    {
        cell = c;
        walkable = walk;
    }
}