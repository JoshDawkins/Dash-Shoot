using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class Healthbar : MonoBehaviour
{
    [SerializeField]
    private Killable trackedObject = null;

    private Slider slider;

	private void Start() {
		slider = GetComponent<Slider>();
	}

	private void Update() {
		if (trackedObject == null) {
			Destroy(gameObject);//Destroy the healthbar if the object it's tracking has been destroyed
			return;
		}

		//Update the slider's values to properly show the tracked object's health
		slider.maxValue = trackedObject.MaxHealth;
		slider.value = trackedObject.CurrentHealth;
	}
}
