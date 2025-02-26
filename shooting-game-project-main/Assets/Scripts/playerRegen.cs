using System.Collections;
using UnityEngine;

public class playerRegen : MonoBehaviour
{
    [Header("----- Regeneration Settings -----")]
    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private int regenAmount = 1;
    [SerializeField] private float regenRate = 1f;
    [SerializeField] private float criticalHealthThreshold = 0.3f;

    private playerController playerRef;
    private float lastDamageTime;
    private Coroutine regenCoroutine;

    private void Start()
    {
        playerRef = GetComponent<playerController>();

        lastDamageTime = -regenDelay;
    }

    private void Update()
    {

        if (playerRef == null) return;

        int currHealth = playerRef.GetHealth();
        int maxHealth = playerRef.GetMaxHealth();
        int regenCap = maxHealth / 2;


        if (currHealth < maxHealth && currHealth < maxHealth * criticalHealthThreshold && Time.time > lastDamageTime + regenDelay)
        {
            if (regenCoroutine == null)
            {
                regenCoroutine = StartCoroutine(RegenHealth(regenCap));
            }
        }

    }

    public void OnPlayerDamaged()
    {

        lastDamageTime = Time.time;

        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
    }

    private IEnumerator RegenHealth(int regenCap)
    {
        while (playerRef.GetHealth() < playerRef.GetMaxHealth())
        {

            int currentHealth = playerRef.GetHealth();
            int healAmount = regenAmount;

            int newHealth = Mathf.Min(currentHealth + healAmount, regenCap);

            HealPlayer(newHealth - currentHealth);

            yield return new WaitForSeconds(regenRate);
        }

        regenCoroutine = null;
    }

    private void HealPlayer(int amount)
    {

        int oldHealth = playerRef.GetHealth();

        playerRef.HealAmount(amount);

    }
}