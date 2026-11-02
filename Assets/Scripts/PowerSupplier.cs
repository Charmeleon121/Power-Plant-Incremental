using System;
using UnityEngine;

public class PowerSupplier : MonoBehaviour {
	// Whether the generator is currently active
	public bool isGeneratorRunning;

	// Current percentage of fuel the generator has, and the maximum
	public float currentFuelLevel, maxFuelLevel;

	// How much power the generator is producing in watts, and its base power
	public float currentlyProducing, basePower;

	// Variables relating to the upgrading of the generator
	public float powerMultiplier;

	// Timer values
	private DateTime fuelLastUpdated;
	private int fuelLastUpdatedMaxDelta;
	public DateTime lastClicked;

	// Set the values as soon as the object is created
	void Awake() {
		isGeneratorRunning = false;
		currentFuelLevel = 0;
		currentlyProducing = 0;

		if (gameObject.name.Contains("Manual")) {
			maxFuelLevel = 1;
			basePower = 1;
			fuelLastUpdatedMaxDelta = 1;
		}

		powerMultiplier = 1;

		fuelLastUpdated = DateTime.UtcNow;
		lastClicked = DateTime.UtcNow;
	}

	/*
	 * Update the fuel level and current activity state of the generator
	 */
	void Update() {
		if (currentFuelLevel == 0) {
			Deactivate();
		}

		int delta = DateTime.UtcNow.Subtract(fuelLastUpdated).Seconds;
		if (delta >= fuelLastUpdatedMaxDelta) {
			if (isGeneratorRunning) {
				currentFuelLevel = Mathf.Max(0f, currentFuelLevel - 1);
			}

			fuelLastUpdated = DateTime.UtcNow;
		}
	}

	// Activate the generator
	public void Activate() {
		if (currentFuelLevel > 0) {
			isGeneratorRunning = true;
			currentlyProducing = basePower * powerMultiplier;
		} else {
			if (gameObject.name.Contains("Manual")) {
				isGeneratorRunning = true;
				currentFuelLevel = 1;
				currentlyProducing = basePower * powerMultiplier;
			}
		}
	}

	// Deactivate the generator
	public void Deactivate() {
		isGeneratorRunning = false;
		currentlyProducing = 0;
	}
}
