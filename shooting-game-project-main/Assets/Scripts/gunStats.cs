using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    public string name;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public float reloadTime;
    public int ammoCurrent;
    public int ammoStored;
    public int ammoMax;
    public int ammoMaxStored;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVolume;
}