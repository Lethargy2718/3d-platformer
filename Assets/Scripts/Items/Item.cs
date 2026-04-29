using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public float cooldown;

    public GameObject viewPrefab;
    public GameObject worldPrefab;
    public GameObject pickupPrefab;

    public ItemBehavior itemBehavior;
}
