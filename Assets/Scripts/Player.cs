using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
	// External scripts
	private UIHandler uiHandler;

	// Input actions
	private InputSystem_Actions keybinds;
	private InputAction useAction, moveAction, lookAction, unlockMenuAction;

	// Variables relating to player movement
	private Rigidbody rb;
	private readonly float walkSpeed = 5f;
	private readonly float lookSpeed = 10f;
	public bool lookAndMoveActive;

	// Player inventory and monehy
	public float money = 0f;
	public float moneyMult = 0.1f;

	/*
	 * Initialize keybinds on Awake
	 */
	private void Awake() {
		keybinds = new();
		keybinds.Player.Enable();
	}

	/*
	 * Start method
	 */
	private void Start() {
		DontDestroyOnLoad(this);

		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();

		useAction = InputSystem.actions.FindAction("Player/Use");
		moveAction = InputSystem.actions.FindAction("Player/Move");
		lookAction = InputSystem.actions.FindAction("Player/Look");
		unlockMenuAction = InputSystem.actions.FindAction("Player/Unlock Menu");
		
		rb = GetComponent<Rigidbody>();
		lookAndMoveActive = true;
	}

	/*
	 * Update method - non-physics-related code should run in here
	 */
	private void Update() {
		if (lookAndMoveActive) {
			Look();
			Interaction();
		}
		
		if (unlockMenuAction.triggered) {
			uiHandler.ToggleUnlockPanel();
		}
	}

	/*
	 * FixedUpdate method - all player movement and similar physics-related
	 * code should be run from here
	 */
	private void FixedUpdate() {
		if (lookAndMoveActive) {
			Move();
		}
	}

	/*
	 * Look method - this is for turning the player left/right, and the camera up/down
	 */
	private void Look() {
		Vector2 inputVector = lookAction.ReadValue<Vector2>();

		float playerLookRot = transform.localRotation.eulerAngles.y + (lookSpeed * inputVector.x * Time.fixedDeltaTime);
		float cameraLookRot = Camera.main.transform.localRotation.eulerAngles.x + (lookSpeed * -inputVector.y * Time.fixedDeltaTime);

		transform.rotation = Quaternion.Euler(0f, playerLookRot, 0f);
		Camera.main.transform.localRotation = Quaternion.Euler(cameraLookRot, 0f, 0f);
	}

	/*
	 * Interact method - this is for handling the player interacting with objects/the environment
	 */
	private void Interaction() {
		GameObject target = null;
		PowerSupplier psScript = null;

		// Obtain the target GameObject, and set the hover text to display information
		if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 3f)) {
			if (hit.collider.CompareTag("Generator")) {
				target = hit.collider.gameObject;
				psScript = target.GetComponent<PowerSupplier>();

				string activeState;
				if (psScript.isGeneratorRunning) {
					activeState = "Active";
				} else {
					activeState = "Inactive";
				}

				uiHandler.ToggleHoverPanel(true, psScript.isGeneratorRunning);
				uiHandler.SetHoverText(hit.collider.name, activeState, psScript.currentFuelLevel, psScript.maxFuelLevel, psScript.currentlyProducing, psScript.powerMultiplier);
			} else {
				uiHandler.ToggleHoverPanel(false);
			}
		} else {
			uiHandler.ToggleHoverPanel(false);
		}

		if (useAction.IsPressed() && target != null) {
			if (target.CompareTag("Generator")) {
				// Prevent the click from registering multiple times in one go by enforcing a time delay between clicks
				double delta = DateTime.UtcNow.Subtract(psScript.lastClicked).TotalMilliseconds;
				
				if (delta >= 150) {
					if (psScript.isGeneratorRunning) {
						psScript.Deactivate();
					} else {
						psScript.Activate();
					}

					psScript.lastClicked = DateTime.UtcNow;
				}
			}
		}
	}

	/*
	 * Move method - this is for moving the player
	 */
	private void Move() {
		Vector2 inputVector = moveAction.ReadValue<Vector2>();
		Vector3 moveVector = (transform.forward * inputVector.y) + (transform.right * inputVector.x);

		if (moveVector.magnitude > 0f) {
			rb.linearVelocity = walkSpeed * moveVector.normalized;
		} else {
			rb.linearVelocity = Vector3.zero;
		}
	}

	/*
	 * Set the money multiplier value
	 */
	public void SetMoneyMultiplier(float value) {
		moneyMult = value;
	}
}
