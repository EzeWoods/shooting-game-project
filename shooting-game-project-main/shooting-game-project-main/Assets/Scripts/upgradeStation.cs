using UnityEngine;

public class upgradeStation : MonoBehaviour
{
    [SerializeField] int upgradePrice;
    [SerializeField] int upgradeLimit;
    bool purchasePrompt;
    int upgradeCount;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (purchasePrompt)
            {
                if (gameManager.instance.getPlayerPoints() >= upgradePrice && upgradeCount < upgradeLimit)
                {
                    upgradeCount++;
                    int newDamage = gameManager.instance.playerScript.getCurrentGun().shootDamage * 2;

                    gameManager.instance.playerScript.getCurrentGun().shootDamage = newDamage;

                    Debug.Log("Upgrade!");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (upgradeCount < upgradeLimit)
            {
                gameManager.instance.updateGunPrompt("Upgrade", upgradePrice);
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
