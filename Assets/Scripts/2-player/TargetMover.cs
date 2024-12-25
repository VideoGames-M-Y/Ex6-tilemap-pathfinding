using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TargetMover : MonoBehaviour
{
    [SerializeField] protected Tilemap tilemap = null;
    [SerializeField] AllowedTiles allowedTiles = null;

    [Tooltip("The speed by which the object moves towards the target, in meters (=grid units) per second")]
    [SerializeField] float speed = 2f;

    [Tooltip("Maximum number of iterations before BFS algorithm gives up on finding a path")]
    [SerializeField] int maxIterations = 1000;

    [Tooltip("The target position in world coordinates")]
    [SerializeField] Vector3 targetInWorld;

    [Tooltip("The target position in grid coordinates")]
    [SerializeField] Vector3Int targetInGrid;

    protected bool atTarget; // This property is set to "true" whenever the object has already found the target.

    protected TilemapGraph tilemapGraph = null; // Changed from private to protected
    private float timeBetweenSteps;

    // Sets the target position and updates the grid position
    public void SetTarget(Vector3 newTarget)
    {
        if (targetInWorld != newTarget)
        {
            targetInWorld = newTarget;
            targetInGrid = tilemap.WorldToCell(targetInWorld);
            atTarget = false;
        }
    }

    // Returns the current target position in world coordinates
    public Vector3 GetTarget()
    {
        return targetInWorld;
    }

    // Initializes the tilemap graph and starts the movement coroutine
    protected virtual void Start()
    {
        tilemapGraph = new TilemapGraph(tilemap, allowedTiles.Get());
        timeBetweenSteps = 1 / speed;
        StartCoroutine(MoveTowardsTheTarget());
    }

    // Coroutine that continuously moves the object towards the target
    IEnumerator MoveTowardsTheTarget()
    {
        for (;;)
        {
            yield return new WaitForSeconds(timeBetweenSteps);

            if (enabled && !atTarget)
                MakeOneStepTowardsTheTarget();
        }
    }

    // Makes one step towards the target using BFS to find the shortest path
    private void MakeOneStepTowardsTheTarget()
    {
        Vector3Int startNode = tilemap.WorldToCell(transform.position);
        Vector3Int endNode = targetInGrid;

        // Get the shortest path using BFS
        List<Vector3Int> shortestPath = BFS.GetPath(tilemapGraph, startNode, endNode, maxIterations);
        Debug.Log("shortestPath = " + string.Join(" , ", shortestPath));

        if (shortestPath.Count >= 2) // shortestPath contains both source and target
        {
            Vector3Int nextNode = shortestPath[1];
            transform.position = tilemap.GetCellCenterWorld(nextNode);
        }
        else
        {
            if (shortestPath.Count == 0)
            {
                Debug.LogError($"No path found between {startNode} and {endNode}");
            }

            atTarget = true;
        }
    }
}
