using System;

[Serializable]
public class EnemyTableData {
	// Different enemies in a level tier & area will determine which enemies can be instantiated in battle
	// key: WorldArea, value: List of enemies that can be instantiated in this 
	public EnemyTableEntry[] enemyTableEntries;
}

[Serializable]
public class EnemyTableEntry {
//	public List<WorldArea> areas;
	public int tier;
	public string enemyPrefab;
}