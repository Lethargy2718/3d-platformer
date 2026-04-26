using UnityEngine;
using UnityEngine.Events;

public class PickupTrigger : MonoBehaviour
{
    public UnityEvent<PlayerController> PlayerEntered;
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            PlayerEntered?.Invoke(player);
        }    
    }
}
