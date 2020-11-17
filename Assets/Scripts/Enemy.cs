using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Killable))]
public class Enemy : MonoBehaviour
{
	private Killable killable = null;
	
	//private void Start() {
	//	//Register this new enemy with the manager
	//	//EnemyManager.AddEnemy(gameObject);

	//	//Disable if the game isn't ready to play yet
	//	if (!GameController.Playing)
	//		gameObject.SetActive(false);
	//}

	private void OnEnable() {
		//Find the object's Killable component and listen for the death event
		killable = GetComponent<Killable>();
		killable.ResetHealth();
		killable.AddDeathListener(OnDeath);
	}

	private void OnDisable() {
		killable.RemoveDeathListener(OnDeath);
	}

	//Method to be called when the enemy dies, to deregister from the manager
	private void OnDeath(Killable.DamageData damageData) {
		GameController.EnemyManager.EnemyDestroyed(gameObject);
	}
}
