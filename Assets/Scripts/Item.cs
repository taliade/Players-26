using UnityEngine;

// Dice QUÉ es cada objeto que aparece en los carriles.
// Va en cada prefab: Monstruo, Llave, Corazón y Regalo.
public enum ItemKind { Monster, Key, Heart, Gift }

public class Item : MonoBehaviour
{
    [SerializeField] ItemKind kind = ItemKind.Monster;

    public ItemKind Kind => kind;
}
