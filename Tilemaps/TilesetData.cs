using System.Collections.Generic;
using UnityEngine;

namespace Tilemaps {
    public class TilesetData : ScriptableObject {
        [DisableInspectorEdit] public int firstgid;
        public List<GameObject> tilePrefabs;

        public TilesetData() {
            tilePrefabs = new List<GameObject>();
        }

        public GameObject GetTile(int tiledIndex) {
            return tilePrefabs[tiledIndex - firstgid];
        }
    }
}
