using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemy Manager")]
public class EnemyManager : ScriptableObject {
    //Enum for enemy types
    public enum EnemyType {
        Turret,
        Chaser,
        ChaserSpawner
    }

    //POCO class for configuring enemy mappings
    [System.Serializable]
    public class EnemyConfig {
        public EnemyType type;
        public Enemy prefab;
        public int poolSize;
    }

    //Workaround for Unity's lack of 2D array serialization
    [System.Serializable]
    public class EnemyWaveList {
        public EnemyWave[] waves;

        //Allows array syntax so we can just do [i] instead of .waves[i]
        public EnemyWave this[int i] => waves[i];
    }

    [SerializeField]
    private EnemyConfig[] enemyConfigs = null;
    [SerializeField]
    private EnemyWaveList[] enemyWaves = null;

	//Keep a count of active enemies
	public int ActiveEnemies { get; private set; }

	//Map each type to a pool of enemies
	private Dictionary<EnemyType, ObjectPool<Enemy>> pools;

    //Should be called in the GameController's Start() method to initialize the object pools
    public void Init() {
        pools = new Dictionary<EnemyType, ObjectPool<Enemy>>(enemyConfigs.Length);

        EnemyConfig pc;
        for (int i = 0; i < enemyConfigs.Length; i++) {
            pc = enemyConfigs[i];

            pools.Add(pc.type, new ObjectPool<Enemy>(pc.prefab, pc.poolSize, 5));
        }

        ActiveEnemies = 0;
    }

    //Called to begin an entire wave of enemies
    public void SpawnWave(int level, int wave) {
        SpawnWave(enemyWaves[level][wave]);
    }

    public void SpawnWave(EnemyWave wave) {
        SpawnPoint[] spawns = FindObjectsOfType<SpawnPoint>();
        SpawnPoint spawn;

		foreach (EnemyWave.EnemySpawnConfig config in wave) {
            spawn = spawns.FirstOrDefault(sp => sp.Index == config.spawnIndex);
            if (spawn == null)
                continue;

            SpawnEnemy(config.enemyType, spawn.transform.position, spawn.transform.rotation);
		}
    }

    //Called whenever an enemy is needed, to get one of the appropriate type from its object pool
    public Enemy SpawnEnemy(EnemyType type, Vector3 position, Quaternion rotation) {
        if (!pools.ContainsKey(type))
            return null;

        ActiveEnemies++;
        return pools[type].SpawnFromPool(position, rotation);
    }

    //Called by the OnDestroy of an enemy's Killable to signal that it has been defeated
    public void EnemyDestroyed(GameObject enemy) {
        enemy.SetActive(false);
        ActiveEnemies--;

		if (ActiveEnemies == 0) {
            //When all enemies have been defeated, we win!
            GameController.Win();
		}
	}

    //Called when the player restarts to ensure that no enemies are left hanging around
    public void ClearAllEnemies() {
		foreach (var p in pools.Values)
            p.DisableAllPooledObjects();

        ActiveEnemies = 0;
    }
}
