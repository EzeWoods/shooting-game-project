using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("AI Components")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform headPos;

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

    private Color colorOrig;
    private Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        lastAttackTime = 0f;
        audioSource = GetComponent<AudioSource>();

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
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        playerController player = FindObjectOfType<playerController>();

        if (player != null)
        {
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

            if (distanceToPlayer <= attackRange)
            {
                if (!isInAttack)
                {
                    StartCoroutine(Attack());
                }
            }
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
