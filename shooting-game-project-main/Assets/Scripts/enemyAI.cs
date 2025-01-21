using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] int Points;
    [SerializeField] int damageAmount;
    [SerializeField] float detectionRange;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

    [SerializeField] GameObject handDamagePrefab;

    bool playerInRange;
    Color colorOrig;
    float lastAttackTime;


    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
        lastAttackTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            MoveTowardsPlayer();
            if (Time.time > lastAttackTime + attackCooldown)
            {
                AttackPlayer();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.ResetPath();
        }
    }

    private void MoveTowardsPlayer()
    {
        playerController player = FindObjectOfType<playerController>();
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRange)
            {
            agent.SetDestination(player.transform.position);

            }
        }
    }

    private void AttackPlayer()
    {
        playerController player = FindObjectOfType<playerController>();
        if (player != null)
        {
            float distanceToplayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToplayer < attackRange)
            {
                GameObject handDamage = Instantiate(handDamagePrefab, transform.position, Quaternion.identity);
            handDamage.transform.position = transform.position + transform.forward;
            lastAttackTime = Time.time;
         }

      }
            
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            // Dead
            Destroy(gameObject);
            gameManager.instance.updateGameGoal(-1);
            gameManager.instance.updatePlayerPoints(Points);
        }

    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}