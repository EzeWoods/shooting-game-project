using UnityEngine;

[CreateAssetMenu(fileName = "NewGun", menuName = "Weapons/GunStats")]
public class gunStats : ScriptableObject
{
    [Header("Gun Model & Info")]
    public GameObject model;
    public string gunName;

    [Header("Gun Stats")]
    public int shootDamage;
    public int shootDist;
    public float shootRate;
    public float reloadTime;

    [Header("Ammo Stats")]
    [SerializeField] public int ammoCurrent;
    [SerializeField] public int ammoStored;
    public int ammoMax;
    public int ammoMaxStored;

    [Header("Gun Effects & Sounds")]
    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVolume;

    [Header("Fire Mode")] // <- Moved the header above the field instead of the enum
    public FireMode fireMode;

    public enum FireMode { SemiAuto, FullAuto, Burst }

    // Getters & Setters to manage ammo
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
