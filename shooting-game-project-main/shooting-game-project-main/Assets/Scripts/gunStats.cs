using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/GunStats")]
public class gunStats : MonoBehaviour
{
    [Header("Gun Model & Info")]
    public GameObject model;
    public string gunName;

    [Header("Gun Stats")]
    public int shootDamage;
    public int shootDist;
    public float shootRate;
    public float reloadTime;
    public int origShootDamage;

    [Header("Ammo Stats")]
    [SerializeField] public int ammoCurrent;
    [SerializeField] public int ammoStored;
    public int ammoMax;
    public int ammoMaxStored;

    [Header("Gun Effects & Sounds")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVolume;

    [Header("Fire Mode")]
    public FireMode fireMode;

    public enum FireMode { SemiAuto, FullAuto, Burst }

    public int GetAmmoCurrent() => ammoCurrent;
    public int GetAmmoStored() => ammoStored;

    public void UseAmmo(int amount)
    {
        ammoCurrent = Mathf.Max(0, ammoCurrent - amount);
    }

    public void Reload()
    {
        int ammoNeeded = ammoMax - ammoCurrent;
        int ammoToReload = Mathf.Min(ammoNeeded, ammoStored);

        ammoCurrent += ammoToReload;
        ammoStored -= ammoToReload;
    }

    public void AddAmmo(int amount)
    {
        ammoStored = Mathf.Min(ammoMaxStored, ammoStored + amount);
    }
}
