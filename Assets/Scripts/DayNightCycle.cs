using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Day/Night cycle length in seconds")]
    private float cycleLength = 20.0f;

    private float RotationPerSecond => 360f / cycleLength;

    public void Update()
    {
        transform.Rotate(Vector3.right, RotationPerSecond * Time.deltaTime);
    }
}
