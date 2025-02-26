using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()   
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.playerScript.OnReset();
        gameManager.instance.stateUnpause();
        
    }

    public void quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void returnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
