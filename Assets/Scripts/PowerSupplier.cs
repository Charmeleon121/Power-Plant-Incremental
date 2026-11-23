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

		switch (name) {
			case "Manual Generator":
				maxFuelLevel = 1;
				basePower = 1;
				fuelLastUpdatedMaxDelta = 1000;
				break;
			case "Wood-burning Generator":
				maxFuelLevel = 5;
				basePower = 1.5f;
				fuelLastUpdatedMaxDelta = 1750;
				break;
			case "Charcoal-burning Generator":
				maxFuelLevel = 7;
				basePower = 1.85f;
				fuelLastUpdatedMaxDelta = 2000;
				break;
			case "Coal-burning Generator":
				maxFuelLevel = 7;
				basePower = 2.25f;
				fuelLastUpdatedMaxDelta = 2000;
				break;
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
	 * Handle any moving parts or other physics-related calculations
	 */
	void FixedUpdate() {
		Transform movingPart;
		if (isGeneratorRunning) {
			Vector3 currentRotation;

			switch (name) {
				case "Manual Generator":
					movingPart = transform.GetChild(1);
					currentRotation = GetMovingPartRotation(movingPart);

					// Base speed on the fuel ratio, so it slows down over time (as if only generating while moving)
					movingPart.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z + GetFuelRatio());
					break;
			}
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

	/*
	 * Get the current fuel ratio of the generator
	 */
	private float GetFuelRatio() {
		return currentFuelLevel / maxFuelLevel;
	}

	/*
	 * Get the current rotation of a moving part in Euler angles
	 */
	private Vector3 GetMovingPartRotation(Transform part) {
		float initRotX = part.localRotation.eulerAngles.x;
		float initRotY = part.localRotation.eulerAngles.y;
		float initRotZ = part.localRotation.eulerAngles.z;

		return new(initRotX, initRotY, initRotZ);
	}
}
