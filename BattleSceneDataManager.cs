using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[Serializable]
public enum WorldArea {
	CelestialCanopy,
	MalaMeadows,
}

public class BattleSceneDataManager : MonoBehaviour {

	public static BattleSceneDataManager bsdm;

	private const string ENEMY_TABLE_FILE = "enemies/battle/enemyTable.json";
	private const int MAX_LEVELS = 100;
//	List<GameObject>[] enemiesByTier = new List<GameObject>[MAX_LEVELS];
	public Dictionary<string, EnemyTableEntry> enemyTable = new Dictionary<string, EnemyTableEntry>();
	public Dictionary<WorldArea, List<string>[]> areaTable = new Dictionary<WorldArea, List<string>[]>();

	private void OnEnable() {
		if (bsdm == null) {
			bsdm = this;
		} else if (bsdm != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		LoadEnemyTableFromFile(ENEMY_TABLE_FILE);
	}

	private void Update() {
		if (Input.GetKeyDown("q")) {
			SaveEnemyTableToFile();
		}
		if (Input.GetKeyDown("p")) {
			TestAddToTable();
		}
	}

	public void SaveEnemyTableToFile() {
		string enemyTableToJson = EnemyDictionaryToJson(enemyTable);
		Debug.LogFormat("to json:\t{0}", enemyTableToJson);
		string filePath = Path.Combine (Application.streamingAssetsPath, ENEMY_TABLE_FILE);
		File.WriteAllText(filePath, enemyTableToJson);
	}

	private void TestAddToTable() {
//		foreach (WorldArea area in Enum.GetValues(typeof(WorldArea))) {
////			Debug.LogFormat("Area: {0}",area);
//			areaTable[area] = InitList();
//			Debug.LogFormat("\tidk {0}",areaTable[area]);
//		}
//		AddEnemyToAreaTable(WorldArea.CelestialCanopy, 1, "NEW ENEMY");
//		AddEnemyToAreaTable(WorldArea.CelestialCanopy, 1, "chep");
//		AddEnemyToAreaTable(WorldArea.CelestialCanopy, 10, "NEW STRONGER ENEMY");
//		AddEnemyToAreaTable(WorldArea.MalaMeadows, 2, "malasada");
//		foreach(var pair in areaTable) {
////			Debug.LogFormat("\tkey: {0}, value: {1}", pair.Key, pair.Value);
//			// i = tier
//			for (int i = 0; i < pair.Value.Length; i++) {
////				Debug.LogFormat("\tidk {0}, {1}", pair.Value[i], areaTable[WorldArea.CelestialCanopy][i]);
//				if (pair.Value[i] == null || pair.Value[i].Count == 0) continue;
//				for (int j = 0; j < pair.Value[i].Count; j++) {
//					Debug.LogFormat("\tarea: {0}, tier: {1}, enemy: {2}", pair.Key, i, pair.Value[i][j]);
//				}
//			}
//		}
//
//		EnemyTableData data = new EnemyTableData {enemyTableEntries = new EnemyTableEntry[1]};
//		data.enemyTableEntries[0] = new EnemyTableEntry {areas = new List<WorldArea>()};
//		data.enemyTableEntries[0].areas.Add(WorldArea.CelestialCanopy);
//		data.enemyTableEntries[0].areas.Add(WorldArea.MalaMeadows);
//		data.enemyTableEntries[0].tier = 1;
//		data.enemyTableEntries[0].enemyPrefab = "star";
////		Debug.LogFormat("DICTIONARY: {0}", areaTable);
////		string enemyTableJson = JsonUtility.ToJson(areaTable);
//		string enemyTableJson = JsonUtility.ToJson(data);
//		Debug.LogFormat("\tDictionary json: {0}",enemyTableJson);
//		EnemyTableData enemyTableData = JsonUtility.FromJson<EnemyTableData>(enemyTableJson);
//		Debug.LogFormat("\tEnemyTableData: {0}", enemyTableData);
//		Dictionary<string, EnemyTableEntry> enemies = new Dictionary<string, EnemyTableEntry>();
//		EnemyTableEntry ent = new EnemyTableEntry{areas = new List<WorldArea>()};
//		ent.areas.Add(WorldArea.CelestialCanopy);
//		ent.areas.Add(WorldArea.MalaMeadows);
//		ent.tier = 1;
//		enemyTable.Add("star", ent);
//		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 1, "star");
//		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "star");
//		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "bobby");
//		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "nobu");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 1, "star");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 1, "Ardf");
		AddEnemyToEnemyTable(WorldArea.CelestialCanopy, 31, "HNGGG");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "star");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "bobby");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 15, "Yorru");
		AddEnemyToEnemyTable(WorldArea.MalaMeadows, 1, "nobu");
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
//				sb.Append(string.Format("\"{0}\"", pair.Value.areas[j]));
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
	
	public static string AreaDictionaryToJson(Dictionary<WorldArea, List<string>[]> dictionary) {
		StringBuilder sb = new StringBuilder();
		sb.Append("{\"areaTableEntries\":[");
		int i = 0;
		foreach(var pair in dictionary) {
			sb.Append("{\"area\":");
			sb.Append((int) pair.Key);
			sb.Append(",\"enemyTiers\":[");
			int tierCount = pair.Value.Length;
			for (int j = 0; j < tierCount; j++) {
				// If not enemies on tier, continue
				if (pair.Value[j] == null) continue;
				sb.Append("{");
				sb.Append(string.Format("\"tier\":{0},\"enemies\":[", j));
				// Else, add this list of enemy names to the json
				int enemyCount = pair.Value[j].Count;
				for (int k = 0; k < enemyCount; k++) {
					sb.Append(string.Format("\"{0}\"",pair.Value[j][k]));
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

	private List<string>[] InitList() {
		List<string>[] list = new List<string>[MAX_LEVELS];
		for (int i = 0; i < MAX_LEVELS; i++) {
			list[0] = new List<string>();
		}
		return list;
	}
	
	public void LoadEnemyTableFromFile(string fileName) {
		string filePath = Path.Combine (Application.streamingAssetsPath, fileName);
		if (File.Exists (filePath)) {
			string jsonData = File.ReadAllText (filePath);
			EnemyTableData enemyTableData = JsonUtility.FromJson<EnemyTableData>(jsonData);
//			Debug.LogFormat("EnemyTableData: {0}",enemyTableData);
			// Instantiate new lists for each world area
			foreach (WorldArea area in Enum.GetValues(typeof(WorldArea))) {
//				Debug.LogFormat("Area: {0}",area);
				areaTable[area] = new List<string>[MAX_LEVELS];
			}
			// For each table entry, add the enemy to their designated area(s).
			for (int i = 0; i < enemyTableData.enemyTableEntries.Length; i++) {
				EnemyTableEntry entry = enemyTableData.enemyTableEntries[i];
				enemyTable.Add(entry.enemyPrefab, entry);
				// For each area the enemy is a part of, add the gameobject to that areas list
				for (int j = 0; j < entry.areas.Count; j++) {
//					Debug.LogFormat("{0} is part of area {1}", entry.enemyPrefab, entry.areas[j]);
					AddEnemyToAreaTable(entry.areas[j], entry.tier, entry.enemyPrefab);
				}   
			}
			Debug.LogFormat("area table: {0}, enemy table: {1}", AreaDictionaryToJson(areaTable), EnemyDictionaryToJson(enemyTable));
			
//			LocalizationData loadedData = JsonUtility.FromJson<LocalizationData> (jsonData);
//
//			for (int i = 0; i < loadedData.items.Length; i++) 
//			{
//				localizedText.Add (loadedData.items [i].key, loadedData.items [i].value);   
//			}
//
//			Debug.Log ("Data loaded, dictionary contains: " + localizedText.Count + " entries");
			
		} else {
			Debug.LogFormat("File {0} was not found.", fileName);
		}
	}

	public void AddEnemyToAreaTable(WorldArea area, int tier, string prefabName) {
		if (!areaTable.ContainsKey(area) || areaTable[area] == null) {
			areaTable[area] = new List<string>[MAX_LEVELS];
		}
		if (areaTable[area][tier] == null) {
			Debug.LogFormat("Initializing lists for {0}", area);
			areaTable[area][tier] = new List<string>();
		}
		areaTable[area][tier].Add(prefabName);
		Debug.LogFormat("added {0}", areaTable[area][tier][areaTable[area][tier].Count-1]);
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
		return Resources.Load<GameObject>("enemies/prefabs/" +enemyPrefabName);
	}
}
