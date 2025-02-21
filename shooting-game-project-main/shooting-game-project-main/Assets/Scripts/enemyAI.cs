using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("AI Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] Animator animator;

    [Header("AI Stats")]
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int Points;
    [SerializeField] float detectionRange;

    [Header("Attack Stats")]
    [SerializeField] int damageAmount;
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;
    [SerializeField] GameObject handDamagePrefab;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] idleSounds;        // Ambient zombie growls
    [SerializeField] private AudioClip[] detectSounds;      // When it spots the player
    [SerializeField] private AudioClip attackSound;         // When attacking
    [SerializeField] private AudioClip[] hurtSounds;        // When taking damage
    [SerializeField] private AudioClip deathSound;          // When dying

    private bool playerInRange;
    private bool isInAttack;
    private bool hasPlayedDetectSound = false; // Ensures the detect sound only plays once
    private float lastAttackTime;
    private float idleSoundTimer = 0f;
    private float idleSoundInterval = 5f; // Adjust for how often the zombie makes idle sounds

    [Header ("Zombie Type")]
    [SerializeField] private ZombieType zombieType;

    private Color colorOrig;
    private Vector3 playerDir;


    public enum ZombieType
    {
        Regular,
        Tank,
        Sprinter
    }

    void Start()
    {
        colorOrig = model.material.color;
        lastAttackTime = 0f;
        audioSource = GetComponent<AudioSource>();

        switch (zombieType)
        {
            case ZombieType.Regular:
                agent.speed = 2f;
                break;
            case ZombieType.Tank:
                agent.speed = 1f;
                break;
            case ZombieType.Sprinter:
                agent.speed = 3f;
                break;
        }

        StartCoroutine(PlayIdleSounds()); // Start idle sounds loop
    }

  

    void Update()
    {
        if (agent.isActiveAndEnabled)
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
            hasPlayedDetectSound = false; // Allow detect sound to play again if the player is seen later
        }
    }

    private void MoveTowardsPlayer()
    {
        playerController player = FindObjectOfType<playerController>();
        if (player != null)
        {
            playerDir = player.transform.position - headPos.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer <= detectionRange)
            {
                agent.SetDestination(player.transform.position);

                // Play the detection sound once when spotting the player
                if (!hasPlayedDetectSound)
                {
                    PlaySound(detectSounds);
                    hasPlayedDetectSound = true;
                }
            }
            else
            {
                agent.ResetPath();
            }

            // Attack logic
            if (distanceToPlayer <= attackRange && !isInAttack)
            {
                StartCoroutine(Attack());
            }

            // Update animation speed based on actual movement speed
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed);

            // Debugging movement issues
            Debug.Log("Agent Speed: " + currentSpeed);
        }
    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Attack()
    {
        isInAttack = true;

        // Play attack sound
        audioSource.PlayOneShot(attackSound);

        gameManager.instance.playerScript.takeDamage(damageAmount);

        yield return new WaitForSeconds(attackCooldown);

        isInAttack = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        // Play pain sound
        PlaySound(hurtSounds);

        StartCoroutine(FlashRed());

        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Play death sound
        audioSource.PlayOneShot(deathSound);

        // Remove the zombie from the game
        Destroy(gameObject, 1f);
        gameManager.instance.updateGameGoals(-1);
        gameManager.instance.updatePlayerPoints(Points);
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    IEnumerator PlayIdleSounds()
    {
        while (true)
        {
            if (!playerInRange && idleSounds.Length > 0) // Only play if player isn't nearby
            {
                PlaySound(idleSounds);
            }
            yield return new WaitForSeconds(idleSoundInterval);
        }
    }

    void PlaySound(AudioClip[] soundArray)
    {
        if (soundArray.Length > 0)
        {
            audioSource.PlayOneShot(soundArray[Random.Range(0, soundArray.Length)]);
        }
    }
}
