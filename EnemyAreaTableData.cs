using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class EnemyAreaTableData {
    // Different enemies in a level tier & area will determine which enemies can be instantiated in battle
    // key: WorldArea, value: List of enemies that can be instantiated in this 
    public EnemyAreaTableEntry[] enemyAreaTableEntries;

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("{\"areaTableEntries\":[");
        int tableEntryCount = enemyAreaTableEntries != null ? enemyAreaTableEntries.Length : 0;
        for(int i = 0; i < tableEntryCount; i++) {
            sb.Append("{\"area\":");
            sb.Append((int) enemyAreaTableEntries[i].area);
            sb.Append(",\"enemyTiers\":[");
            EnemyTierEntry[] tierEntries = enemyAreaTableEntries[i].enemyTierListEntries;
            int tierCount = tierEntries.Length;
            for (int j = 0; j < tierCount; j++) {
                // If not enemies on tier, continue
                if (tierEntries[j] == null) continue;
                sb.Append("{");
                sb.Append(string.Format("\"tier\":{0},\"enemies\":[", j));
                // Else, add this list of enemy names to the json
                List<string> enemyList = tierEntries[j].enemies;
                int enemyCount = enemyList.Count;
                for (int k = 0; k < enemyCount; k++) {
                    sb.Append(string.Format("\"{0}\"", enemyList[k]));
                    if (k < enemyCount - 1) {
                        sb.Append(",");
                    }
                }
                sb.Append("]},");
            }
            sb.Remove(sb.ToString().Length - 1, 1);
            sb.Append("]}");
            if (i < tableEntryCount - 1) {
                sb.Append(",");
            }
        }
        sb.Append("]}");
        return sb.ToString();
    }
}

[System.Serializable]
 public class EnemyAreaTableEntry {
     public WorldArea area;
     public EnemyTierEntry[] enemyTierListEntries;
 }
 
[System.Serializable]
public class EnemyTierEntry {
    public List<string> enemies;
}