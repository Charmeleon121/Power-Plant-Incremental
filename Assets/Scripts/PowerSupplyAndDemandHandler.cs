using System;
using UnityEngine;

public class PowerSupplyAndDemandHandler : MonoBehaviour {
	// External scripts
	private Player player;

	// All things relating to power supply and demand
	public float powerSupply, powerDemand;
	private GameObject[] suppliers;

	// All things to do with the amount of money the player gets from generating
	public float moneyDelta;
	private DateTime moneyLastUpdated;

	/*
	 * Start method
	 */
	void Start() {
		player = GameObject.Find("Player").GetComponent<Player>();

		// Initial power demand of 100 W
		powerDemand = 100;

		UpdatePowerStats();

		moneyLastUpdated = DateTime.UtcNow;
	}

	/*
	 * Update method
	 */
	void Update() {
		if (player.lookAndMoveActive) {
			UpdatePowerStats();

			// Only update the money every second
			if (DateTime.UtcNow.Subtract(moneyLastUpdated).TotalSeconds >= 1) {
				CalcMoneyDelta();
				player.money += moneyDelta;
				moneyLastUpdated = DateTime.UtcNow;
			}
		}
	}

	/*
	 * Update the power supply and demand display
	 */
	private void UpdatePowerStats() {
		suppliers = GameObject.FindGameObjectsWithTag("Generator");

		powerSupply = 0;
		PowerSupplier psScript;
		foreach (GameObject supplier in suppliers) {
			psScript = supplier.GetComponent<PowerSupplier>();

			if (psScript.isGeneratorRunning && psScript.currentFuelLevel > 0) {
				powerSupply += psScript.currentlyProducing;
			}
		}
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

	/*
	 * Calculate the amount of money the player should get based on their generating
	 */
	private void CalcMoneyDelta() {
		moneyDelta = 10 * Mathf.Pow(powerSupply / powerDemand, 1.5f) * player.moneyMult;
	}
}
