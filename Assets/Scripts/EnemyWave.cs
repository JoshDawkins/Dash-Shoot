using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Enemy Wave")]
public class EnemyWave : ScriptableObject, IEnumerable<EnemyWave.EnemySpawnConfig>
{
    [System.Serializable]
    public class EnemySpawnConfig {
        public int spawnIndex;
        public EnemyManager.EnemyType enemyType;
    }

    [SerializeField]
    private List<EnemySpawnConfig> spawnConfigs = new List<EnemySpawnConfig>();

    public EnemySpawnConfig this[int i] => spawnConfigs[i];

	public IEnumerator<EnemySpawnConfig> GetEnumerator() {
        return spawnConfigs.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
        return spawnConfigs.GetEnumerator();
    }
}
