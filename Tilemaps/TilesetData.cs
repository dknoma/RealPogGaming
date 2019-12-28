using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tilemaps {
    public class TilesetData : ScriptableObject {
        public List<Tile> tilePrefabs;
        public string tilemapName;

        public TilesetData() {
            tilePrefabs = new List<Tile>();
        }

        public Tile GetTile(int index) {
            return tilePrefabs[index];
        }
    }
}
