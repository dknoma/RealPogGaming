using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTable : MonoBehaviour {
	
	// TODO: Store prefabs of enemies here
	// Different enemies in a level tier & area will determine which enemies can be instantiated in battle
	private enum WorldArea {
		MalaMeadows,
	}

	private const int MAX_LEVELS = 50;
	// key: WorldArea, value: List of enemies that can be instantiated in this area
	private Dictionary<WorldArea, List<GameObject>[]> enemyTable = new Dictionary<WorldArea, List<GameObject>[]>();

	private void OnEnable() {
		// Possibly read from json file
		if (enemyTable[WorldArea.MalaMeadows] == null) {
			foreach (WorldArea area in Enum.GetValues(typeof(WorldArea))) {
				Debug.Log(area);
				enemyTable[area] = new List<GameObject>[MAX_LEVELS];
			}
		}

//		enemyTable[WorldArea.MalaMeadows] = new List<GameObject>[MAX_LEVELS];
	}
}
