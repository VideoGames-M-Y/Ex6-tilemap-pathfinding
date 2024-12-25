using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component allows the player to move by clicking the arrow keys,
 * but only if the new position is on an allowed tile.
 */
public class KeyboardMoverByTile : KeyboardMover
{
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] AllowedTiles allowedTiles = null;

    [SerializeField] private TileBase[] waterTiles = null;   // Water tiles
    [SerializeField] private TileBase boatTile = null;       // The boat tile
    [SerializeField] private TileBase mountainTile = null;   // Single mountain Tile
    [SerializeField] private TileBase horseTile = null;      // The horse tile
    [SerializeField] private TileBase pickaxeTile = null;    // The pickaxe tile
    [SerializeField] private TileBase grassTile = null;      // The grass tile

    [SerializeField] private TileBase replaceTile = null;    // Replacement tile after collecting items

    private bool hasBoat = false;
    private bool hasHorse = false;
    private bool hasPickaxe = false;

    // Returns the tile at the given world position
    private TileBase TileOnPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }

    void Update()
    {
        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);

        // Handle item collection and movement
        if (tileOnNewPosition == pickaxeTile)
        {
            CollectPickaxe(newPosition);
        }
        else if (tileOnNewPosition == boatTile)
        {
            CollectBoat(newPosition);
        }
        else if (tileOnNewPosition == horseTile)
        {
            CollectHorse(newPosition);
        }

        // Handle movement if allowed
        if (allowedTiles.Contains(tileOnNewPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
        }
    }

    // Handles collecting the pickaxe and replacing the tile with a new one
    private void CollectPickaxe(Vector3 pickaxePosition)
    {
        hasPickaxe = true;
        Debug.Log("Pickaxe collected! You can now mine mountains.");

        // Replace the pickaxe tile with the replacement tile if defined
        if (replaceTile != null)
        {
            SetTileAtPosition(pickaxePosition, replaceTile);
        }
        else
        {
            SetTileAtPosition(pickaxePosition, null);
        }
    }

    // Handles collecting the boat and adding water tiles to allowed tiles
    private void CollectBoat(Vector3 boatPosition)
    {
        hasBoat = true;
        Debug.Log("Boat collected! You can now move on water.");

        // Replace the boat tile with the replacement tile if defined
        if (replaceTile != null)
        {
            SetTileAtPosition(boatPosition, replaceTile);
        }
        else
        {
            SetTileAtPosition(boatPosition, null);
        }

        // Add water tiles to the allowed tiles list
        foreach (TileBase waterTile in waterTiles)
        {
            if (waterTile != null)
            {
                allowedTiles.AddTile(waterTile);  // Ensure the water tile is added to allowed tiles
                Debug.Log($"Added water tile: {waterTile.name}");
            }
            else
            {
                Debug.LogError("Water tile is null!");
            }
        }
    }

    // Handles collecting the horse and adding mountain tiles to allowed tiles
    private void CollectHorse(Vector3 horsePosition)
    {
        hasHorse = true;
        Debug.Log("Horse collected! You can now move on hills.");

        // Replace the horse tile with the replacement tile if defined
        if (replaceTile != null)
        {
            SetTileAtPosition(horsePosition, replaceTile);
        }
        else
        {
            SetTileAtPosition(horsePosition, null);
        }

        // Add the single mountain tile to the allowed tiles list
        if (mountainTile != null)
        {
            allowedTiles.AddTile(mountainTile); // Ensure the mountain tile is added to allowed tiles
            Debug.Log($"Added mountain tile: {mountainTile.name}");
        }
        else
        {
            Debug.LogError("Mountain tile is null!");
        }
    }

    // Mines the mountain tile and turns it into grass
    private void MineMountain(Vector3 mountainPosition)
    {
        // Replace mountain tile with grass if the player has the pickaxe
        if (hasPickaxe && mountainTile != null && grassTile != null)
        {
            SetTileAtPosition(mountainPosition, grassTile);
            Debug.Log("Mountain mined and turned into grass!");

            // Immediately update the player's position to the new tile
            Vector3 newPosition = NewPosition();
            TileBase tileOnNewPosition = TileOnPosition(newPosition);

            // Allow movement only if the tile is allowed (including the new grass tile)
            if (allowedTiles.Contains(tileOnNewPosition))
            {
                transform.position = newPosition;
            }
            else
            {
                Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
            }
        }
        else
        {
            Debug.LogError("Mountain or grass tile is null!");
        }
    }

    // Sets the tile at a specific world position
    private void SetTileAtPosition(Vector3 worldPosition, TileBase newTile)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        tilemap.SetTile(cellPosition, newTile);
    }
}
