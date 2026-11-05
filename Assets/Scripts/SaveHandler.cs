using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using UnityEngine;

public class SaveHandler : MonoBehaviour {
	// External scripts
	private Player playerScript;
	private PowerSupplyAndDemandHandler psad;
	private UIHandler uiHandler;
	
	// Important GameObjects
	private GameObject player;
	private GameObject[] generators;
	
	// Prefabs for instantiation on load
	private GameObject manualGenPrefab, woodGenPrefab;
	
	void Start() {
		player = GameObject.Find("Player");
		playerScript = player.GetComponent<Player>();
		
		psad = GameObject.Find("EventSystem").GetComponent<PowerSupplyAndDemandHandler>();
		
		uiHandler = GameObject.Find("EventSystem").GetComponent<UIHandler>();
		
		generators = GameObject.FindGameObjectsWithTag("Generator");
	}
	
	public async void SaveGame() {
		List<string> names = new();
		List<Vector3> positions = new();
		List<Quaternion> rotations = new();
		List<float> fuelLevels = new();
		List<int> fuelUseTimes = new();
		List<float> mults = new();
		List<bool> states = new();
		List<bool> purchased = new();
		
		PowerSupplier psScript;
		foreach (GameObject generator in generators) {
			psScript = generator.GetComponent<PowerSupplier>();
			
			names.Add(generator.name);
			positions.Add(generator.transform.position);
			rotations.Add(generator.transform.rotation);
			fuelLevels.Add(psScript.currentFuelLevel);
			fuelUseTimes.Add(psScript.fuelLastUpdatedMaxDelta);
			mults.Add(psScript.powerMultiplier);
			states.Add(psScript.isGeneratorRunning);
		}
		
		for (int i = 0; i < uiHandler.unlockPanelContent.childCount; ++i) {
			UnlockButton uBtn = uiHandler.unlockPanelContent.GetChild(i).GetComponent<UnlockButton>();
			purchased.Add(uBtn.purchased);
		}
		
		SaveData saveData = new SaveData {
			playerPosition = new float[] { 
				player.transform.position.x,
				player.transform.position.y,
				player.transform.position.z
			},
			playerMoney = playerScript.money,
			playerMoneyMult = playerScript.moneyMult,
			targetProduction = psad.powerDemand,
			generatorNames = names,
			generatorPositions = positions,
			generatorRotations = rotations,
			generatorFuelLevels = fuelLevels,
			generatorFuelUseTimes = fuelUseTimes,
			generatorMults = mults,
			generatorStates = states,
			upgradesPurchased = purchased
		};
		
		await using FileStream createStream = File.Create("save.json");
		await JsonSerializer.SerializeAsync(createStream, saveData);
		
		uiHandler.pauseMenu.transform.GetChild(1).gameObject.SetActive(true);
	}
	
	public async void LoadGame() {
		manualGenPrefab = Resources.Load<GameObject>("Prefabs/Manual Generator");
		woodGenPrefab = Resources.Load<GameObject>("Prefabs/Wood-burning Generator");
		
		using FileStream openStream = File.OpenRead("save.json");
		SaveData saveData = await JsonSerializer.DeserializeAsync<SaveData>(openStream);
		
		Vector3 playerPos = new(saveData.playerPosition[0], saveData.playerPosition[1], saveData.playerPosition[2]);
		player.transform.position = playerPos;
		playerScript.money = saveData.playerMoney;
		playerScript.moneyMult = saveData.playerMoneyMult;
		
		psad.powerDemand = saveData.targetProduction;
		
		GameObject generatorInst;
		PowerSupplier genScript;
		for (int i = 0; i < saveData.generatorNames.Count; ++i) {
			if (saveData.generatorNames[i].Contains("Manual")) {
				generatorInst = Instantiate(manualGenPrefab, saveData.generatorPositions[i], saveData.generatorRotations[i]);
				genScript = generatorInst.GetComponent<PowerSupplier>();
				
				genScript.currentFuelLevel = saveData.generatorFuelLevels[i];
				genScript.fuelLastUpdatedMaxDelta = saveData.generatorFuelUseTimes[i];
				genScript.powerMultiplier = saveData.generatorMults[i];
				genScript.isGeneratorRunning = saveData.generatorStates[i];
			}
		}
		
		for (int i = 0; i < uiHandler.unlockPanelContent.childCount; ++i) {
			UnlockButton uBtn = uiHandler.unlockPanelContent.GetChild(i).GetComponent<UnlockButton>();
			uBtn.purchased = saveData.upgradesPurchased[i];
		}
	}
}

public class SaveData {
	// All of the data which would actually be saved
	public float[] playerPosition {get; set;}
	public float playerMoney {get; set;}
	public float playerMoneyMult {get; set;}
	public float targetProduction {get; set;}
	public List<string> generatorNames {get; set;}
	public List<Vector3> generatorPositions {get; set;}
	public List<Quaternion> generatorRotations {get; set;}
	public List<float> generatorFuelLevels {get; set;}
	public List<int> generatorFuelUseTimes {get; set;}
	public List<float> generatorMults {get; set;}
	public List<bool> generatorStates {get; set;}
	public List<bool> upgradesPurchased {get; set;}
}