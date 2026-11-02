using System;
using UnityEngine;

public class PowerSupplyAndDemandHandler : MonoBehaviour {
	// External scripts
	private UIHandler uiHandler;
	private Player player;

	// All things relating to power supply and demand
	public float powerSupply, powerDemand;
	private int quotasMet = 0;
	private GameObject[] suppliers;

	// UI update timer
	private DateTime uiLastUpdated;

	/*
	 * Start method
	 */
	void Start() {
		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();
		player = GameObject.Find("Player").GetComponent<Player>();

		// Initial power demand of 100 W
		powerDemand = 100;
		
		UpdatePowerDisplay();
		uiLastUpdated = DateTime.UtcNow;
	}

	/*
	 * Update method
	 */
	void Update() {
		double delta = DateTime.UtcNow.Subtract(uiLastUpdated).TotalSeconds;

		if (delta >= 1) {
			UpdatePowerDisplay();
			uiLastUpdated = DateTime.UtcNow;
		}
		
		if (powerDemand == 0) {
			++quotasMet;
			powerDemand = Mathf.Floor(100 * Mathf.Pow(quotasMet, Mathf.Exp(1) / 1.3f));
			player.money += 10 * Mathf.Pow(quotasMet, Mathf.Exp(1) / 2f);
		}
	}

	/*
	 * Update the power supply and demand display
	 */
	private void UpdatePowerDisplay() {
		suppliers = GameObject.FindGameObjectsWithTag("Generator");

		powerSupply = 0;
		PowerSupplier psScript;
		foreach (GameObject supplier in suppliers) {
			psScript = supplier.GetComponent<PowerSupplier>();

			if (psScript.isGeneratorRunning) {
				powerSupply += psScript.currentlyProducing;
				powerDemand = Mathf.Max(0f, powerDemand - psScript.currentlyProducing);
				player.money += psScript.currentlyProducing * player.moneyMult;
			}
		}

		uiHandler.powerStatsText.text = $"Current demand: {powerDemand:n2} W\nSatisfaction: {powerSupply:n2} W";
	}

	/*
	 * Set the power generation multiplier for every generator of a given type
	 */
	public void SetPowerGenMultiplier(string type, float value) {
		suppliers = GameObject.FindGameObjectsWithTag("Generator");

		PowerSupplier psScript;
		foreach (GameObject supplier in suppliers) {
			if (supplier.name.Contains(type)) {
				psScript = supplier.GetComponent<PowerSupplier>();
				psScript.powerMultiplier = value;
			}
		}
	}
}
