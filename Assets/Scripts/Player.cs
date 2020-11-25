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
	private ProjectileManager.ProjectileType projectileType = ProjectileManager.ProjectileType.Player;
	[SerializeField]
	private Killable killable = null;
	[SerializeField]
	private GameObject dashEffect = null;
	
	public Rigidbody Rb { get; private set; }

	private Vector3 movedir;
	private float shootTimer = 0.0f;
	private Coroutine dashCoroutine = null;
	private PlayerInputManager input;

	private void Awake() {
		Rb = GetComponent<Rigidbody>();
		input = ScriptableObject.CreateInstance<PlayerInputManager>();
	}

	private void Update() {
		//Update the timers if necessary
		if (shootTimer > 0.0f)
			shootTimer = Mathf.Max(0.0f, shootTimer - Time.deltaTime);

		//If we're currently dashing, then don't do anything else
		if (dashCoroutine != null)
			return;

		//Handle movement
		movedir = input.Move;//Vector2 is implicitly converted to a Vector3 with a z value of 0

		if (movedir == Vector3.zero) {
			Rb.velocity = movedir;
			Rb.constraints = RigidbodyConstraints.FreezeAll;
		} else {
			movedir.Normalize();
			Rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
			Rb.velocity = movedir * movementSpeed;
		}

		//Use the Aim axes to determine facing if they are input from the keyboard or gamepad
		if (input.Aim != Vector3.zero) {
			//Remember, in Unity forward is on the Z-axis, not the Y-axis
			transform.LookAt(transform.position + new Vector3(input.AimX, 0, input.AimY));
		}
		//Otherwise, follow the mouse if it has moved since the previous frame
		else if (input.LastMouseTime >= Time.time - Time.deltaTime) {
			//Rotate to face the mouse
			if (Physics.Raycast(Camera.main.ScreenPointToRay(input.MousePosition), out RaycastHit hit)) {
				transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
			}
		}

		//Shoot if applicable and able
		if (input.Shoot && shootTimer == 0.0f) {
			//Instantiate(projectilePrefab, transform.position + (transform.forward * projectileOffset), transform.rotation);
			//projectilePool.SpawnFromPool(transform.position + (transform.forward * projectileOffset), transform.rotation);
			GameController.ProjectileManager.SpawnProjectile(projectileType, transform.position + (transform.forward * projectileOffset), transform.rotation);
			shootTimer = rateOfFire;
		}

		//Start a dash when the spacebar is pressed
		if (input.Dash) {
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
