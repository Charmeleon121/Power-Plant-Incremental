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
		
		uiLastUpdated = DateTime.UtcNow;
	}

	/*
	 * Update method
	 */
	void Update() {
		int delta = DateTime.UtcNow.Subtract(uiLastUpdated).Seconds;

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
		PowerSupplier powerSupplierScript;
		foreach (GameObject supplier in suppliers) {
			powerSupplierScript = supplier.GetComponent<PowerSupplier>();

			if (powerSupplierScript.isGeneratorRunning) {
				powerSupply += powerSupplierScript.currentlyProducing;
				powerDemand = Mathf.Max(0f, powerDemand - powerSupplierScript.currentlyProducing);
				player.money += powerSupplierScript.currentlyProducing * player.moneyMult;
			}
		}

		uiHandler.powerStatsText.text = $"Current demand: {powerDemand:n2} W\nSatisfaction: {powerSupply:n2} W";
	}
}
