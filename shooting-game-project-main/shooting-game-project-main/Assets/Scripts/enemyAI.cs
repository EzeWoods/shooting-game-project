using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] int damageAmount;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

    bool playerInRange;
    Color colorOrig;
    float lastAttackTime;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        lastAttackTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void AttackPlayer()
    {
        playerController player = FindObjectOfType<playerController>();
        if (player != null)
        {
            player.takeDamage(damageAmount);
            lastAttackTime = Time.time;
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            //Enemy dies
            Debug.Log("Enemy is dead");
            Destroy(gameObject);
        }

    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}