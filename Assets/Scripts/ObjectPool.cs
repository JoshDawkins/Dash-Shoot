using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
	private T[] pool;
	private readonly T template;
	private readonly bool fixedSize;
	private readonly int growthIncrement;
	
	public ObjectPool(T template, int poolSize, bool fixedSize = false) {
		pool = new T[poolSize];
		this.template = template;
		this.fixedSize = fixedSize;
		growthIncrement = poolSize;

		//Fill the array with copies of the template, and immediately deactivate them
		for (int i = 0; i < poolSize; i++) {
			pool[i] = Object.Instantiate(template);
			pool[i].gameObject.SetActive(false);
		}
	}

	public ObjectPool(T template, int poolSize, int growthIncrement) : this(template, poolSize, false) {
		this.growthIncrement = growthIncrement;
	}

	public T SpawnFromPool(Vector3 position, Quaternion rotation) {
		//Get the first inactive element from the pool
		T spawned = pool.FirstOrDefault(o => !o.gameObject.activeInHierarchy);

		if (spawned == null) {//No inactive elements, so we should grow if possible
			if (fixedSize)
				return null;//This pool is not allowed to grow, so all we can do is return null to indicate no more can be spawned

			//Otherwise, we will increase the size by the growthIncrement
			int oldSize = pool.Length;
			System.Array.Resize(ref pool, oldSize + growthIncrement);

			//Fill in the new elements with fresh, deactivated copies of the template
			for (int i = oldSize; i < pool.Length; i++) {
				pool[i] = Object.Instantiate(template);
				pool[i].gameObject.SetActive(false);
			}

			//Finally, we'll want to get the first of the new elements
			spawned = pool[oldSize];
		}

		//Activate and set the transform position and rotation
		spawned.transform.position = position;
		spawned.transform.rotation = rotation;
		spawned.gameObject.SetActive(true);

		//Finally, return the spawned object
		return spawned;
	}

	public void DisableAllPooledObjects() {
		foreach (T obj in pool) {
			obj.gameObject.SetActive(false);
		}
	}
}
