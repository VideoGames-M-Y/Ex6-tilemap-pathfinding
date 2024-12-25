using UnityEngine;

/**
 * This component represents a cycle of points in space.
 * It can represent, for example, a patrol path for an AI.
 */
public class Cycle : MonoBehaviour
{
    [SerializeField] float gizmoRadius = 0.2f;  // Radius for gizmos of patrol points

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < transform.childCount; ++i)
        {
            // Draw the sphere at each patrol point
            Gizmos.DrawSphere(transform.GetChild(i).position, gizmoRadius);

            // Connect each point to the next one in the cycle
            Gizmos.DrawLine(
                transform.GetChild(i).position,
                transform.GetChild((i + 1) % transform.childCount).position
            );
        }
    }
}
