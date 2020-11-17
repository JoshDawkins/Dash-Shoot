using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Projectile Manager")]
public class ProjectileManager : ScriptableObject
{
    //Enum for projectile types
    public enum ProjectileType {
        Player,
        Basic
    }

    //POCO class for configuring projectile mappings
    [System.Serializable]
    public class ProjectileConfig {
        public ProjectileType type;
        public Projectile prefab;
        public int poolSize;
    }

    [SerializeField]
    private ProjectileConfig[] projectileConfigs = null;

    //Map each type to a pool of projectiles
    private Dictionary<ProjectileType, ObjectPool<Projectile>> pools;

    //Should be called in the GameController's Start() method it initialize the object pools
	public void Init() {
        pools = new Dictionary<ProjectileType, ObjectPool<Projectile>>(projectileConfigs.Length);

        ProjectileConfig pc;
		for (int i = 0; i < projectileConfigs.Length; i++) {
            pc = projectileConfigs[i];

            pools.Add(pc.type, new ObjectPool<Projectile>(pc.prefab, pc.poolSize, 5));
		}
	}

    //Called whenever a projectile is needed, to get one of the appropriate type from its object pool
    public Projectile SpawnProjectile(ProjectileType type, Vector3 position, Quaternion rotation) {
        return pools[type]?.SpawnFromPool(position, rotation);
    }

    //Called when the player restarts to ensure that no projectiles are left hanging around
    public void ClearAllProjectiles() {
        foreach (var p in pools.Values)
            p.DisableAllPooledObjects();
    }
}
