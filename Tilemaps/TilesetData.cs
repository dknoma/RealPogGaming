using System.Collections.Generic;
using UnityEngine;

namespace Tilemaps {
    public class TilesetData : ScriptableObject {
        public List<GameObject> tilePrefabs;

        public TilesetData() {
            tilePrefabs = new List<GameObject>();
        }

        public GameObject GetTile(int index) {
            return tilePrefabs[index];
        }
    }
}
