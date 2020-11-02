using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager
{
    private static List<GameObject> enemies = new List<GameObject>();

    public static void AddEnemy(GameObject newEnemy) {
        enemies.Add(newEnemy);
    }

    public static void AddEnemies(params GameObject[] newEnemies) {
        enemies.AddRange(newEnemies);
    }

    public static void EnemyDestroyed(GameObject enemy) {
		enemies.Remove(enemy);
		if (enemies.Count == 0) {
            //When all enemies have been defeated, we win!
            GameController.Win();
		}
	}

    public static void Clear() {
        enemies.Clear();
    }

    public static void SetAllEnemiesActive(bool active) {
        enemies.ForEach(e => e.SetActive(active));
    }
}
