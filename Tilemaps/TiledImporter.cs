using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] private TextAsset json;
        [SerializeField] private string path;
        [SerializeField] private string thisPath;

        private TiledInfo tiledInfo;

        public List<TilesetData> tileSets;

        public void ProcessJSON() {
            Debug.LogFormat("I'm processing the thing!");
            
            GetAssetPath();
            LoadFromJson();
        }

        private void GetAssetPath() {
            string assetPath = AssetDatabase.GetAssetPath(json);
            
            path = Split(assetPath, "/(\\w)+\\.json")[0];
            thisPath = Split(path, "/")[2];
        }

        private static string[] Split(string input, string regex) {
            return Regex.Split(input, regex);
        }
        
        private void LoadFromJson() {
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

            AssetDatabase.CreateAsset(asset, string.Format("{0}/{1}.asset", path, filename));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        public void CreateTilePrefabs() {
            
        }
        
        public void BuildTileset() {
            if (tiledInfo != null) {
                Debug.LogFormat("I'm building the thing!");

                var tilesetsInfo = tiledInfo.tilesets;
                foreach (TiledInfo.Tileset tileset in tilesetsInfo) {
                    CreateAsset(tileset.source);
                }
            } else {
                Debug.Log("Tiled info does not seem to be processed. Please make sure to process the JSON file.");
            }
        }
    }
}
