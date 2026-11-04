using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {
	// External scripts
	private Player player;
	private PowerSupplyAndDemandHandler psadHandler;

	// UI element prefabs
	private GameObject unlockButtonPrefab;

	// UI elements
	public TextMeshProUGUI powerStatsText, objNameText, objInfoText, moneyText, fpsText;
	public GameObject hoverPanel, unlockPanel;
	public Transform unlockPanelContent;

	// Upgrade costs, and whether the upgrades have been purchased
	private List<float> upgradeCosts;

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
		// Initialize the external script variables
		player = GameObject.Find("Player").GetComponent<Player>();
		psadHandler = GetComponent<PowerSupplyAndDemandHandler>();

		unlockButtonPrefab = Resources.Load<GameObject>("UI Prefabs/Unlock Tree Button");
		
		// Ensure the unlock window begins invisible
		unlockPanel.SetActive(false);
		unlockPanelLastToggled = DateTime.UtcNow;

		// Ensure the hover panel begins invisible
		ToggleHoverPanel(false);

		upgradeCosts = new();

		// Create the buttons for the unlock window
		CreateUnlockPanelButton("Manual", 0, "Improved Gear Ratios", 3.00f, "powerGen", 1.0f, 1.1f);
		Transform[] parentButton = { unlockPanelContent.GetChild(0) };
		CreateUnlockPanelButton("Manual", 1, "Handcrank Lubricant", 5.00f, "fuelDelta", 1000, 1100, parentButton);
		CreateUnlockPanelButton("Manual", 1, "test", 5.00f, "fuelDelta", 1000, 1100, parentButton);

		// Assign the generic functionality to the unlock window buttons
		AssignGenericFunctionsToButtons();

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

		for (int i = 0; i < unlockPanelContent.childCount; ++i) {
			try {
				Button button = unlockPanelContent.GetChild(i).GetComponent<Button>();
				UnlockButton ubScript = button.GetComponent<UnlockButton>();
				
				if (player.money >= upgradeCosts[i] && !ubScript.purchased) {
					ubScript.unlocked = true;
					button.image.color = unlockedButtonColor;
					button.interactable = true;
				}
			} catch (Exception) {
				continue;
			}
		}
	}

	/*
	 * Create a button for the unlock window
	 * 
	 * Parameters:
	 * targetType:     Indicates what the button will affect ("Manual" for manual generator, "Player" for the player, etc.)
	 * tier:           Indicates what tier the upgrade is - higher tier means further to the right in the window, more expensive
	 *			           and providing better upgrades
	 * title:          The title displayed at the top of the button
	 * cost:           The cost of purchasing this upgrade (values will be truncated to 2d.p.)
	 * modifierTarget: Indicates what specific aspect of the target is being upgraded ("powerGen", "fuelDelta", etc.)
	 * initValue:      What the value of the target is before being upgraded
	 * newValue:       What the button will upgrade that value to
	 * parents:        The button or buttons directly before this one in the upgrade tree structure - leave blank for starting upgrades
	 */
	private void CreateUnlockPanelButton(string targetType, int tier, string title, float cost, string modifierTarget, float initValue, float newValue, Transform[] parents = null) {
		List<Transform> existingButtons = new();
		for (int i = 0; i < unlockPanelContent.childCount; ++i) {
			existingButtons.Add(unlockPanelContent.GetChild(i));
		}
		
		Vector3 position = new(-750f + (tier * 300), 0f, 0f);

		switch (targetType) {
			case "Manual":
				bool found = false;
				
				UnlockButton ub;
				foreach (Transform btn in existingButtons) {
					ub = btn.gameObject.GetComponent<UnlockButton>();
					if (ub.position[0] == position.x && ub.position[1] == 380f) {
						Vector3 prevBtnPos = new(ub.position[0], 450f, 0f);
						
						// Somehow, these two are saving entirely different vectors.
						btn.localPosition = prevBtnPos;
						ub.position = prevBtnPos;
						
						position = new(position.x, 310f, 0f);

						found = true;
						break;
					}
				}

				if (!found) {
					position = new(position.x, 380f, 0f);
				}

				break;
		}

		GameObject newBtnObj = Instantiate(unlockButtonPrefab, new(0f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), unlockPanelContent);
		newBtnObj.GetComponent<RectTransform>().anchoredPosition = position;
		
		UnlockButton ubScript = newBtnObj.GetComponent<UnlockButton>();
		ubScript.position = position;
		ubScript.parents = parents;
		ubScript.unlocked = false;
		ubScript.purchased = false;

		Button newBtn = newBtnObj.GetComponent<Button>();
		
		newBtn.image.color = lockedButtonColor;
		newBtn.interactable = false;
		newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = title;
		newBtn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Cost: ${cost:n2}";

		switch (modifierTarget) {
			case "powerGen":
				newBtn.onClick.AddListener(() => {
					psadHandler.SetPowerGenMultiplier(targetType, newValue);
				});

				newBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Power production modifier:\n{initValue:n2} → {newValue:n2}";
				break;
			case "fuelDelta":
				newBtn.onClick.AddListener(() => {
					psadHandler.SetMaxFuelDelta(targetType, Mathf.RoundToInt(newValue));
				});

				newBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Time between fuel uses:\n{initValue / 1e3f:n2} → {newValue / 1e3f:n2}";
				break;
		}

		upgradeCosts.Add(cost);
	}

	/*
	 * Assign functions to all buttons on the UI
	 */
	private void AssignGenericFunctionsToButtons() {
		for (int i = 0; i < unlockPanelContent.childCount; ++i) {
			try {
				Button button = unlockPanelContent.GetChild(i).GetComponent<Button>();

				int index = i;
				button.onClick.AddListener(() => {
					player.money -= upgradeCosts[index];
					button.GetComponent<UnlockButton>().purchased = true;
					button.image.color = purchasedButtonColor;
					button.interactable = false;
				});
			} catch {
				continue;
			}
		}
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
