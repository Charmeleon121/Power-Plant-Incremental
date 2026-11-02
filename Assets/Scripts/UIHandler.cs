using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
	// External scripts
	private Player player;
	private PowerSupplyAndDemandHandler psadHandler;
	
	// UI elements
	public TextMeshProUGUI powerStatsText, objNameText, objInfoText, moneyText, fpsText;
	public GameObject hoverPanel, unlockPanel;
	public Button unlockButton00;
	
	// Timer values
	private DateTime unlockPanelLastToggled;

	/*
	 * Start method
	 */
	void Start() {
		player = GameObject.Find("Player").GetComponent<Player>();
		psadHandler = GetComponent<PowerSupplyAndDemandHandler>();
		
		unlockPanel.SetActive(false);
		unlockPanelLastToggled = DateTime.UtcNow;

		ToggleHoverPanel(false);

		AssignFunctionsToButtons();

		// Lock the cursor to the window and hide it
		ToggleCursorLockState(true);
	}

	/*
	 * Update method
	 */
	void Update() {
		string moneyString = player.money.ToString("N2");
		moneyText.text = $"${moneyString}";
		
		fpsText.text = $"FPS: {1f / Time.deltaTime:n2}";
	}

	/*
	 * Assign functions to all buttons on the UI
	 */
	private void AssignFunctionsToButtons() {
		unlockButton00.onClick.AddListener(() => {
			psadHandler.SetPowerGenMultiplier("Manual", 1.1f);
		});
	}

	/*
	 * Toggle visibility of the hover text panel
	 */
	public void ToggleHoverPanel(bool state) {
		hoverPanel.SetActive(state);
	}

	/*
	 * Update the hover text
	 */
	public void SetHoverText(string name, string state, float currentFuel, float maxFuel, float producing, float modifier) {
		objNameText.text = name;

		string prefix;
		if (producing < 1e3) {
			prefix = "";
		} else if (producing >= 1e3 && producing < 1e6) {
			prefix = "k";
		} else if (producing >= 1e6 && producing < 1e9) {
			prefix = "M";
		} else {
			prefix = "G";
		}

		objInfoText.text = $"{state}\n{currentFuel:n2}/{maxFuel:n2}\n{producing:n2} {prefix}W\n{modifier:n2}x";
	}
	
	/*
	 * Toggle visibility of the unlock panel
	 */
	public void ToggleUnlockPanel() {
		double delta = DateTime.UtcNow.Subtract(unlockPanelLastToggled).TotalMilliseconds;

		if (delta >= 150) {
			unlockPanelLastToggled = DateTime.UtcNow;
			unlockPanel.SetActive(!unlockPanel.activeSelf);
			player.lookAndMoveActive = !player.lookAndMoveActive;

			ToggleCursorLockState(!unlockPanel.activeSelf);
		}
	}

	/*
	 * Change the lock state and visibility of the cursor
	 */
	private void ToggleCursorLockState(bool locked) {
		if (locked) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
