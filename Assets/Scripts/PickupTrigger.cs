using UnityEngine;
using UnityEngine.Events;

public class PickupTrigger : MonoBehaviour
{
    public UnityEvent<PlayerController> PlayerEntered;
    
    private bool isOnCooldown = false;

    private void OnTriggerStay(Collider other)
    {
        if (isOnCooldown) return;
        
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            PlayerEntered?.Invoke(player);
        }
    }

    public void StartCooldown(float collectionCooldown)
    {
        isOnCooldown = true;
        Invoke(nameof(EndCooldown), collectionCooldown);
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
    }

    public void ResetCooldown() => isOnCooldown = false;
}