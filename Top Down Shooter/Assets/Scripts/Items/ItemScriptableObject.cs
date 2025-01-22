using UnityEngine;

[CreateAssetMenu(fileName = "newItemName", menuName = "Data/Items/Item")]
public class ItemScriptableObject : ScriptableObject
{
    /// <summary>
    /// This Scriptable Object manages the data of each base item in the game.
    /// It makes the process of creating item more easy.
    /// </summary>

    [Header("Item Information")]
    public string itemName;
    public Sprite itemSprite;
    public GameObject itemModel;
}
