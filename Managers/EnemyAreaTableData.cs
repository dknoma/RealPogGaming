using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class EnemyAreaTableData {
    // Different enemies in a level tier & area will determine which enemies can be instantiated in battle
    // key: WorldArea, value: List of enemies that can be instantiated in this 
    public EnemyAreaTableEntry[] enemyAreaTableEntries;
}

[System.Serializable]
 public class EnemyAreaTableEntry {
//     public WorldArea area;
//     public EnemyTierEntry[] enemyTierListEntries;
 }
 
[System.Serializable]
public class EnemyTierEntry {
//    public List<string> enemies;
}