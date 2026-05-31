using System.Collections.Generic;
using UnityEngine;

public class BattlePathfinder : MonoBehaviour
{
    public static BattlePathfinder Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<BattleTile> FindPath(
        Vector2Int start,
        Vector2Int end)
    {
        List<PathNode> openList =
            new List<PathNode>();

        List<Vector2Int> closedList =
            new List<Vector2Int>();

        PathNode startNode =
            new PathNode(start);

        PathNode endNode =
            new PathNode(end);

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode current =
                openList[0];

            foreach (PathNode node in openList)
            {
                if (node.fCost < current.fCost)
                {
                    current = node;
                }
            }

            if (current.pos == end)
            {
                return BuildPath(current);
            }

            openList.Remove(current);

            closedList.Add(current.pos);

            foreach (Vector2Int dir in GetDirections())
            {
                Vector2Int nextPos =
                    current.pos + dir;

                if (closedList.Contains(nextPos))
                    continue;

                BattleTile tile =
                    BattleGridManager.Instance
                    .GetTile(nextPos);

                if (tile == null)
                    continue;

                if (!tile.walkable)
                    continue;

                if (tile.IsOccupied() &&
                    nextPos != end)
                    continue;

                int newCost =
                    current.gCost + 1;

                PathNode existing =
                    openList.Find(
                        n => n.pos == nextPos);

                if (existing == null)
                {
                    PathNode node =
                        new PathNode(nextPos);

                    node.gCost = newCost;

                    node.hCost =
                        Mathf.Abs(end.x - nextPos.x)
                        +
                        Mathf.Abs(end.y - nextPos.y);

                    node.parent = current;

                    openList.Add(node);
                }
                else if (newCost < existing.gCost)
                {
                    existing.gCost = newCost;

                    existing.parent = current;
                }
            }
        }

        return null;
    }

    private List<BattleTile> BuildPath(
        PathNode endNode)
    {
        List<BattleTile> path =
            new List<BattleTile>();

        PathNode current = endNode;

        while (current != null)
        {
            BattleTile tile =
                BattleGridManager.Instance
                .GetTile(current.pos);

            path.Add(tile);

            current = current.parent;
        }

        path.Reverse();

        return path;
    }

    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }
}