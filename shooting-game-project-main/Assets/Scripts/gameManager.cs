using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public Transform spawnPoint;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuWin;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] TMP_Text roundCountText;
    [SerializeField] TMP_Text pointsCountText;
    [SerializeField] public int playerPoints;
    [SerializeField] GameObject menuBuyGun;
    [SerializeField] GameObject menuBuyAmmo;
    [SerializeField] TMP_Text menuBuyAmmoPrice;
    [SerializeField] TMP_Text menuBuyGunPrice;
    [SerializeField] TMP_Text menuBuyGunName;
    //[SerializeField] TMP_Text menuAmmoCount;
    [SerializeField] public TMP_Text uiAmmoCount;
    [SerializeField] public TMP_Text uiAmmoStored;
    [SerializeField] public TMP_Text uiActiveWeapon;

    [SerializeField] public int enemyRemaining;

    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject player;
    public playerController playerScript;
    public bool isPaused;

    float timeScaleOrig;
    int goalCount;
    bool hadInteractPrompt;
    public bool inRound = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        updatePlayerPoints(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == menuBuyAmmo || menuActive == menuBuyGun)
            {
                hideInteractPrompt();
                hadInteractPrompt = true;
            }

            if (menuActive == null)
            {
                statePause();

            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }

        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoals(int amount)
    {
        enemyRemaining += amount;
        goalCountText.text = enemyRemaining.ToString("F0");

        if (enemyRemaining <= 0)
            inRound = false;
    }

    public void updatePlayerPoints(int amount)
    {
        playerPoints += amount;
        pointsCountText.text = playerPoints.ToString("F0");
    }

    public void youLose()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = menuLose;
        menuActive.SetActive(true);

    }

    public void youWin()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public int getPlayerPoints()
    {
        return playerPoints;
    }

    public void updateAmmoBuyPrice(int amount)
    {
        string decoration = "[" + amount.ToString("F0") + "]";

        if (amount <= 999)
            decoration = " " + decoration;

        menuBuyAmmoPrice.text = decoration;
    }

    public void updateGunPrompt(string gunName, int amount)
    {
        string decoration = "[" + amount.ToString("F0") + "]";

        if (amount <= 999)
            decoration = " " + decoration;

        if (gunName.Length < 7)
            menuBuyGunName.text = "  " + gunName;
        else
            menuBuyGunName.text = gunName;

        menuBuyGunPrice.text = decoration;
    }

    public void showBuyGunPrompt()
    {
        if (menuActive == null)
        {
            menuActive = menuBuyGun;
            menuActive.SetActive(true);
        }
    }

    public void showBuyAmmoPrompt()
    {
        if (menuActive == null)
        {
            menuActive = menuBuyAmmo;
            menuActive.SetActive(true);
        }
    }

    public void hideInteractPrompt()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void updateRoundCount(int round)
    {
        roundCountText.text = round.ToString("F0");
    }
}
