using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBehavior : MonoBehaviour
{
    [SerializeField]
    private float spawnRate = 1.5f;
    [SerializeField]
    //private Killable spawnedPrefab = null;
    private EnemyManager.EnemyType enemyType = EnemyManager.EnemyType.Chaser;

    private float spawnTimer = 0.0f;

	private void Start() {
        //We'll always give the player a head-start of 1 full spawn
        spawnTimer = spawnRate;
	}

	private void Update() {
        //Update the spawn timer if necessary
        if (spawnTimer > 0.0f)
            spawnTimer = Mathf.Max(0.0f, spawnTimer - Time.deltaTime);

        //Spawn the assigned prefab when the timer reaches 0
        if (spawnTimer == 0.0f) {
            //Instantiate(spawnedPrefab, transform.position, transform.rotation);
            GameController.EnemyManager.SpawnEnemy(enemyType, transform.position, transform.rotation);

            //Reset the spawn timer
            spawnTimer = spawnRate;
        }
    }
}
