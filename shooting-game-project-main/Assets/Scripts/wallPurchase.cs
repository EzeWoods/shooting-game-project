using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallPurchase : MonoBehaviour
{
    [SerializeField] private gunStats gun;
    [SerializeField] private GameObject gunModel;
    [SerializeField] private int gunPrice;
    [SerializeField] private int ammoPrice;

    private bool ammoPrompt;
    private bool purchasePrompt;

    void Start()
    {
        if (gunModel != null && gun != null)
        {
            gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ammoPrompt && gameManager.instance.getPlayerPoints() >= ammoPrice)
            {
                gameManager.instance.playerScript.refillAmmo();
                gameManager.instance.updatePlayerPoints(-ammoPrice);
            }
            else if (purchasePrompt && gameManager.instance.getPlayerPoints() >= gunPrice)
            {
                gameManager.instance.playerScript.getGunStats(gun);
                gameManager.instance.updatePlayerPoints(-gunPrice);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerGun = gameManager.instance.playerScript.getCurrentGun();

            if (playerGun == null || playerGun.name != gun.name)
            {
                gameManager.instance.updateGunPrompt(gun.name, gunPrice);
                gameManager.instance.showBuyGunPrompt();
                purchasePrompt = true;
            }
            else
            {
                gameManager.instance.updateAmmoBuyPrice(ammoPrice);
                gameManager.instance.showBuyAmmoPrompt();
                ammoPrompt = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.hideInteractPrompt();
            purchasePrompt = false;
            ammoPrompt = false;
        }
    }
}
