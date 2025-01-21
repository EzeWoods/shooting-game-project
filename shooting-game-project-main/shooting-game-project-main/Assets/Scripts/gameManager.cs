using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;

    public GameObject player;
    public playerController playerScript;
    public bool isPaused;

    float timeScaleOrig;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
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

}
