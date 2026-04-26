using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public float cooldown;
    public GameObject prefab;

    public abstract bool Use(PlayerStateDriver3D player);
}
