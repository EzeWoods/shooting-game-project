using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] TMP_Text pointsCountText;

    [SerializeField] int playerPoints;

    public Image playerHPBar;
    public GameObject playerDamageScreen;

    public GameObject player;
    public playerController playerScript;
    public bool isPaused;

    float timeScaleOrig;
    int goalCount;

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

    public void updateGameGoal(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");
    }

    public void updatePlayerPoints(int amount)
    {
        playerPoints += amount;
        pointsCountText.text = playerPoints.ToString("F0");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);

    }

}
