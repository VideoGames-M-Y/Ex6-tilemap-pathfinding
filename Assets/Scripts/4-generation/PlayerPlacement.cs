using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic; // Add this line for List<T>

public class PlayerPlacement : MonoBehaviour
{
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] TileBase wallTile = null;
    [SerializeField] TileBase floorTile = null;
    [SerializeField] GameObject player;
    private List<Vector3Int> floorTiles = new List<Vector3Int>();

    void Start()
    {
        FindAllFloorTiles();
        CheckTileTypeUnderPlayer();
    }

    void Update()
    {
        CheckTileTypeUnderPlayer();
    }

    void CheckTileTypeUnderPlayer()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3Int gridPosition = tilemap.WorldToCell(playerPosition);
        TileBase tileAtPosition = tilemap.GetTile(gridPosition);

        if (tileAtPosition == wallTile)
        {
            Debug.Log($"Player is standing on a mountain (wall tile) at {gridPosition}");
            MoveToRandomTile();
        }
        else if (tileAtPosition == floorTile)
        {
            Debug.Log($"Player is standing on a floor tile at {gridPosition}");
            CheckReachableTiles();
        }
        else
        {
            Debug.Log($"Player is standing on an unknown tile type at {gridPosition}");
        }
    }

    public void FindAllFloorTiles()
    {
        floorTiles.Clear();

        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                TileBase tileAtPosition = tilemap.GetTile(gridPosition);

                if (tileAtPosition == floorTile)
                {
                    floorTiles.Add(gridPosition);
                }
            }
        }

        Debug.Log($"Found {floorTiles.Count} floor tiles.");
    }

    public List<Vector3Int> GetFloorTiles()
    {
        return floorTiles;
    }

    public void MoveToRandomTile()
    {
        FindAllFloorTiles();

        if (floorTiles.Count > 0)
        {
            Vector3Int randomTile = floorTiles[Random.Range(0, floorTiles.Count)];

            while (tilemap.GetTile(randomTile) == wallTile)
            {
                randomTile = floorTiles[Random.Range(0, floorTiles.Count)];
            }

            if (tilemap.GetTile(randomTile) == floorTile)
            {
                Vector3 worldPosition = tilemap.CellToWorld(randomTile);
                player.transform.position = worldPosition;
                CheckTileTypeUnderPlayer();
            }
        }
        else
        {
            Debug.LogWarning("No valid floor tiles found to move the player to.");
        }
    }

    public bool Are100TilesReachable(Vector3Int startPosition)
    {
        if (!tilemap.HasTile(startPosition) || tilemap.GetTile(startPosition) != floorTile)
        {
            Debug.LogWarning("Starting position is not a floor tile.");
            return false;
        }

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        int reachableTileCount = 1;
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
        };

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (Vector3Int direction in directions)
            {
                Vector3Int neighbor = current + direction;

                if (!visited.Contains(neighbor) && tilemap.HasTile(neighbor) && tilemap.GetTile(neighbor) == floorTile)
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    reachableTileCount++;

                    if (reachableTileCount >= 100)
                    {
                        Debug.Log($"Found {reachableTileCount} reachable tiles.");
                        return true;
                    }
                }
            }
        }

        Debug.Log($"Only {reachableTileCount} reachable tiles found, which is fewer than 100.");
        return false;
    }

    public void CheckReachableTiles()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3Int gridPosition = tilemap.WorldToCell(playerPosition);

        bool result = Are100TilesReachable(gridPosition);
        if (result)
        {
            // Debug.Log("There are 100 reachable tiles.");
            return;
        }
        else
        {
            // Debug.Log("There are fewer than 100 reachable tiles.");
            MoveToRandomTile();
        }
    }
}
