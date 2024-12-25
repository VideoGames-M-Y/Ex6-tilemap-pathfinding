using UnityEngine;
using UnityEngine.UIElements;

/**
 * This component just rotates its object between angular bounds.
 */
public class Rotator : MonoBehaviour
{
    [SerializeField] float minAngle = -90;      // Minimum rotation angle
    [SerializeField] float maxAngle = 90;       // Maximum rotation angle
    [SerializeField] Vector3 angularVelocity = new Vector3(30, 0, 0); // Rotation speed (degrees per second)

    private int direction = 1;   // Rotation direction (1 for clockwise, -1 for counterclockwise)
    private float angle = 0;     // Current angle of rotation

    void Update()
    {
        transform.Rotate(direction * angularVelocity * Time.deltaTime);  // Rotate the object

        angle += direction * Time.deltaTime;  // Update the current angle

        if (angle > 180)
            angle -= 360;  // Normalize the angle

        if (angle <= minAngle)
            direction = 1;  // Reverse direction when reaching min angle

        if (angle >= maxAngle)
            direction = -1; // Reverse direction when reaching max angle
    }
}
