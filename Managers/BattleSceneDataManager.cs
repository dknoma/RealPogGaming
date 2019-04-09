using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum WorldArea {
	TestHub,
	CelestialCanopy,
	PaltryPlains,
	MalaMeadows,
}

public class BattleSceneDataManager : MonoBehaviour {

	public static BattleSceneDataManager bsdm;

	private const string AREA_TABLE_FILE = "enemies/battle/areaTable.json";
	private const string ENEMY_TABLE_FILE = "enemies/battle/enemyTable.json";
	private const string AREA_TABLE_FILE_COPY = "enemies/battle/areaTable_Copy.json";
	private const string ENEMY_TABLE_FILE_COPY = "enemies/battle/enemyTable_Copy.json";
	private const int MAX_LEVELS = 100;
	public Dictionary<string, EnemyTableEntry> enemyTable = new Dictionary<string, EnemyTableEntry>();
	public Dictionary<WorldArea,EnemyAreaTableEntry> areaTable = new Dictionary<WorldArea, EnemyAreaTableEntry>();

	private void OnEnable() {
		if (bsdm == null) {
			bsdm = this;
		} else if (bsdm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		LoadEnemyTableFromFile(ENEMY_TABLE_FILE);
		LoadAreaTableFromFile(AREA_TABLE_FILE);
	}

	private void Update() {
		if (Input.GetKeyDown("q")) {
			SaveEnemyTableToFile();
			SaveAreaTableToFile();
		}
		if (Input.GetKeyDown("p")) {
			TestAddToTable();
		}
	}

	public void SaveAreaTableToFile() {
		string areaTableToJson = AreaDictionaryToJson(areaTable);
		Debug.LogFormat("to json:\t{0}", areaTableToJson);
		string filePath = Path.Combine (Application.streamingAssetsPath, AREA_TABLE_FILE);
		File.WriteAllText(filePath, areaTableToJson);
	}

	public void SaveEnemyTableToFile() {
		string enemyTableToJson = EnemyDictionaryToJson(enemyTable);
		Debug.LogFormat("to json:\t{0}", enemyTableToJson);
		string filePath = Path.Combine (Application.streamingAssetsPath, ENEMY_TABLE_FILE);
		File.WriteAllText(filePath, enemyTableToJson);
	}
	
	public void SaveAreaTableCopyToFile() {
		string areaTableToJson = AreaDictionaryToJson(areaTable);
		Debug.LogFormat("to json:\t{0}", areaTableToJson);
		string filePath = Path.Combine (Application.streamingAssetsPath, AREA_TABLE_FILE_COPY);
		File.WriteAllText(filePath, areaTableToJson);
	}
	
	public void SaveEnemyTableCopyToFile() {
		string enemyTableToJson = EnemyDictionaryToJson(enemyTable);
		Debug.LogFormat("to json:\t{0}", enemyTableToJson);
		string filePath = Path.Combine (Application.streamingAssetsPath, ENEMY_TABLE_FILE_COPY);
		File.WriteAllText(filePath, enemyTableToJson);
	}
	

	private void TestAddToTable() {
		AddEnemyToEnemyTable(WorldArea.TestHub, 1, "Enemy1");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 1, "star");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 1, "Ardf");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 31, "HNGGG");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "star");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "bobby");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 15, "Yorru");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "nobu");
		
		AddEnemyToAreaTable(WorldArea.CelestialCanopy, 1, "star");
		AddEnemyToAreaTable(WorldArea.CelestialCanopy, 31, "HNGGG");
		AddEnemyToAreaTable(WorldArea.TestHub, 1, "Enemy1");
		AddEnemyToAreaTable(WorldArea.MalaMeadows, 15, "Yorru");
	}
	
	/// <summary>
	/// Serializes the enemy table Dictionary to JSON format.
	/// Format:
	/// 	{
	/// 		"enemyTableEntries":[
	/// 			{
	/// 				"areas":[
	/// 					0,
	/// 					1
	/// 				],
	/// 				"tier":1,
	/// 				"enemyPrefab":"prefab1"
	/// 			},
	/// 			{
	/// 				"areas":[
	/// 					0
	/// 				],
	/// 				"tier":1,
	/// 				"enemyPrefab":"prefab2"
	/// 			},
	/// 		]
	/// 	}
	/// </summary>
	/// <param name="dictionary"></param>
	/// <returns></returns>
	public static string EnemyDictionaryToJson(Dictionary<string, EnemyTableEntry> dictionary) {
		StringBuilder sb = new StringBuilder();
		sb.Append("{\"enemyTableEntries\":[");
		int i = 0;
		foreach(var pair in dictionary) {
			sb.Append("{\"areas\":[");
			int areaCount = pair.Value.areas.Count;
			for (int j = 0; j < areaCount; j++) {
				sb.Append((int)pair.Value.areas[j]);
				if (j < areaCount - 1) {
					sb.Append(",");
				}
			}
			sb.Append("],\"tier\":");
			sb.Append(pair.Value.tier);
			sb.Append(",\"enemyPrefab\":\"");
			sb.Append(pair.Key);
			sb.Append("\"}");
			if (i < dictionary.Count - 1) {
				sb.Append(",");
			}
			i++;
		}
		sb.Append("]}");
		return sb.ToString();
	}
	
	/// <summary>
	/// Serializes the area table Dictionary to JSON format.
	/// Format:
	/// 	{
	///			"areaTableEntries": [
	/// 			{
	///					"area": 0,
	///					"enemies": [
	/// 					{
	///							"tier": 1,
	///							"enemies": ["star", "Ardf"]
	///						}, {
	///							"tier": 31,
	///							"enemies": ["HNGGG"]
	///						}
	/// 				]
	///				}, {
	///					"area": 1,
	///					"enemies": [
	/// 					{
	///							"tier": 1,
	///							"enemies": ["star", "bobby", "nobu"]
	///						}, {
	///							"tier": 15,
	///							"enemies": ["Yorru"]
	///						}
	/// 				]
	///				}
	/// 		]
	///		}
	/// </summary>
	/// <param name="dictionary"></param>
	/// <returns></returns>
	public static string AreaDictionaryToJson(Dictionary<WorldArea, EnemyAreaTableEntry> dictionary) {
		StringBuilder sb = new StringBuilder();
		sb.Append("{\"areaTableEntries\":[");
		int i = 0;
		foreach(var pair in dictionary) {
			sb.Append("{\"area\":");
			sb.Append((int) pair.Key);
			sb.Append(",\"enemyTiers\":[");
			EnemyTierEntry[] tierEntries = pair.Value.enemyTierListEntries;
			int tierCount = tierEntries.Length;
			for (int j = 0; j < tierCount; j++) {
				// If not enemies on tier, continue
				if (tierEntries[j] == null) continue;
				sb.Append("{");
				sb.Append(string.Format("\"tier\":{0},\"enemies\":[", j));
				// Else, add this list of enemy names to the json
				List<string> enemies = tierEntries[j].enemies;
				int enemyCount = enemies.Count;
				for (int k = 0; k < enemyCount; k++) {
					sb.Append(string.Format("\"{0}\"",enemies[k]));
					if (k < enemyCount - 1) {
						sb.Append(",");
					}
				}
				sb.Append("]},");
			}
			sb.Remove(sb.ToString().Length - 1, 1);
			sb.Append("]}");
			if (i < dictionary.Count - 1) {
				sb.Append(",");
			}
			i++;
		}
		sb.Append("]}");
		return sb.ToString();
	}
	
	public void LoadAreaTableFromFile(string fileName) {
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
		if (File.Exists (filePath)) {
			string jsonData = File.ReadAllText (filePath);
			EnemyAreaTableData enemyAreaTableData = JsonToEnemyAreaTableData(jsonData);
//			Debug.LogFormat("enemyAreaTableData: {0}", enemyAreaTableData);
			// For each table entry, add the enemy to their designated area(s).
			for (int i = 0; i < enemyAreaTableData.enemyAreaTableEntries.Length; i++) {
				EnemyAreaTableEntry entry = enemyAreaTableData.enemyAreaTableEntries[i];
				EnemyTierEntry[] tierEntries = entry.enemyTierListEntries;
				for (int j = 0; j < tierEntries.Length; j++) {
					if(tierEntries[j] == null) continue;
					for (int k = 0; k < tierEntries[j].enemies.Count; k++) {
						AddEnemyToAreaTable(entry.area, j, entry.enemyTierListEntries[j].enemies[k]);
					}
				} 
			}
			Debug.LogFormat("loaded area table: {0}", AreaDictionaryToJson(areaTable));
		} else {
			Debug.LogFormat("File {0} was not found.", fileName);
		}
	}
	
	public void LoadEnemyTableFromFile(string fileName) {
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
		if (File.Exists (filePath)) {
			string jsonData = File.ReadAllText (filePath);
			EnemyTableData enemyTableData = JsonUtility.FromJson<EnemyTableData>(jsonData);
//			Debug.LogFormat("EnemyTableData: {0}",enemyTableData);
			// For each table entry, add the enemy to their designated area(s).
			for (int i = 0; i < enemyTableData.enemyTableEntries.Length; i++) {
				EnemyTableEntry entry = enemyTableData.enemyTableEntries[i];
				enemyTable.Add(entry.enemyPrefab, entry);
			}
			Debug.LogFormat("loaded enemy table: {0}", EnemyDictionaryToJson(enemyTable));
		} else {
			Debug.LogFormat("File {0} was not found.", fileName);
		}
	}
	
	public void LoadEnemyAndAreaTableFromFile(string fileName) {
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
		if (File.Exists (filePath)) {
			string jsonData = File.ReadAllText (filePath);
			EnemyTableData enemyTableData = JsonUtility.FromJson<EnemyTableData>(jsonData);
//			Debug.LogFormat("EnemyTableData: {0}",enemyTableData);
			// For each table entry, add the enemy to their designated area(s).
			for (int i = 0; i < enemyTableData.enemyTableEntries.Length; i++) {
				EnemyTableEntry entry = enemyTableData.enemyTableEntries[i];
				enemyTable.Add(entry.enemyPrefab, entry);
				// For each area the enemy is a part of, add the gameobject to that areas list
				for (int j = 0; j < entry.areas.Count; j++) {
					Debug.LogFormat("{0} is part of area {1}", entry.enemyPrefab, entry.areas[j]);
					AddEnemyToAreaTable(entry.areas[j], entry.tier, entry.enemyPrefab);
				}   
			}
			Debug.LogFormat("loaded enemy table: {0}", EnemyDictionaryToJson(enemyTable));
		} else {
			Debug.LogFormat("File {0} was not found.", fileName);
		}
	}

	private EnemyAreaTableData JsonToEnemyAreaTableData(string jsonData) {
		JSONNode node = JSON.Parse(jsonData);
		JSONArray areas = node["areaTableEntries"].AsArray;
		EnemyAreaTableData result = new EnemyAreaTableData{enemyAreaTableEntries = new EnemyAreaTableEntry[areas.Count]};
		// For each area in the json file...
		foreach (var area in areas) {
			JSONNode areaInfo = area.Value; 						
			WorldArea areaName = (WorldArea)areaInfo["area"].AsInt; // Get area
			JSONArray tierList = areaInfo["enemyTiers"].AsArray;	// Get the areas tier list
			EnemyAreaTableEntry enemyAreaTableEntry 
				= new EnemyAreaTableEntry {area = areaName,enemyTierListEntries = new EnemyTierEntry[MAX_LEVELS]};
			// For each table entry
			foreach (var tierListInfo in tierList) {
				JSONNode tierInfo = tierListInfo.Value;
				int tier = tierInfo["tier"];						// Get the tier
				JSONArray enemies = tierInfo["enemies"].AsArray;	// Get the enemy list
				enemyAreaTableEntry.enemyTierListEntries[tier] = new EnemyTierEntry{enemies = new List<string>()};
				foreach (var enemy in enemies) {
					enemyAreaTableEntry.enemyTierListEntries[tier].enemies.Add(enemy.Value);	// Add enemies to the list
				}
			}
			result.enemyAreaTableEntries[(int) areaName] = enemyAreaTableEntry;	// Add the table entry to the area
		}
		return result;
	}

	public void AddEnemyToAreaTable(WorldArea area, int tier, string prefabName) {
		if (!areaTable.ContainsKey(area) || areaTable[area] == null) {
			areaTable.Add(area, new EnemyAreaTableEntry {
				                                            area = area, 
				                                            enemyTierListEntries = new EnemyTierEntry[MAX_LEVELS]
			                                            });
			areaTable[area].enemyTierListEntries[tier] = new EnemyTierEntry {enemies = new List<string>()};
//			Debug.Log("Made new table entry");
		} else if (areaTable[area].enemyTierListEntries[tier] == null) {
			areaTable[area].enemyTierListEntries[tier] = new EnemyTierEntry {enemies = new List<string>()};
		} else if (areaTable[area].enemyTierListEntries[tier].enemies == null) {
//			Debug.LogFormat("Initializing lists for {0}", area);
			areaTable[area].enemyTierListEntries[tier].enemies = new List<string>();
		}
		areaTable[area].enemyTierListEntries[tier].enemies.Add(prefabName);
//		Debug.LogFormat("added {0}", areaTable[area].enemyTierListEntries[tier].enemies[areaTable[area].enemyTierListEntries[tier].enemies.Count-1]);
	}
	
	public void AddEnemyToEnemyTable(WorldArea area, int tier, string prefabName) {
		if (!enemyTable.ContainsKey(prefabName) || enemyTable[prefabName] == null) {
			enemyTable.Add(prefabName, new EnemyTableEntry{areas = new List<WorldArea>()});
		}
		enemyTable[prefabName].areas.Add(area);
		enemyTable[prefabName].tier = tier;
		enemyTable[prefabName].enemyPrefab = prefabName;
	}

	public GameObject GetEnemyPrefab(string enemyPrefabName) { 
		return Resources.Load<GameObject>(string.Format("enemies/prefabs/{0}", enemyPrefabName));
	}
}
