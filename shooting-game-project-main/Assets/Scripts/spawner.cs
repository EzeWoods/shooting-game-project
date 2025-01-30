using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{

    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int roundNumber;
    [SerializeField] int numToSpawn;        // enemy to spawn
    //[SerializeField] int numRemaining;    // enemy Remaining
    [SerializeField] int enemiesPerRound;   // enemy per round
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] float timeBetweenRounds;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int roundToWin;        // set to -1 if unlimited waves are wanted

    int spawnCount;
    bool roundStarted;
    bool startSpawning;
    bool isSpawning;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartNextRound());
    }

    // Update is called once per frame
    void Update()
    {
        if (roundNumber == roundToWin)
            gameManager.instance.youWin();

        if (gameManager.instance.enemyRemaining <= 0 && !gameManager.instance.inRound)
        {
                StartCoroutine(StartNextRound());
        }
    }

    public IEnumerator StartNextRound()
    {
        gameManager.instance.inRound = true;

        yield return new WaitForSeconds(timeBetweenRounds);

        roundNumber++;
        gameManager.instance.updateRoundCount(roundNumber);

        numToSpawn = roundNumber * enemiesPerRound;

        gameManager.instance.updateGameGoals(numToSpawn);

        StartCoroutine(spawn());
    }

    IEnumerator spawn()
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);

            int spawnInt = Random.Range(0, spawnPos.Length);

            Instantiate(objectToSpawn, spawnPos[spawnInt].position, spawnPos[spawnInt].rotation);
            //spawnCount++;

        }

    }
}
