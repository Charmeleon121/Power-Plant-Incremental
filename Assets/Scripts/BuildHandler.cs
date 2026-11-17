using UnityEngine;

public class BuildHandler : MonoBehaviour {
	private GameObject manualGeneratorPrefab, manualGeneratorGhostPrefab, woodGeneratorPrefab, woodGeneratorGhostPrefab;

	void Start() {
		manualGeneratorPrefab = Resources.Load<GameObject>("Prefabs/Manual Generator");
		manualGeneratorGhostPrefab = Resources.Load<GameObject>("Prefabs/Manual Generator Ghost");
	}

	void Update() {
		/*
		 * TODO:
		 * 
		 * - Lock out the buy menu until the current ghost is placed
		 * - Ensure the current ghost remains locked to the player's cursor and the ground
		 * 
		 * - (In Player): if a ghost exists, and the player left clicks, place the correct
		 *					object where the ghost was, and remove the ghost
		 */
	}

	public void EnterBuildMode(string type) {
		switch (type) {
			case "Manual Generator":
				// Create the ghost in front of the player
				break;
		}
	}
}
