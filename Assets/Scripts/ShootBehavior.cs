using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBehavior : MonoBehaviour
{
    [SerializeField]
    private float rateOfFire = 0.33f,
        aimSpeed = 4.0f,
        projectileOffset = 0.7f;
    [SerializeField]
    private Projectile projectilePrefab = null;

    private float shootTimer = 0.0f;

	private void Start() {
        //Randomize when the first shot will go off, so they aren't all shooting in perfect sync
        shootTimer = Random.Range(0.0f, rateOfFire);
	}

	private void Update() {
        //Update the shoot timer if necessary
        if (shootTimer > 0.0f)
            shootTimer = Mathf.Max(0.0f, shootTimer - Time.deltaTime);

        //Aim at the player, lerping so it is not an immediate snap to the player
        Vector3 toPlayer = GameController.Player.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * aimSpeed);

        if (shootTimer == 0.0f) {
            Instantiate(projectilePrefab, transform.position + (transform.forward * projectileOffset), transform.rotation);
            shootTimer = rateOfFire;
        }
	}
}
