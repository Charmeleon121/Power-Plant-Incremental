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
	public int fuelLastUpdatedMaxDelta;
	public DateTime lastClicked;

	// Set the values as soon as the object is created
	void Awake() {
		isGeneratorRunning = false;
		currentFuelLevel = 0;
		currentlyProducing = 0;

		if (gameObject.name.Contains("Manual")) {
			maxFuelLevel = 1;
			basePower = 1;
			fuelLastUpdatedMaxDelta = 1000;
		} else if (gameObject.name.Contains("Wood")) {
			maxFuelLevel = 5;
			basePower = 1.5f;
			fuelLastUpdatedMaxDelta = 1750;
		} else if (gameObject.name.Contains("Charcoal")) {
			maxFuelLevel = 7;
			basePower = 1.85f;
			fuelLastUpdatedMaxDelta = 2000;
		} else if (gameObject.name.Contains("Coal")) {
			maxFuelLevel = 7;
			basePower = 2.25f;
			fuelLastUpdatedMaxDelta = 2000;
		}

		powerMultiplier = 1;

		fuelLastUpdated = DateTime.UtcNow;
		lastClicked = DateTime.UtcNow;
	}

	/*
	 * Update the fuel level and current activity state of the generator
	 */
	void Update() {
		if (isGeneratorRunning) {
			currentlyProducing = basePower * powerMultiplier;

			double delta = DateTime.UtcNow.Subtract(fuelLastUpdated).TotalMilliseconds;
			if (delta >= fuelLastUpdatedMaxDelta) {
				currentFuelLevel = Mathf.Max(0f, currentFuelLevel - 0.1f);
				fuelLastUpdated = DateTime.UtcNow;
			}
		} else {
			currentlyProducing = 0f;
		}

		if (currentFuelLevel == 0) {
			Deactivate();
		}
	}

	/*
	 * Activate the generator
	 */
	public void Activate() {
		if (currentFuelLevel > 0) {
			isGeneratorRunning = true;
		} else {
			if (gameObject.name.Contains("Manual")) {
				isGeneratorRunning = true;
				currentFuelLevel = 1;
			}
		}
	}

	/*
	 * Deactivate the generator
	 */
	public void Deactivate() {
		isGeneratorRunning = false;
		currentlyProducing = 0;
	}
}
