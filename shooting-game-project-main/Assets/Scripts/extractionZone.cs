using UnityEngine;

public class extractionZone : MonoBehaviour
{
    [SerializeField] int extractPrice; // Optional
    bool purchasePrompt;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (purchasePrompt)
            {
                gameManager.instance.youWin();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.canExtract)
            {
                gameManager.instance.updateGunPrompt("Extract", 0);
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
