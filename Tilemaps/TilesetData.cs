using System.Collections.Generic;
using UnityEngine;

namespace Tilemaps {
    public class TilesetData : ScriptableObject {
        public int offset;
        public List<GameObject> tilePrefabs;

        private readonly int tiledOffset;

        public TilesetData() {
            tiledOffset = offset + 1;
            tilePrefabs = new List<GameObject>();
        }

        public GameObject GetTile(int tiledIndex) {
            return tilePrefabs[tiledIndex - tiledOffset];
        }
    }
}
