using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
	[SerializeField]
	private float movementSpeed = 6.0f,
		dashSpeed = 10.0f,
		dashDuration = 1.0f,
		rateOfFire = 0.33f,
		projectileOffset = 0.7f;
	[SerializeField]
	//private Projectile projectilePrefab = null;
	private ProjectileManager.ProjectileType projectileType = ProjectileManager.ProjectileType.Player;
	[SerializeField]
	private Killable killable = null;
	[SerializeField]
	private GameObject dashEffect = null;
	
	public Rigidbody Rb { get; private set; }

	private Vector3 movedir;
	private float shootTimer = 0.0f;
	private Coroutine dashCoroutine = null;
	//private ObjectPool<Projectile> projectilePool = null;

	private void Awake() {
		Rb = GetComponent<Rigidbody>();
	}

	//private void Start() {
	//	projectilePool = new ObjectPool<Projectile>(projectilePrefab, 10, 5);
	//}

	private void Update() {
		//Update the timers if necessary
		if (shootTimer > 0.0f)
			shootTimer = Mathf.Max(0.0f, shootTimer - Time.deltaTime);

		//If we're currently dashing, then don't do anything else
		if (dashCoroutine != null)
			return;

		//Handle movement
		movedir = Vector3.zero;

		if (Input.GetKey(KeyCode.W))
			movedir += Vector3.forward;
		if (Input.GetKey(KeyCode.S))
			movedir += Vector3.back;
		if (Input.GetKey(KeyCode.A))
			movedir += Vector3.left;
		if (Input.GetKey(KeyCode.D))
			movedir += Vector3.right;

		if (movedir == Vector3.zero) {
			Rb.velocity = movedir;
			Rb.constraints = RigidbodyConstraints.FreezeAll;
		} else {
			movedir.Normalize();
			Rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
			Rb.velocity = movedir * movementSpeed;
		}

		//Rotate to face the mouse
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit)) {
			transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		}

		//Shoot if applicable and able
		if (Input.GetMouseButton(0) && shootTimer == 0.0f) {
			//Instantiate(projectilePrefab, transform.position + (transform.forward * projectileOffset), transform.rotation);
			//projectilePool.SpawnFromPool(transform.position + (transform.forward * projectileOffset), transform.rotation);
			GameController.ProjectileManager.SpawnProjectile(projectileType, transform.position + (transform.forward * projectileOffset), transform.rotation);
			shootTimer = rateOfFire;
		}

		//Start a dash when the spacebar is pressed
		if (Input.GetKeyDown(KeyCode.Space)) {
			dashCoroutine = StartCoroutine(Dash());
		}
	}

	public void ResetHealth() {
		killable.ResetHealth();
	}

	private IEnumerator Dash() {
		//Make sure we can move
		Rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

		//Enable the dash effect
		dashEffect.SetActive(true);

		//Disable the killable component so we can't take damage
		killable.enabled = false;

		//Snap the player's facing to the movement direction, and launch the player forward
		if (movedir != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(movedir);
		Rb.velocity = transform.forward * dashSpeed;

		//Wait for dashDuration seconds
		yield return new WaitForSeconds(dashDuration);

		//Disable the dash effect, enable the killable so we can take damage again, and null out the coroutine
		dashEffect.SetActive(false);
		killable.enabled = true;
		dashCoroutine = null;
	}
}
