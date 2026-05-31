using System.Collections.Generic;
using UnityEngine;

public class BattleGridManager : MonoBehaviour
{
    public static BattleGridManager Instance;

    [Header("Grid")]
    public BattleTile tilePrefab;

    public int width = 10;
    public int height = 10;

    public float tileSize = 1f;

    // =========================
    // PLAYER
    // =========================

    [Header("Player")]
    public BattleUnit playerPrefab;

    // KHÔNG serialize runtime clone
    private List<BattleTile> playerSpawnTiles =
        new List<BattleTile>();

    public List<BattleTile> PlayerSpawnTiles
    {
        get { return playerSpawnTiles; }
    }

    // =========================
    // TILES
    // =========================

    private Dictionary<Vector2Int, BattleTile>
        tiles =
        new Dictionary<Vector2Int, BattleTile>();

    // =========================
    // UNITY
    // =========================

    private void Awake()
    {
        Instance = this;

        GenerateGrid();

        SetupPlayerSpawnTiles();
    }

    // =========================
    // GENERATE GRID
    // =========================

    private void GenerateGrid()
    {
        for (int x = 0;
            x < width;
            x++)
        {
            for (int y = 0;
                y < height;
                y++)
            {
                Vector3 pos =
                    new Vector3(
                        x * tileSize,
                        y * tileSize,
                        0);

                BattleTile tile =
                    Instantiate(
                        tilePrefab,
                        pos,
                        Quaternion.identity,
                        transform);

                tile.gridPos =
                    new Vector2Int(x, y);

                tiles.Add(
                    tile.gridPos,
                    tile);
            }
        }
    }

    // =========================
    // PLAYER SPAWN TILE
    // =========================

    private void SetupPlayerSpawnTiles()
    {
        playerSpawnTiles.Clear();

        for (int x = 0; x < 4; x++)
        {
            BattleTile tile =
                GetTile(
                    new Vector2Int(x, 0));

            if (tile != null)
            {
                playerSpawnTiles.Add(tile);
            }
        }
    }

    // =========================
    // GET TILE
    // =========================

    public BattleTile GetTile(
        Vector2Int pos)
    {
        if (tiles.ContainsKey(pos))
        {
            return tiles[pos];
        }

        return null;
    }

    // =========================
    // GET TILE AT POSITION
    // =========================

    public BattleTile GetTileAtPosition(
        Vector2Int pos)
    {
        return GetTile(pos);
    }

    public Vector2Int WorldToGridPosition(
        Vector3 worldPos)
    {
        int x =
            Mathf.RoundToInt(worldPos.x);

        int y =
            Mathf.RoundToInt(worldPos.y);

        return new Vector2Int(x, y);
    }
}