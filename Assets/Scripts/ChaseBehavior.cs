using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Killable))]
public class ChaseBehavior : MonoBehaviour
{
    [SerializeField]
    private float speed = 8.0f,
        aimSpeed = 4.0f;
    [SerializeField]
    private int damage = 1;

    private Rigidbody rb;
    private Killable killable;

	private void Start() {
        rb = GetComponent<Rigidbody>();
        killable = GetComponent<Killable>();
	}

	private void Update() {
        //Aim at the player, lerping so it is not an immediate snap to the player
        Vector3 toPlayer = GameController.Player.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(toPlayer), Time.deltaTime * aimSpeed);

        //Move forward
        rb.velocity = transform.forward * speed;
    }

	private void OnCollisionEnter(Collision collision) {
        //Damage the other object if it has a Killable component (meaning it can take damage)
        collision.gameObject.GetComponent<Killable>()?.ApplyDamage(damage);

        //Destroy self
        killable.ApplyDamage(killable.CurrentHealth);
    }
}
