using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    private int index = 0;

    public int Index { get => index; }

	private void OnDrawGizmos() {
		Gizmos.color = new Color(1, .5f, 0);//Orange
		Gizmos.DrawLine(transform.position, transform.position + (transform.forward * 2));
	}
}
