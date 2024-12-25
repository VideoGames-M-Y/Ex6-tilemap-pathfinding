using UnityEngine;

public class Sleeper : MonoBehaviour
{
    [SerializeField] private float minSleepTime = 2f; // Minimum sleep duration
    [SerializeField] private float maxSleepTime = 4f; // Maximum sleep duration

    private float sleepTimer;
    private Chaser chaser;

    private void Awake()
    {
        chaser = GetComponent<Chaser>();
    }

    private void OnEnable()
    {
        sleepTimer = Random.Range(minSleepTime, maxSleepTime);

        if (chaser != null)
        {
            chaser.enabled = false;
        }
    }

    private void Update()
    {
        sleepTimer -= Time.deltaTime;

        if (sleepTimer <= 0f)
        {
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (chaser != null)
        {
            chaser.enabled = true;
        }
    }
}
