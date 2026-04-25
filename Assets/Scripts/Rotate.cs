using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] private float rotationSpeed = 180f;

    private void Update()
    {
        transform.Rotate(new Vector3(0f, rotationSpeed * Time.deltaTime, 0f));
    }
}
