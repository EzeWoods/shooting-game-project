using UnityEngine;

public class healingItems : MonoBehaviour
{
    [SerializeField] private int healAmount = 10;

    public void HealPlayer(playerController player)
    {
        if (player != null)
        {
            int currentHealth = player.GetHealth();
            int maxHealth = player.GetMaxHealth();
            int finalHealAmount = Mathf.Min(healAmount, maxHealth - currentHealth);

            if (finalHealAmount > 0)
            {
                player.HealAmount(finalHealAmount);
                Destroy(gameObject);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();
        if (player != null)
        {
            HealPlayer(player);
        }

    }

}
