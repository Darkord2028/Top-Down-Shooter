using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager instance = null;

    [Header("Default UI")]
    [SerializeField] Sprite defaultGunIcon;

    [Header("Equipped Weapon")]
    [SerializeField] Image rightHandWeaponIcon;
    [SerializeField] Image leftHandWeaponIcon;

    [Header("Pick Up Weapon")]
    [SerializeField] GameObject pickUpWeaponParent;
    [SerializeField] Image pickUpWeaponIcon;
    [SerializeField] TextMeshProUGUI pickUpWeaponName;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        pickUpWeaponParent.SetActive(false);
        
    }

    public void SetWeaponSprite(WeaponScriptableObject weapon)
    {
        if (weapon.isRightHandedWeapon)
        {
            rightHandWeaponIcon.sprite = weapon.itemSprite;
        }
        else
        {
            leftHandWeaponIcon.sprite = weapon.itemSprite;
        }
    }

    public void SetWeaponDefaultSprite(bool isRightHandWeapon)
    {
        if (isRightHandWeapon)
        {
            rightHandWeaponIcon.sprite = defaultGunIcon;
        }
        else
        {
            leftHandWeaponIcon.sprite = defaultGunIcon;
        }
    }

    public void SetPickUpWeaponUI(WeaponScriptableObject weapon)
    {
        if(weapon != null)
        {
            pickUpWeaponIcon.sprite = weapon.itemSprite;
            pickUpWeaponName.text = weapon.itemName;
            pickUpWeaponParent.gameObject.SetActive(true);
        }
        else
        {
            pickUpWeaponIcon.sprite = defaultGunIcon;
            pickUpWeaponName.text = "";
            pickUpWeaponParent.gameObject.SetActive(false);
        }
    }

}
