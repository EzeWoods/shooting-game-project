using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [Header("----- Components -----")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private LayerMask ignoreMask;
    [SerializeField] private GameObject gunModel;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private AudioSource gunAudioSource;

    [Header("----- Player Stats -----")]
    [SerializeField] private int HP;
    [SerializeField] private int speed;
    [SerializeField] private int sprintMod;
    [SerializeField] private int jumpMax;
    [SerializeField] private int jumpSpeed;
    [SerializeField] private int gravity;

    [Header("----- Gun Stats -----")]
    [SerializeField] private List<gunStats> gunList = new List<gunStats>();
    [SerializeField] private int shootDist;
    [SerializeField] private int shootDamage;
    [SerializeField] private float shootRate;
    [SerializeField] private gunStats startingGun;

    [Header("----- Gun Audio -----")]
    [SerializeField] private AudioClip gunfireSound;
    [SerializeField] private float gunfireVolume = 1.0f;

    public int invCapacityLimit = 2;

    private Vector3 moveDir;
    private Vector3 playerVel;

    private int jumpCount;
    private int HPOrig;
    private int gunListPos;

    private bool isShooting;
    private bool isSprinting;
    private bool isReloading;
    private float shootTimer;

    void Start()
    {
        HPOrig = HP;
        ResetPlayer();
        updatePlayerUI();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!gameManager.instance.isPaused)
        {
            movement();
            selectGun();
            shootTimer += Time.deltaTime;
        }

        sprint();
    }

    void movement()
    {
        bool hasNoAmmo = false;
        bool needsReload = false;
        bool isLowAmmo = false;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }

        moveDir = (transform.right * Input.GetAxis("Horizontal")) +
                   (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(moveDir * speed * Time.deltaTime);
        jump();
        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;

        if ((controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            playerVel = Vector3.zero;
        }

        if(gunList.Count > 0)
        {
            hasNoAmmo = gunList[gunListPos].ammoStored == 0 && gunList[gunListPos].ammoCurrent == 0;
            needsReload = gunList[gunListPos].ammoCurrent == 0;
            isLowAmmo = gunList[gunListPos].ammoStored <= (gunList[gunListPos].ammoMax + 5);

            gameManager.instance.lowAmmoPrompt.SetActive(isLowAmmo && !hasNoAmmo);

            if (hasNoAmmo)
            {
                gameManager.instance.noAmmoPrompt.SetActive(true);
                //gameManager.instance.flashText(0.1f, gameManager.instance.noAmmoText);
            }
            else
            {
                gameManager.instance.noAmmoPrompt.SetActive(false);
            }

            gameManager.instance.reloadPrompt.SetActive(needsReload && !hasNoAmmo);
        }

        if (Input.GetButton("Shoot") && gunList.Count > 0 && !isReloading && shootTimer >= shootRate)
        {
            if (gunList[gunListPos].ammoCurrent > 0)
                Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && gunList[gunListPos].ammoCurrent < gunList[gunListPos].ammoMax)
        {
            StartCoroutine(reload());
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void Shoot()
    {
        if (gunAudioSource != null && gunfireSound != null)
        {
            gunAudioSource.PlayOneShot(gunfireSound, gunfireVolume);
        }

        shootTimer = 0;
        gunList[gunListPos].ammoCurrent--;

        StartCoroutine(flashMuzzleFire());

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreMask))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }

        updatePlayerUI();
    }

    IEnumerator flashMuzzleFire()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator reload()
    {
        if (gunList[gunListPos].ammoStored <= 0 || gunList[gunListPos].ammoCurrent == gunList[gunListPos].ammoMax)
            yield break;

        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(gunList[gunListPos].reloadTime);

        int ammoNeeded = gunList[gunListPos].ammoMax - gunList[gunListPos].ammoCurrent;
        int ammoToReload = Mathf.Min(ammoNeeded, gunList[gunListPos].ammoStored);

        gunList[gunListPos].ammoCurrent += ammoToReload;
        gunList[gunListPos].ammoStored -= ammoToReload;

        gameManager.instance.reloadPrompt.SetActive(false);
        updatePlayerUI();
        isReloading = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashScreenDamage());

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
        else if(HP <= 3)
        {
            gameManager.instance.lowHealthPrompt.SetActive(true);
        }
        else
        {
            gameManager.instance.lowHealthPrompt.SetActive(false);
        }
    }

    IEnumerator flashScreenDamage()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;

        if(gunList.Count == 0)
        {
            gameManager.instance.weapon1Text.text = "";
            gameManager.instance.weapon2Text.text = "";
        }

        if (gunList.Count > 0)
        {
            gameManager.instance.uiAmmoCount.text = gunList[gunListPos].ammoCurrent.ToString("F0");
            gameManager.instance.uiActiveWeapon.text = gunList[gunListPos].name;
            gameManager.instance.uiAmmoStored.text = gunList[gunListPos].ammoStored.ToString("F0");

            if (gunList[0])
            {
                gameManager.instance.weapon1Text.text = gunList[0].gunName;
            }
        }

        if (gunList.Count > 1)
        {
            if (gunList[1])
                gameManager.instance.weapon2Text.text = gunList[1].gunName;
        }
        else
        {
            gameManager.instance.weapon2Text.text = "";
        }
    }

    public void getGunStats(gunStats gun)
    {
        if(gunList.Count >= invCapacityLimit)
        {
            gunList[gunListPos] = gun;
        }
        else
        {
            gunList.Add(gun);
            gunListPos = gunList.Count - 1;
        }

        changeGun();
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    void changeGun()
    {
        shootTimer = gunList[gunListPos].shootRate;
        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        updatePlayerUI();

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void ResetPlayer()
    {
        HP = HPOrig;
        gunList.Clear();
        getGunStats(startingGun);
        startingGun.ammoCurrent = startingGun.ammoMax;
        startingGun.ammoStored = startingGun.ammoMaxStored;
        startingGun.origShootDamage = startingGun.shootDamage;
        updatePlayerUI();
    }

    public gunStats getCurrentGun()
    {
        if (gunList.Count > 0)
            return gunList[gunListPos];

        return null;
    }

    public void refillAmmo()
    {
        if (gunList.Count > 0)
        {
            gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;
            gunList[gunListPos].ammoStored = gunList[gunListPos].ammoMaxStored;
            updatePlayerUI();
        }
    }

    public void onReset()
    {
        // Reset player health
        HP = HPOrig;

        // Clear the gun list and re-add the starting gun
        gunList.Clear();
        getGunStats(startingGun);

        // Reset starting gun ammo
        startingGun.ammoCurrent = startingGun.ammoMax;
        startingGun.ammoStored = startingGun.ammoMaxStored;

        // Update UI to reflect changes
        updatePlayerUI();

        // Reset player position if needed (optional, adjust as necessary)
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPoint.position; // Ensure you have a spawnPoint set in GameManager
        controller.enabled = true;
    }

    // Returns current health
    public int getHealth()
    {
        return HP;
    }

    // Returns max health
    public int getMaxHealth()
    {
        return HPOrig;
    }

    // Heals the player to full health
    public void healDamage()
    {
        HP = HPOrig;
        updatePlayerUI();
    }
}
