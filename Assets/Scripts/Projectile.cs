using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer), typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float speed = 10.0f;
    [SerializeField]
    private Color color = Color.green;

    //Temporary variable for use while we are still transitioning to ObjectPools
    //[SerializeField] private bool isPooled = false;

    private Rigidbody rb = null;

	private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

	private void Start() {
        GetComponent<Renderer>().material.color = color;
	}

	private void OnEnable() {
		rb.velocity = transform.forward * speed;
    }

	private void OnCollisionEnter(Collision collision) {
        //Damage the other object if it has a Killable component (meaning it can take damage)
        collision.gameObject.GetComponent<Killable>()?.ApplyDamage(damage);

        //Destroy the projectile
        //if (isPooled)
            gameObject.SetActive(false);
        //else
            //Destroy(gameObject);
	}
}
