using System;
using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour {
	// External scripts
	private Player player;
	
	// UI elements
	public TextMeshProUGUI powerStatsText, hoverText, moneyText, fpsText;
	public GameObject unlockPanel;
	
	// Timer values
	private DateTime unlockPanelLastToggled;

	/*
	 * Start method
	 */
	void Start() {
		player = GameObject.Find("Player").GetComponent<Player>();
		
		unlockPanel.SetActive(false);
		unlockPanelLastToggled = DateTime.UtcNow;
	}

	/*
	 * Update method
	 */
	void Update() {
		string moneyString = player.money.ToString("N2");
		moneyText.text = $"${moneyString}";
		
		fpsText.text = $"FPS: {1f / Time.deltaTime:n2}";
	}

	// Update the hover text
	public void SetHoverText(string text) {
		hoverText.text = text;
	}
	
	// Toggle visibility of the unlock panel
	public void ToggleUnlockPanel() {
		int delta = DateTime.UtcNow.Subtract(unlockPanelLastToggled).Milliseconds;

		if (delta >= 200) {
			unlockPanelLastToggled = DateTime.UtcNow;
			unlockPanel.SetActive(!unlockPanel.activeSelf);
			player.lookAndMoveActive = !player.lookAndMoveActive;
		}
	}
}
