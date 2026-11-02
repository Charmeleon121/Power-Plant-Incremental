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
	public Button[] unlockButtons = new Button[1];

	// Upgrade costs, and whether the upgrades have been purchased
	private readonly float[] upgradeCosts = { 5.00f };
	private bool[] upgradesPurchased = { false };

	// Color values
	private readonly Color lockedButtonColor = new(255f, 170f / 255f, 170f / 255f);
	private readonly Color unlockedButtonColor = new(170f / 255f, 255f, 170f / 255f);
	private readonly Color purchasedButtonColor = new(100f / 255f, 100f / 255f, 100f / 255f);

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

		foreach (Button btn in unlockButtons) {
			btn.image.color = lockedButtonColor;
			btn.interactable = false;
		}
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

		for (int i = 0; i < upgradeCosts.Length; ++i) {
			if (player.money >= upgradeCosts[i] && !upgradesPurchased[i]) {
				unlockButtons[i].image.color = unlockedButtonColor;
				unlockButtons[i].interactable = true;
			}
		}
	}

	/*
	 * Assign functions to all buttons on the UI
	 */
	private void AssignFunctionsToButtons() {
		unlockButtons[0].onClick.AddListener(() => {
			psadHandler.SetPowerGenMultiplier("Manual", 1.1f);
			player.money -= upgradeCosts[0];

			unlockButtons[0].image.color = purchasedButtonColor;
			upgradesPurchased[0] = true;
			unlockButtons[0].interactable = false;
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
