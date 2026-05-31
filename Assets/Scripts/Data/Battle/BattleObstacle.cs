using UnityEngine;

public class BattleObstacle : MonoBehaviour
{
    private void Start()
    {
        Vector3 pos =
            transform.position;

        Vector2Int gridPos =
            BattleGridManager.Instance
            .WorldToGridPosition(pos);

        BattleTile tile =
            BattleGridManager.Instance
            .GetTile(gridPos);

        if (tile != null)
        {
            tile.isBlocked = true;
        }
    }
}