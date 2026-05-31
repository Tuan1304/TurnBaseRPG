using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Tilemaps")]
    public Tilemap groundTilemap;

    public Tilemap waterTilemap;

    public Tilemap obstacleTilemap;

    public LayerMask worldObstacleLayer;

    private void Awake()
    {
        Instance = this;
    }

    // kiểm tra ô có đi được không
    public bool IsWalkable(Vector3 worldPos)
    {
        Vector3Int cell =
            groundTilemap.WorldToCell(worldPos);

        // phải có ground
        bool hasGround =
            groundTilemap.HasTile(cell);

        if (!hasGround)
            return false;

        // nước
        if (waterTilemap != null &&
            waterTilemap.HasTile(cell))
        {
            return false;
        }

        // vật cản
        if (obstacleTilemap != null &&
            obstacleTilemap.HasTile(cell))
        {
            return false;
        }

        Collider2D hit =
            Physics2D.OverlapPoint(
                worldPos,
                worldObstacleLayer);

        if (hit != null)
        {
            return false;
        }

        return true;
    }

    // lấy vị trí center của tile
    public Vector3 GetCellCenter(Vector3 worldPos)
    {
        Vector3Int cell =
            groundTilemap.WorldToCell(worldPos);

        return groundTilemap.GetCellCenterWorld(cell);
    }
}