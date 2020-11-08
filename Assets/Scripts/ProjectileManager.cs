using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Projectile Manager")]
public class ProjectileManager : ScriptableObject
{
    public enum ProjectileType {
        Player,
        Basic
    }

    [System.Serializable]
    public class ProjectileConfig {
        public ProjectileType type;
        public Projectile prefab;
        public int poolSize;
    }

    [SerializeField]
    private ProjectileConfig[] projectileConfigs = null;

    private Dictionary<ProjectileType, ObjectPool<Projectile>> pools;

	public void Init() {
        pools = new Dictionary<ProjectileType, ObjectPool<Projectile>>(projectileConfigs.Length);

        ProjectileConfig pc;
		for (int i = 0; i < projectileConfigs.Length; i++) {
            pc = projectileConfigs[i];

            pools.Add(pc.type, new ObjectPool<Projectile>(pc.prefab, pc.poolSize, 5));
		}
	}

    public Projectile SpawnProjectile(ProjectileType type, Vector3 position, Quaternion rotation) {
        return pools[type]?.SpawnFromPool(position, rotation);
    }
}
