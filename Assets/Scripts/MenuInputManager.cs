using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

public class MenuInputManager : IMenuActions, IDisposable {
	private PlayerInputActions input;

	//Constructor in initialize the PlayerInputActions
	public MenuInputManager() {
		input = new PlayerInputActions();
		input.Menu.SetCallbacks(this);
		input.Menu.Enable();
	}

	//Destructor to ensure Dispose is always called
	~MenuInputManager() {
		Dispose();
	}

	//Dispose makes sure the PlayerInputActions object is cleaned up
	public void Dispose() {
		if (input != null) {
			input.Menu.Disable();
			input.Dispose();
			input = null; 
		}
	}

	//Input flags
	public bool SelectPressed { get; private set; } = false;
	public bool BackPressed { get; private set; } = false;

	//Input buttons
	public void OnSelect(InputAction.CallbackContext context) {
		if (context.started)
			SelectPressed = true;
		if (context.canceled)
			SelectPressed = false;
	}

	public void OnBack(InputAction.CallbackContext context) {
		if (context.started)
			BackPressed = true;
		if (context.canceled)
			BackPressed = false;
	}
}
