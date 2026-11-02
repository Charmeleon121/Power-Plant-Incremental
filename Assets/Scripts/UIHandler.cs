using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour {
	// External scripts
	private Player player;
	
	// UI text elements
	public TextMeshProUGUI powerStatsText, hoverText, moneyText;

	/*
	 * Start method
	 */
	void Start() {
		player = GameObject.Find("Player").GetComponent<Player>();
	}

	/*
	 * Update method
	 */
	void Update() {
		string moneyString = player.money.ToString("N2");
		moneyText.text = $"${moneyString}";
	}

	// Update the hover text
	public void SetHoverText(string text) {
		hoverText.text = text;
	}
}
