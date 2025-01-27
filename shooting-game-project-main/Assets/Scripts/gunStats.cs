using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public GameObject model;
    public int shootDamage;
    public int shootDistance;
    public float shootRate;
    public int ammoCurrent;
    public int ammoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootSoundVolume;
}