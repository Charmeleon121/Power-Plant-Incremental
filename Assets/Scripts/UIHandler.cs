using TMPro;
using UnityEngine;

public class UIHandler : MonoBehaviour {
	// UI text elements
	public TextMeshProUGUI powerStatsText, hoverText;

	public void SetHoverText(string text) {
		hoverText.text = text;
	}
}
