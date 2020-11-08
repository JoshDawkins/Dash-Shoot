using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DashEffect : MonoBehaviour
{
	[SerializeField]
	private int damage = 2;

	private void OnTriggerEnter(Collider other) {
		//Destroy any projectiles that touch the dash effect
		if (other.GetComponent<Projectile>() != null) {
			other.gameObject.SetActive(false);
		}

		//Damage any killable objects we hit
		Killable otherKillable = other.GetComponent<Killable>();
		if (otherKillable != null) {
			otherKillable.ApplyDamage(damage);
		}
	}
}
