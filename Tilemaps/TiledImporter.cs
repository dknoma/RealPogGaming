using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] private TextAsset json;

//        private ScriptableObject tileset;

        public List<TilesetData> tileSets;

        public void ProcessJSON() {
            Debug.LogFormat("I'm doing the thing!");
            
            LoadFromJson();
        }
        
        public void LoadFromJson() {
            TiledInfo tiledInfo = new TiledInfo();
            JsonUtility.FromJsonOverwrite(json.text, tiledInfo);
            
            Debug.Log(tiledInfo);
//            if(tileset) {
//                DestroyImmediate(tileset);
//            }
//            tileset = ScriptableObject.CreateInstance<TilesetData>();
//            tileset.hideFlags = HideFlags.HideAndDontSave;
        }

        private void CreateAsset(string filename) {
            TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();

            AssetDatabase.CreateAsset(asset, string.Format("Assets/Tilesets/Testing/{0}.asset", filename));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }
}
