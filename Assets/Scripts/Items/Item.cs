using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public float cooldown;
    public GameObject pickupPrefab;
    public GameObject heldPrefab;

    public abstract bool Use(PlayerController player);
}
