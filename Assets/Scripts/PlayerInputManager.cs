using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class PlayerInputManager : ScriptableObject, IGameplayActions {
	public float MoveX { get; private set; }
	public float MoveY { get; private set; }
	public Vector3 Move { get => new Vector3(MoveX, 0, MoveY); }//Don't forget, Unity flips the Y and Z axes from the logical interpretation
	public float AimX { get; private set; }
	public float AimY { get; private set; }
	public Vector3 Aim { get => new Vector3(AimX, 0, AimY); }//Don't forget, Unity flips the Y and Z axes from the logical interpretation
	public Vector2 MousePosition { get; private set; }
	public float LastMouseTime { get; private set; }
	public bool Dash {
		get {
			if (dash) {//This is sort of a hack to make sure it only registers the initial press... need to find a better way
				dash = false;
				return true;
			}
			return false;
		}
		private set { dash = value; }
	}
	public bool Shoot { get; private set; }

	private PlayerInputActions inputs;
	private bool dash;
	
	//Enable and set callbacks for the player input
	private void OnEnable() {
		inputs = new PlayerInputActions();
		inputs.Gameplay.SetCallbacks(this);
		inputs.Gameplay.Enable();
	}

	//Disable and dispose of the player input
	private void OnDisable() {
		inputs.Gameplay.Disable();
		inputs.Dispose();
		inputs = null;
	}

	public void OnMoveX(InputAction.CallbackContext context) {
		MoveX = context.ReadValue<float>();
	}

	public void OnMoveY(InputAction.CallbackContext context) {
		MoveY = context.ReadValue<float>();
	}

	public void OnAimX(InputAction.CallbackContext context) {
		AimX = context.ReadValue<float>();
	}

	public void OnAimY(InputAction.CallbackContext context) {
		AimY = context.ReadValue<float>();
	}

	public void OnMousePosition(InputAction.CallbackContext context) {
		MousePosition = context.ReadValue<Vector2>();
	}

	public void OnMouseDelta(InputAction.CallbackContext context) {
		//We don't actually care what the value of the mouse delta is, only when the last mouse movement occurred
		LastMouseTime = Time.time;
	}

	public void OnDash(InputAction.CallbackContext context) {
		if (context.started)
			Dash = true;
		if (context.canceled)
			Dash = false;
	}

	public void OnShoot(InputAction.CallbackContext context) {
		if (context.started)
			Shoot = true;
		if (context.canceled)
			Shoot = false;
	}
}
