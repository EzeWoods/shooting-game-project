using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medStation : MonoBehaviour
{
    [SerializeField] int healPrice;
    bool purchasePrompt;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (purchasePrompt)
            {
                if (gameManager.instance.getPlayerPoints() >= healPrice)
                {
                    gameManager.instance.playerScript.healDamage();
                    gameManager.instance.updatePlayerPoints(-healPrice);
                }

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.getHealth() < gameManager.instance.playerScript.getMaxHealth())
            {
                gameManager.instance.updateGunPrompt("Health", healPrice);
                gameManager.instance.showBuyGunPrompt();
                purchasePrompt = true;
            }


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.hideInteractPrompt();
            purchasePrompt = false;
        }
    }
}
