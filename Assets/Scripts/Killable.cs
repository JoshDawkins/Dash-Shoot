using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Killable : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 10;
	[SerializeField]
	private bool destroyOnDeath = true;
	[SerializeField]
	private DamageEvent onHurt = new DamageEvent(),
		onDeath = new DamageEvent();

	public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
	public int CurrentHealth { get; private set; }
	public float HealthPercent { get => CurrentHealth / (float)MaxHealth; }

	private void Start() {
		CurrentHealth = MaxHealth;
	}

	private void OnDisable() {
		onHurt.RemoveAllListeners();
		onDeath.RemoveAllListeners();
	}

	//Applies damage to this Killable instance, firing the OnHurt or OnDeath events as necessary
	public void ApplyDamage(int damage) {
		CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
		DamageData damageData = new DamageData() {
			damagedObject = this,
			damageDealt = damage
		};

		//Fire onDeath event and possibly destroy if health has fallen to 0; otherwise just fire onHurt event
		if (CurrentHealth == 0) {
			onDeath.Invoke(damageData);
			if (destroyOnDeath)
				Destroy(gameObject);
		} else
			onHurt.Invoke(damageData);
	}

	public void ResetHealth() {
		CurrentHealth = MaxHealth;
	}


	/*** LISTENERS ***/
	public void AddHurtListener(UnityAction<DamageData> listener) {
		onHurt.AddListener(listener);
	}

	public void AddDeathListener(UnityAction<DamageData> listener) {
		onDeath.AddListener(listener);
	}

	public void AddDamageListeners(UnityAction<DamageData> hurtListener, UnityAction<DamageData> deathListener) {
		AddHurtListener(hurtListener);
		AddDeathListener(deathListener);
	}

	public void RemoveHurtListener(UnityAction<DamageData> listener) {
		onHurt.RemoveListener(listener);
	}

	public void RemoveDeathListener(UnityAction<DamageData> listener) {
		onDeath.RemoveListener(listener);
	}

	public void RemoveDamageListeners(UnityAction<DamageData> hurtListener, UnityAction<DamageData> deathListener) {
		RemoveHurtListener(hurtListener);
		RemoveDeathListener(deathListener);
	}


	/*** UTILITY CLASSES ***/
	[System.Serializable]
	//UnityEvent type for events that involve damage
	public class DamageEvent : UnityEvent<DamageData> { }

	//POCO class that will be used to provide relevant data to DamageEvents
	[System.Serializable]
	public class DamageData {
		public Killable damagedObject;
		public int damageDealt;
	}
}
