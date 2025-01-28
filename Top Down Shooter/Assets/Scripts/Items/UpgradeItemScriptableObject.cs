using UnityEngine;

/// <summary>
/// This scriptanle object stors the UI elements that need to be show during upgrades.
/// </summary>
[CreateAssetMenu(fileName = "newUpgradeItem", menuName = "Data/Items/Upgrade Item")]
public class UpgradeItemScriptableObject : ScriptableObject
{
    public UpgradeType upgradeType;
    public Sprite upgradeSprite;
    [TextArea]
    public string upgradeText;
}
