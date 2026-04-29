using UnityEngine;

[CreateAssetMenu(menuName = "Items/Throwable")]
public class ThrowableItem : Item, IStackable
{
    public GameObject throwablePrefab;
}
