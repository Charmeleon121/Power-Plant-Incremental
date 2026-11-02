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
	private int fuelUpdateTimer = 0;
	private int maxFuelUpdateTimer;
	public DateTime lastClicked;

	// Set the values as soon as the object is created
	void Awake() {
		isGeneratorRunning = false;
		currentFuelLevel = 0;
		currentlyProducing = 0;

		if (gameObject.name.Contains("Manual")) {
			maxFuelLevel = 1;
			basePower = 1;
			maxFuelUpdateTimer = 240;
		}

		powerMultiplier = 1;

		lastClicked = DateTime.UtcNow;
	}

	/*
	 * Update the fuel level and current activity state of the generator
	 */
	void Update() {
		if (currentFuelLevel == 0) {
			Deactivate();
			fuelUpdateTimer = 0;
		}

		if (fuelUpdateTimer == maxFuelUpdateTimer) {
			if (isGeneratorRunning) {
				currentFuelLevel = Mathf.Max(0f, currentFuelLevel - 1);
			}

			fuelUpdateTimer = 0;
		} else {
			++fuelUpdateTimer;
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
