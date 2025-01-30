using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallPurchase : MonoBehaviour
{
    [SerializeField] gunStats gun;
    [SerializeField] GameObject gunModel;
    [SerializeField] int gunPrice;
    [SerializeField] int ammoPrice;

    bool ammoPrompt;
    bool purchasePrompt;

    // Start is called before the first frame update
    void Start()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ammoPrompt)
            {
                if (gameManager.instance.getPlayerPoints() >= ammoPrice)
                {
                    // fill players ammo for respective weapon
                    gameManager.instance.playerScript.refillAmmo();
                    gameManager.instance.updatePlayerPoints(-ammoPrice);
                }
            }
            else if (purchasePrompt)
            {
                if (gameManager.instance.getPlayerPoints() >= gunPrice)
                {
                    gameManager.instance.playerScript.getGunStats(gun);
                    gameManager.instance.updatePlayerPoints(-gunPrice);
                }

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.getCurrentGun().name != gun.name)
            {
                gameManager.instance.updateGunPrompt(gun.name, gunPrice);
                gameManager.instance.showBuyGunPrompt();
                purchasePrompt = true;
            }
            else if (gameManager.instance.playerScript.getCurrentGun().name == gun.name)
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
