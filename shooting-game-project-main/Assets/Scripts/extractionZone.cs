using TMPro;
using UnityEngine;

public class extractionZone : MonoBehaviour
{
    [SerializeField] int extractPrice; // Optional
    [SerializeField] GameObject extractText;
    bool purchasePrompt;

    // Update is called once per frame
    void Update()
    {
        if (gameManager.instance.canExtract)
        {
            extractText.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.canExtract)
            {
                gameManager.instance.youWin();
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
