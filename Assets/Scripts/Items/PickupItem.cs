using UnityEngine;

public class PickupItem : MonoBehaviour, IHittable
{
    public Item item;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GiveItemToPlayer(PlayerController player)
    {
        // If given successfully
        if (player.Inventory.AddItem(item, 1) == 0) 
        {
            Destroy(gameObject);
        } 
    }

    public void GetHit(float dmg, Vector3 hitDiretion)
    {
        rb.AddForce(hitDiretion * dmg, ForceMode.Impulse);
    }
}
