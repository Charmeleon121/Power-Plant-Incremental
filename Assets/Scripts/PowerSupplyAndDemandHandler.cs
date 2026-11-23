using System;
using UnityEngine;

public class PowerSupplyAndDemandHandler : MonoBehaviour {
	// External scripts
	private UIHandler uiHandler;
	private Player player;

	// All things relating to power supply and demand
	public float powerSupply, powerDemand;
	private GameObject[] suppliers;

	// UI update timer
	private DateTime uiLastUpdated;

	/*
	 * Start method
	 */
	void Start() {
		uiHandler = GetComponent<UIHandler>();
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
		if (player.lookAndMoveActive) {
			double delta = DateTime.UtcNow.Subtract(uiLastUpdated).TotalSeconds;

			if (delta >= 1) {
				UpdatePowerDisplay();
				uiLastUpdated = DateTime.UtcNow;
			}

			player.money += 10 * Mathf.Pow(powerSupply / powerDemand, Mathf.Exp(1) / 1.3f) * player.moneyMult;
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

			if (psScript.isGeneratorRunning && psScript.currentFuelLevel > 0) {
				powerSupply += psScript.currentlyProducing;
				player.money += psScript.currentlyProducing * player.moneyMult;
			}
		}

		uiHandler.powerStatsText.text = $"Satisfaction: {powerSupply:n2}/{powerDemand:n2} W\n({(powerSupply * 100f) / powerDemand:n2}%)";
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

	/*
	 * Modify the max delta time for updating the fuel for every generator of a given
	 * type - larger values mean longer running between fuel uses
	 */
	public void SetMaxFuelDelta(string type, int value) {
		PowerSupplier psScript;
		foreach (GameObject supplier in suppliers) {
			if (supplier.name.Contains(type)) {
				psScript = supplier.GetComponent<PowerSupplier>();
				psScript.fuelLastUpdatedMaxDelta = value;
			}
		}
	}
}
