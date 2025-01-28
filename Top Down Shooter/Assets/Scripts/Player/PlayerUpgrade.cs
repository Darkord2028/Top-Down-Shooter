using UnityEngine;

/// <summary>
/// This class stores different upgrade functions that player gets in the game
/// </summary>
public class PlayerUpgrade : MonoBehaviour
{
    Player player;
    Health health;

    [Tooltip("Values needs to percentages, which will be added to the base ugrade")]
    [Header("Player Upgrade")]
    [SerializeField] int speedUpgrade;
    [SerializeField] int healthUpgrade;
    [SerializeField] int healAmount;

    [Header("Weapon Upgrade")]
    [SerializeField] WeaponScriptableObject[] leftHandWeapons;
    [SerializeField] WeaponScriptableObject[] rightHandWeapons;

    [Header("Special Weapon Upgrade")]
    [SerializeField] SpecialWeaponScriptableObject[] specialWeapons;

    [SerializeField] UpgradeItemScriptableObject[] upgrades;

    private void Start()
    {
        player = GetComponent<Player>();
        health = GetComponent<Health>();
    }

    #region Set Upgrade Fuctions

    private void PlayerSpeedUpgrade()
    {
        float baseSpeed = player.playerData.moveSpeed;
        player.LocomotionState.speedUpgrade = baseSpeed * speedUpgrade / 100f;
    }

    private void PlayerHealthUpgrade()
    {
        int baseHealth = health._MaxHealth;
        int upgradeAmount = baseHealth * healthUpgrade / 100;

        health.HealthUpgrade(upgradeAmount);
    }

    private void PlayerHeal()
    {
        int baseHealth = health._MaxHealth;
        int healAmount = baseHealth * this.healAmount / 100;

        health.Heal(healAmount);
    }

    private void PlayerLeftWeaponUpgrade()
    {
        player.EquipmentManager.leftHandWeapon.UpgradeWeapon();
    }

    private void PlayerRightWeaponUpgrade()
    {
        player.EquipmentManager.rightHandWeapon.UpgradeWeapon();
    }

    private void PlayerSpecialWeaponUpgrade()
    {
        Shuffle(specialWeapons);
        for (int i = 0; i < specialWeapons.Length; i++)
        {
            if (specialWeapons[i].currentLevel != 0)
            {
                specialWeapons[i].UpdateWeapon();
                break;
            }
        }
    }

    private void PlayerGetNewRightWeapon()
    {
        Shuffle(rightHandWeapons);

        Shuffle(rightHandWeapons);
        while (player.EquipmentManager.rightHandWeapon == rightHandWeapons[0])
        {
            Shuffle(rightHandWeapons);
        }

        player.EquipmentManager.LoadWeapon(rightHandWeapons[0]);
    }

    private void PlayerGetNewLeftWeapon()
    {
        Shuffle(leftHandWeapons);

        Shuffle(leftHandWeapons);
        while (player.EquipmentManager.leftHandWeapon == leftHandWeapons[0])
        {
            Shuffle(leftHandWeapons);
        }

        player.EquipmentManager.LoadWeapon(leftHandWeapons[0]);
    }

    private void PlayerGetNewSpecialWeapon()
    {
        for (int i = 0; i < specialWeapons.Length; i++)
        {
            if (specialWeapons[i].currentLevel == 0)
            {
                specialWeapons[i].UpdateWeapon();
                break;
            }
        }
    }

    #endregion

    #region Shuffle Functions

    /// <summary>
    /// This function shuffles the contents of the list.
    /// </summary>
    /// <param name="array"></param>
    private void Shuffle(UpgradeItemScriptableObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // Get a random index in the range [0, i]

            // Swap elements at i and randomIndex
            UpgradeItemScriptableObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    /// <summary>
    /// This function shuffles the contents of the list.
    /// </summary>
    /// <param name="array"></param>
    private void Shuffle(WeaponScriptableObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // Get a random index in the range [0, i]

            // Swap elements at i and randomIndex
            WeaponScriptableObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    /// <summary>
    /// This function shuffles the contents of the list.
    /// </summary>
    /// <param name="array"></param>
    private void Shuffle(SpecialWeaponScriptableObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // Get a random index in the range [0, i]

            // Swap elements at i and randomIndex
            SpecialWeaponScriptableObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    #endregion

    #region Other Functions

    public void EnableUpgradeTree()
    {
        if (upgrades == null) return;

        Shuffle(upgrades);
        for (int i = 0; i < 3; i++)
        {
            WorldUIManager.instance.UpdateUpgradeTree(i, upgrades);
        }

    }

    public void OnSelectButtonClick(int buttonIndex)
    {
        UpgradeBaseOnUpgradeType(upgrades[buttonIndex]);
    }

    private void UpgradeBaseOnUpgradeType(UpgradeItemScriptableObject upgradeItem)
    {
        switch(upgradeItem.upgradeType)
        {
            case UpgradeType.None:
                WorldUIManager.instance.DisableUpgradeTree(1f);
                return;

            case UpgradeType.Heal:
                PlayerHeal();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.HealthIncrese:
                PlayerHealthUpgrade();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.SpeedIncrese:
                PlayerSpeedUpgrade();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.LeftWeaponUpgrade:
                PlayerLeftWeaponUpgrade();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.RightWeaponUpgrade:
                PlayerRightWeaponUpgrade();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.SpecialWeaponUpgrade:
                PlayerSpecialWeaponUpgrade();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.GetNewRightWeapon:
                PlayerGetNewRightWeapon();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.GetNewLeftWeapon:
                PlayerGetNewLeftWeapon();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

            case UpgradeType.GetNewSpecialWeapon:
                PlayerGetNewSpecialWeapon();
                WorldUIManager.instance.DisableUpgradeTree(1f);
                break;

        }

    }

    #endregion

}

/// <summary>
/// Different upgrade types in the game.
/// </summary>
public enum UpgradeType
{
    None,
    Heal,
    HealthIncrese,
    SpeedIncrese,
    LeftWeaponUpgrade,
    RightWeaponUpgrade,
    SpecialWeaponUpgrade,
    GetNewRightWeapon,
    GetNewLeftWeapon,
    GetNewSpecialWeapon
}
