using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private int amountToSpawn;
    [SerializeField] private GameObject enemyPrefab;

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            newEnemy.GetComponent<Unit>().firstState = Unit.InitialState.ATTACK;
            newEnemy.GetComponent<Unit>().onlyAttack = true;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log("Finished spawning");
        yield return null;
    }
}
