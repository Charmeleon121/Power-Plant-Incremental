using UnityEngine;

public class BuildHandler : MonoBehaviour {
	private Player playerScript;
	
	private GameObject manualGeneratorPrefab, manualGeneratorGhostPrefab, woodGeneratorPrefab, woodGeneratorGhostPrefab;
	private GameObject activeGhost;
	
	public bool snapEnabled = false;

	void Start() {
		playerScript = GameObject.Find("Player").GetComponent<Player>();
		
		manualGeneratorPrefab = Resources.Load<GameObject>("Prefabs/Manual Generator");
		manualGeneratorGhostPrefab = Resources.Load<GameObject>("Prefabs/Manual Generator Ghost");
		
		activeGhost = null;
	}

	/*
	 * Every frame, if a ghost object currently exists, keep it locked to where the player is looking
	 */
	void Update() {
		if (activeGhost != null && playerScript.lookAndMoveActive) {
			Vector3 playerLookPos = playerScript.GetLookCoords();

			if (activeGhost.name.Contains("Manual")) {
				playerLookPos.y = 1.198f;
			}
			
			if (snapEnabled) {
				playerLookPos.x = Mathf.RoundToInt(playerLookPos.x);
				playerLookPos.z = Mathf.RoundToInt(playerLookPos.z);
			}
			
			activeGhost.transform.position = playerLookPos;
		}
	}

	/*
	 * Enter build mode by spawning in a ghost version of the bought object to allow the player
	 * to place it
	 */
	public void EnterBuildMode(string type) {
		playerScript.inBuildMode = true;
		
		Vector3 playerLookPos = playerScript.GetLookCoords();
		
		switch (type) {
			case "Manual Generator":
				playerLookPos.y = 1.198f; // TODO: Adjust based on final model size
				activeGhost = Instantiate(manualGeneratorGhostPrefab, playerLookPos, Quaternion.Euler(0f, 0f, 0f));
				activeGhost.tag = "Ghost";
				break;
		}
	}
	
	/*
	 * Replace an existing ghost object with its constructed counterpart
	 */
	public void Build() {
		if (activeGhost.name.Contains("Manual")) {
			Instantiate(manualGeneratorPrefab, activeGhost.transform.position, activeGhost.transform.rotation);
		}
		
		playerScript.inBuildMode = false;
		Destroy(activeGhost);
		activeGhost = null;
	}
	
	/*
	 * Rotate the ghost object
	 */
	public void Rotate(int amount) {
		if (activeGhost != null) {
			float xRot = activeGhost.transform.rotation.eulerAngles.x;
			float yRot = activeGhost.transform.rotation.eulerAngles.y;
			float zRot = activeGhost.transform.rotation.eulerAngles.z;
			
			activeGhost.transform.rotation = Quaternion.Euler(
				xRot,
				yRot + amount,
				zRot
			);
		}
	}
	
	/*
	 * Get the current active ghost object
	 */
	public GameObject GetGhostObject() {
		return activeGhost;
	}
}
