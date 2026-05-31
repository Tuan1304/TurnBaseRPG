using UnityEngine;

public class GridCursor : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePos =
            Input.mousePosition;

        mousePos.z =
            -Camera.main.transform.position.z;

        Vector3 worldPos =
            Camera.main
            .ScreenToWorldPoint(mousePos);

        Vector2Int gridPos =
            BattleGridManager.Instance
            .WorldToGridPosition(worldPos);

        BattleTile tile =
            BattleGridManager.Instance
            .GetTile(gridPos);

        if (tile != null)
        {
            transform.position =
                tile.GetWorldPosition();
        }
    }
}