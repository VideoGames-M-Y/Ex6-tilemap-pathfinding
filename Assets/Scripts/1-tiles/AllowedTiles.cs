using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AllowedTiles : MonoBehaviour {
    [SerializeField] private List<TileBase> allowedTiles = new List<TileBase>(); // Use List instead of array

    public bool Contains(TileBase tile) {
        return allowedTiles.Contains(tile); // Works with List
    }

    public void AddTile(TileBase tile) {
        if (!allowedTiles.Contains(tile)) {
            allowedTiles.Add(tile); // Add the tile to the list
        }
    }

    public TileBase[] Get() {
        return allowedTiles.ToArray();  // Convert List to an array
    }
}