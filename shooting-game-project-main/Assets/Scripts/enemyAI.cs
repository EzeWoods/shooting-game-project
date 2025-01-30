using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int Points;
    [SerializeField] float detectionRange;

    [SerializeField] int damageAmount;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;

    [SerializeField] GameObject handDamagePrefab;

    bool playerInRange;

    Color colorOrig;
    float lastAttackTime;

    Vector3 playerDir;
    bool isInAttack;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        //gameManager.instance.updateGameGoal(1);
        lastAttackTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.isActiveAndEnabled)
        {
            MoveTowardsPlayer();
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
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        playerController player = FindObjectOfType<playerController>();
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.transform.position);
            }

            if(playerDir.magnitude <= attackRange)
            {
                if(!isInAttack)
                {
                    StartCoroutine(attack());
                }
            }
        }
    }
    
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    
    IEnumerator attack()
    {
        isInAttack = true;

        gameManager.instance.playerScript.takeDamage(damageAmount);

        yield return new WaitForSeconds(attackCooldown);

        isInAttack = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            // Dead
            Destroy(gameObject, 1f);
            Destroy(gameObject);
            gameManager.instance.updateGameGoals(-1);
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