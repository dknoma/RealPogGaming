using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class EnemyTableData {
	// Different enemies in a level tier & area will determine which enemies can be instantiated in battle
	// key: WorldArea, value: List of enemies that can be instantiated in this 
	public EnemyTableEntry[] enemyTableEntries;

	public override string ToString() {
		StringBuilder sb = new StringBuilder();
		sb.Append("{\"enemyTableEntries\":[");
		int tableEntryCount = enemyTableEntries != null ? enemyTableEntries.Length : 0;
		for (int i = 0; i < tableEntryCount; i++) {
			sb.Append("{\"areas\":[");
			for (int j = 0; j < enemyTableEntries[i].areas.Count; j++) {
				sb.Append(string.Format("\"{0}\"", enemyTableEntries[i].areas[j]));
				if (j < enemyTableEntries[i].areas.Count - 1) {
					sb.Append(",");
				}
			}
			sb.Append("],\"tier\":");
			sb.Append(enemyTableEntries[i].tier);
			sb.Append(",\"enemyPrefab\":\"");
			sb.Append(enemyTableEntries[i].enemyPrefab);
			sb.Append("\"}");
			if (i < tableEntryCount - 1) {
				sb.Append(",");
			}
		}
		sb.Append("]}");
		return sb.ToString();
	}
}

[System.Serializable]
public class EnemyTableEntry {
	public List<WorldArea> areas;
	public int tier;
	public string enemyPrefab;
}