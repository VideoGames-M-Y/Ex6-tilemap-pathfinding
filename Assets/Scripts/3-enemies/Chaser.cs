using UnityEngine;

/**
 * This component chases a given target object.
 */
public class Chaser : TargetMover
{
    [Tooltip("The object that we try to chase")]
    [SerializeField] Transform targetObject = null;

    // Returns the position of the target object
    public Vector3 TargetObjectPosition()
    {
        return targetObject.position;
    }

    private void Update()
    {
        // Set the target to the position of the target object
        SetTarget(targetObject.position);
    }
}
