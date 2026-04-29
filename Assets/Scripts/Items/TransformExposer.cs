using UnityEngine;

public class TransformExposer : MonoBehaviour
{
    [SerializeField] private Transform t;
    public Transform T => t;
}
