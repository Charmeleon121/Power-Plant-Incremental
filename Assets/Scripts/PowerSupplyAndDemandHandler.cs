using UnityEngine;

public class PowerSupplyAndDemandHandler : MonoBehaviour {
	// External scripts
	private UIHandler uiHandler;

	// All things relating to power supply and demand
	public float powerSupply, powerDemand;
	private GameObject[] suppliers;

	// UI update timer
	private int updateTimer;
	private int maxUpdateTimer = 30;

	/*
	 * Start method
	 */
	void Start() {
		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();

		// Initial power demand of 100 W
		powerDemand = 100;
	}

	/*
	 * Update method
	 */
	void Update() {
		if (updateTimer == maxUpdateTimer) {
			UpdatePowerDisplay();
			updateTimer = 0;
		} else {
			++updateTimer;
		}

		/*
		 * TODO: Implement system where, upon meeting quota, some amount of money is calculated and 
		 * given to the player. The next quota should also be calculated, starting slightly above the
		 * previous and getting exponentially more to encourage purchasing new generators/upgrades.
		 * 
		 * On a related note, implement:
		 * - Ability to place new generators/objects
		 * - Upgrade tree/shop
		 */
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
			}
		}

		if (powerDemand == 0) {
			uiHandler.powerStatsText.text = $"Current demand: {powerDemand:n2} W\nSatisfaction: {powerSupply:n2} W";
		} else {
			uiHandler.powerStatsText.text = $"Current demand: {powerDemand:n2} W\nSatisfaction: {powerSupply:n2} W";
		}
	}
}
