using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] gunStats startingGun;

    [SerializeField] GameObject muzzleFlash; // Position where the muzzle flash should appear

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    int gunListPos;

    bool isShooting;
    bool isSprinting;
    bool isReloading;
    float shootTimer;

    void Start()
    {
        HPOrig = HP;
        onReset();
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        
        if(!gameManager.instance.isPaused)
        {
            movement();
            selectGun();
            shootTimer += Time.deltaTime;
        }
        
        sprint();
    }

    void movement()
    {
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

        if (Input.GetButton("Shoot") && gunList.Count > 0 && !isReloading && shootTimer >= shootRate)
        {
            if (gunList[gunListPos].ammoCurrent > 0)
                shoot();
            else
                StartCoroutine(reload());
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

    void shoot()
    {
        shootTimer = 0;
        gunList[gunListPos].ammoCurrent--;

        // Instantiate and play muzzle flash effect
        //if (muzzleFlashPrefab != null && muzzleFlashPosition != null)
        //{
        //    ParticleSystem muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPosition.position, muzzleFlashPosition.rotation);
        //    muzzleFlash.Play();
        //    Destroy(muzzleFlash.gameObject, muzzleFlash.main.duration);
        //}
        //else
        //{
        //    Debug.LogError("Muzzle flash prefab or position is not assigned.");
        //}
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

        gameManager.instance.uiAmmoCount.text = gunList[gunListPos].ammoCurrent.ToString("F0");
        gameManager.instance.uiAmmoStored.text = gunList[gunListPos].ammoStored.ToString("F0");

        isReloading = false;
    }

    public void onReset()
    {
        gunList.Clear();
        getGunStats(startingGun);
        startingGun.ammoCurrent = startingGun.ammoMax;
        startingGun.ammoStored = startingGun.ammoMaxStored;
    }

    public void refillAmmo()
    {
        //gunList[gunListPos].ammoMax = gunList[gunListPos].ammoMaxStored;
        gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;
        gunList[gunListPos].ammoStored = gunList[gunListPos].ammoMaxStored;
        updatePlayerUI();
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
    }

    public void healDamage()
    {
        HP = HPOrig;
        updatePlayerUI();
    }

    public int getHealth()
    {
        return HP;
    }

    public int getMaxHealth()
    {
        return HPOrig;
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
        
        if(gunList.Count > 0)
        {
            gameManager.instance.uiAmmoCount.text = gunList[gunListPos].ammoCurrent.ToString("F0");
            gameManager.instance.uiActiveWeapon.text = gunList[gunListPos].name;
            gameManager.instance.uiAmmoStored.text = gunList[gunListPos].ammoStored.ToString("F0");
        }
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

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
        shootDist = gunList[gunListPos].shootDistance;
        shootRate = gunList[gunListPos].shootRate;
        gunList[gunListPos].ammoStored = gunList[gunListPos].ammoMaxStored;
        gunList[gunListPos].ammoCurrent = gunList[gunListPos].ammoMax;

        updatePlayerUI();

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;

        //muzzleFlashPosition = gunModel.transform.Find("MuzzleFlashPosition"); // Update muzzle flash position

        //if (muzzleFlashPosition == null)
        //{
        //    Debug.LogError("MuzzleFlashPosition not found on the gun model.");
        //}
        //else
        //{
        //    Debug.Log("MuzzleFlashPosition updated successfully.");
        //}
    }

    public gunStats getCurrentGun()
    {
        if (gunList.Count > 0)
            return gunList[gunListPos];

        return null;
    }
}