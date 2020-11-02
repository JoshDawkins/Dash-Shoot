﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	public static GameController Instance { get; private set; }
	public static Player Player { get; private set; }
	public static bool Playing { get; private set; }

	[SerializeField]
	private Text controlsLbl = null,
		countdownLbl = null,
		winLoseLbl = null,
		retryLbl = null;
	[SerializeField]
	private CinemachineVirtualCamera mainCam = null;

	private void Awake() {
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(this);
	}

	private IEnumerator Start() {
		Player = FindObjectOfType<Player>();

		yield return InitializeGame();
	}

	private IEnumerator InitializeGame() {
		//Make sure timescale has been reset, in case we're restarting
		Time.timeScale = 1;

		//We are not currently ready to play
		Playing = false;

		//Make sure UI is in the right state
		controlsLbl.gameObject.SetActive(true);
		countdownLbl.gameObject.SetActive(false);
		winLoseLbl.gameObject.SetActive(false);
		retryLbl.gameObject.SetActive(false);
		
		//Reset player and disable control
		Player.transform.position = new Vector3(0, 0.33f, 0);
		Player.transform.rotation = Quaternion.LookRotation(Vector3.forward);
		Player.ResetHealth();
		Player.enabled = false;

		//Force the cinemachine camera to recenter on the player
		mainCam.gameObject.SetActive(false);
		yield return null;//Wait 1 frame, otherwise the deactivation/reactivation necessary to recenter will be skipped as trivial
		mainCam.gameObject.SetActive(true);

		//If level 1 is not already loaded (such as in the editor), load it
		Scene lvl1 = SceneManager.GetSceneByName("Level1");
		if (!lvl1.isLoaded) {
			yield return SceneManager.LoadSceneAsync("Level1", LoadSceneMode.Additive);
			lvl1 = SceneManager.GetSceneByName("Level1");
		}
		SceneManager.SetActiveScene(lvl1);

		//Wait for the player to press the spacebar before starting the game for realsies
		StartCoroutine(StartAfterInput());
	}

	private IEnumerator StartAfterInput() {
		//Wait each frame until the user presses the spacebar
		while (true) {
			if (Input.GetKeyDown(KeyCode.Space))
				break;

			if (Input.GetKeyDown(KeyCode.Escape)) {
				Quit();
				yield break;
			}

			yield return null;
		}

		//Start countdown timer
		countdownLbl.gameObject.SetActive(true);
		for (int i = 3; i > 0; i--) {
			countdownLbl.text = i.ToString();
			yield return new WaitForSeconds(1);
		}
		countdownLbl.gameObject.SetActive(false);
		controlsLbl.gameObject.SetActive(false);

		//Enable the player and enemies
		Player.enabled = true;
		EnemyManager.SetAllEnemiesActive(true);

		//Ready to play
		Playing = true;
	}

	//End of game
	public static void Win() {
		Instance.WinLose(true);
	}

	public static void Lose() {
		Instance.WinLose(false);
	}

	private void WinLose(bool win) {
		//Show win and retry messages
		winLoseLbl.gameObject.SetActive(true);
		winLoseLbl.text = win ? "YOU WIN!" : "YOU LOSE!";
		retryLbl.gameObject.SetActive(true);

		//Disable player
		Player.enabled = false;
		Player.Rb.velocity = Vector3.zero;
		Player.Rb.constraints = RigidbodyConstraints.FreezeAll;

		//Pause everything
		Time.timeScale = 0;

		//Begin the coroutine to restart the game on a keypress
		StartCoroutine(Instance.RestartAfterKeyPress());
	}

	//Instance method with the necessary signature to be called as an OnDeath event by the player's Killable component
	public void Lose(Killable.DamageData damageData) {
		WinLose(false);
	}

	private IEnumerator RestartAfterKeyPress() {
		//Wait for a keypress
		while (true) {
			if (Input.GetKeyDown(KeyCode.R))
				break;

			if (Input.GetKeyDown(KeyCode.Escape)) {
				Quit();
				yield break;
			}

			yield return null;
		}

		//Unload Level1
		EnemyManager.Clear();
		yield return SceneManager.UnloadSceneAsync("Level1");

		//Reinitialize the game
		yield return InitializeGame();
	}

	public static void Quit() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
         Application.Quit();
		#endif
	}
}
