using UnityEngine;

/// <summary>
/// Base class for item in the game.
/// It stores 3D model of the item, its sprite as well as its name
/// </summary>
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
