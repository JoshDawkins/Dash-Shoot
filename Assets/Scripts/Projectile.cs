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

	private void Start() {
        GetComponent<Renderer>().material.color = color;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}

	private void OnCollisionEnter(Collision collision) {
        //Damage the other object if it has a Killable component (meaning it can take damage)
        collision.gameObject.GetComponent<Killable>()?.ApplyDamage(damage);

        //Destroy the projectile
        Destroy(gameObject);
	}
}
