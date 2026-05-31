using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder : MonoBehaviour
{
    public static AStarPathfinder Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Vector3> FindPath(
        Vector3 startWorld,
        Vector3 targetWorld)
    {
        GridManager grid = GridManager.Instance;

        Vector3Int startCell =
            grid.groundTilemap.WorldToCell(startWorld);

        Vector3Int targetCell =
            grid.groundTilemap.WorldToCell(targetWorld);

        GridNode startNode =
            new GridNode(
                startCell,
                grid.IsWalkable(
                    grid.groundTilemap.GetCellCenterWorld(startCell)));

        GridNode targetNode =
            new GridNode(
                targetCell,
                true);

        List<GridNode> openList =
            new List<GridNode>();

        HashSet<Vector3Int> closedList =
            new HashSet<Vector3Int>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost <
                    currentNode.fCost)
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);

            closedList.Add(currentNode.cell);

            if (currentNode.cell == targetCell)
            {
                return RetracePath(
                    startNode,
                    currentNode);
            }

            foreach (Vector3Int neighborPos
                in GetNeighbors(currentNode.cell))
            {
                if (closedList.Contains(neighborPos))
                    continue;

                bool walkable =
                    grid.IsWalkable(
                        grid.groundTilemap
                        .GetCellCenterWorld(neighborPos));

                if (!walkable)
                    continue;

                int newCost =
                    currentNode.gCost +
                    GetDistance(
                        currentNode.cell,
                        neighborPos);

                GridNode neighborNode =
                    openList.Find(
                        n => n.cell == neighborPos);

                if (neighborNode == null)
                {
                    neighborNode =
                        new GridNode(
                            neighborPos,
                            true);

                    neighborNode.gCost =
                        newCost;

                    neighborNode.hCost =
                        GetDistance(
                            neighborPos,
                            targetCell);

                    neighborNode.parent =
                        currentNode;

                    openList.Add(neighborNode);
                }
                else if (newCost < neighborNode.gCost)
                {
                    neighborNode.gCost =
                        newCost;

                    neighborNode.parent =
                        currentNode;
                }
            }
        }

        return null;
    }

    List<Vector3> RetracePath(
        GridNode startNode,
        GridNode endNode)
    {
        List<Vector3> path =
            new List<Vector3>();

        GridNode currentNode =
            endNode;

        while (currentNode != startNode)
        {
            path.Add(
                GridManager.Instance
                .groundTilemap
                .GetCellCenterWorld(
                    currentNode.cell));

            currentNode =
                currentNode.parent;
        }

        path.Reverse();

        return path;
    }

    List<Vector3Int> GetNeighbors(
        Vector3Int cell)
    {
        return new List<Vector3Int>()
        {
            cell + Vector3Int.up,
            cell + Vector3Int.down,
            cell + Vector3Int.left,
            cell + Vector3Int.right
        };
    }

    int GetDistance(
        Vector3Int a,
        Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x)
            + Mathf.Abs(a.y - b.y);
    }
}