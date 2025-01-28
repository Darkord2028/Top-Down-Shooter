using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Static class that manages UI in the world for player as well as enemies.
/// </summary>
public class WorldUIManager : MonoBehaviour
{
    public static WorldUIManager instance = null;

    [Header("Player Health")]
    [SerializeField] Slider healthSlider;

    [Header("Player Ability")]
    [SerializeField] Slider abilitySlider;
    [SerializeField] TextMeshProUGUI levelTxt;

    [Header("Player Upgrade Tree")]
    [SerializeField] GameObject upgradeTreeParent;
    [SerializeField] Image[] upgradeSprites;
    [SerializeField] TextMeshProUGUI[] upgradeTxts;

    [Header("Player Death UI")]
    [SerializeField] TextMeshProUGUI winLoseText;
    [SerializeField] GameObject deathUIParent;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI secondsText;
    [SerializeField] TextMeshProUGUI minutesText;

    [Header("Wave")]
    [SerializeField] TextMeshProUGUI currentWaveText;

    [Header("Default UI")]
    [SerializeField] Sprite defaultGunIcon;

    [Header("Equipped Weapon")]
    [SerializeField] Image rightHandWeaponIcon;
    [SerializeField] Image leftHandWeaponIcon;

    [Header("Pick Up Weapon")]
    [SerializeField] GameObject pickUpWeaponParent;
    [SerializeField] Image pickUpWeaponIcon;
    [SerializeField] TextMeshProUGUI pickUpWeaponName;

    [Header("Time")]
    [SerializeField] TextMeshProUGUI timeText;

    private float tickSeconds = 0;
    public int seconds { get; private set; }
    public int minutes { get; private set; }

    public int currentWave { get; private set; }

    #region Unity Callback Functions

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

    private void Update()
    {
        UpdateTime();
    }

    #endregion

    #region Weapon UI

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

    #endregion

    #region Player UI

    public void InitializePlayerHealth(int maxHealth, int currentHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void DamagePlayerHealth(int Damage)
    {
        healthSlider.value -= Damage;
    }

    public void IncreasePlayerHealth(int increaseAmount)
    {
        healthSlider.value += increaseAmount;
    }

    public void InitializePlayerAbility(int CurrentAbility, int MaxAbility, int Level)
    {
        abilitySlider.maxValue = MaxAbility;
        abilitySlider.value = CurrentAbility;
        levelTxt.text = Level.ToString();
    }

    public void UpdatePlayerAbility(int ability)
    {
        abilitySlider.value = ability;
    }

    public void UpdateUpgradeTree(int upgradeIndex, UpgradeItemScriptableObject[] upgradeItem)
    {
        upgradeTreeParent.SetActive(true);

        if (upgradeIndex == 0)
        {
            upgradeSprites[upgradeIndex].sprite = upgradeItem[upgradeIndex].upgradeSprite;
            upgradeTxts[upgradeIndex].text = upgradeItem[upgradeIndex].upgradeText;
        }
        else if (upgradeIndex == 1)
        {
            upgradeSprites[upgradeIndex].sprite = upgradeItem[upgradeIndex].upgradeSprite;
            upgradeTxts[upgradeIndex].text = upgradeItem[upgradeIndex].upgradeText;
        }
        else if (upgradeIndex == 2)
        {
            upgradeSprites[upgradeIndex].sprite = upgradeItem[upgradeIndex].upgradeSprite;
            upgradeTxts[upgradeIndex].text = upgradeItem[upgradeIndex].upgradeText;
        }
        else
        {
            return;
        }
    }

    public void DisableUpgradeTree(float timeScale)
    {
        Time.timeScale = timeScale;
        upgradeTreeParent.SetActive(false);
    }

    public void SetCurrentWave()
    {
        currentWaveText.text = currentWave.ToString();
    }

    public void SetDeathUI(bool isWon)
    {
        deathUIParent.SetActive(true);
        SaveManager saveManager = new SaveManager();
        int Highscore = saveManager.GetHighScore();
        int BestSeconds = saveManager.GetBestTimeInSeconds();
        int BestMinutes = saveManager.GetBestTimeInMinutes();

        highScoreText.text = Highscore.ToString();
        secondsText.text = BestSeconds.ToString();
        minutesText.text = BestMinutes.ToString();
        
        if(isWon)
        {
            winLoseText.text = "You Won!";
        }
        else
        {
            winLoseText.text = "You Lost!";
        }

    }

    public void ReloadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Private Functions

    private void UpdateTime()
    {
        tickSeconds += Time.deltaTime;
        seconds = Mathf.RoundToInt(tickSeconds);

        if(seconds >= 60)
        {
            tickSeconds = 0;
            minutes += 1;
            currentWave += 1;
            SetCurrentWave();
            WorldObjectPoolManager.instance.DeployEnemies(EnemyType.Boss, 2, WorldObjectPoolManager.instance.bossSpawnCount[currentWave]);
        }

        timeText.text = $"{minutes}:{seconds}";
    }

    #endregion

}
