using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This component allows the player to move by clicking the arrow keys,
 * but only if the new position is on an allowed tile.
 */
public class KeyboardMoverByTile : KeyboardMover {
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] AllowedTiles allowedTiles = null;

    [SerializeField] private TileBase[] waterTiles = null;    // Water tiles
    [SerializeField] private TileBase boatTile = null;        // The boat tile
    [SerializeField] private TileBase mountainTile = null;    // Single mountaine Tile
    [SerializeField] private TileBase horseTile = null;       // The horse tile
    [SerializeField] private TileBase pickaxeTile = null; 
    [SerializeField] private TileBase grassTile = null;

    [SerializeField] private TileBase replaceTile = null;     // Replacement tile after collecting items

    private bool hasBoat = false;
    private bool hasHorse = false;
    private bool hasPickaxe = false;

    private TileBase TileOnPosition(Vector3 worldPosition) {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }


    void Update() {
        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);

        // Check if the player collects the pickaxe
        if (tileOnNewPosition == pickaxeTile) {
            CollectPickaxe(newPosition);
        }

        // Check if the player can mine mountains with the pickaxe
        if (hasPickaxe && tileOnNewPosition == mountainTile) {
            MineMountain(newPosition);
        }

        // Check if the player collects the boat or the horse
        if (tileOnNewPosition == boatTile) {
            CollectBoat(newPosition);
        }
        if (tileOnNewPosition == horseTile) {
            CollectHorse(newPosition);
        }

        // Allow movement only if the tile is allowed
        if (allowedTiles.Contains(tileOnNewPosition)) {
            transform.position = newPosition;
        } else {
            Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
        }
    }

    private void CollectPickaxe(Vector3 pickaxePosition) {
        hasPickaxe = true;
        Debug.Log("Pickaxe collected! You can now mine mountains.");

        // Replace the pickaxe tile with the replacement tile if defined
        if (replaceTile != null) {
            SetTileAtPosition(pickaxePosition, replaceTile);
        } else {
            SetTileAtPosition(pickaxePosition, null);
        }
    }

    private void MineMountain(Vector3 mountainPosition) {
        // Replace mountain tile with grass if the player has the pickaxe
        if (mountainTile != null && grassTile != null) {
            // Replace the mountain tile with the grass tile
            SetTileAtPosition(mountainPosition, grassTile);
            Debug.Log("Mountain mined and turned into grass!");

            // Immediately update the player's position to the new tile
            Vector3 newPosition = NewPosition();  // Recalculate the position
            TileBase tileOnNewPosition = TileOnPosition(newPosition);

            // Allow movement only if the tile is allowed (including the new grass tile)
            if (allowedTiles.Contains(tileOnNewPosition)) {
                transform.position = newPosition;  // Move the player to the new tile
            } else {
                Debug.LogError("You cannot walk on " + tileOnNewPosition + "!");
            }
        } else {
            Debug.LogError("Mountain or grass tile is null!");
        }
    }

    private void SetTileAtPosition(Vector3 worldPosition, TileBase newTile) {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        tilemap.SetTile(cellPosition, newTile);
    }

    private void CollectBoat(Vector3 boatPosition) {
        hasBoat = true;
        Debug.Log("Boat collected! You can now move on water.");

        // Replace the boat tile with the replacement tile if defined
        if (replaceTile != null) {
            SetTileAtPosition(boatPosition, replaceTile);
        } else {
            SetTileAtPosition(boatPosition, null);
        }

        // Add water tiles to the allowed tiles list
        foreach (TileBase waterTile in waterTiles) {
            if (waterTile != null) {
                allowedTiles.AddTile(waterTile);  // Ensure the water tile is added to allowed tiles
                Debug.Log($"Added water tile: {waterTile.name}");
            } else {
                Debug.LogError("Water tile is null!");
            }
        }
    }

    private void CollectHorse(Vector3 horsePosition) {
        hasHorse = true;
        Debug.Log("Horse collected! You can now move on hills.");

        // Replace the horse tile with the replacement tile if defined
        if (replaceTile != null) {
            SetTileAtPosition(horsePosition, replaceTile);
        } else {
            SetTileAtPosition(horsePosition, null);
        }

        // Add the single mountaine tile to the allowed tiles list
        if (mountainTile != null) {
            allowedTiles.AddTile(mountainTile); // Ensure the mountaine tile is added to allowed tiles
            Debug.Log($"Added mountaine tile: {mountainTile.name}");
        } else {
            Debug.LogError("mountaine tile is null!");
        }
    }
}

