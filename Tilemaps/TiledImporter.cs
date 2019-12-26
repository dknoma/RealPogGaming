using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] private TextAsset json;
        [SerializeField] private string path;
        [SerializeField] private string parentPath;
        [SerializeField] private string thisPath;

        private TiledInfo tiledInfo;

        [SerializeField] private List<TilesetData> tilesets = new List<TilesetData>();

        public string Json => json.name;

        public void ProcessJSON() {
            Debug.LogFormat("I'm processing the thing!");
            
            GetAssetPath();
            LoadFromJson();
        }

        private void GetAssetPath() {
            string assetPath = AssetDatabase.GetAssetPath(json);
            
            this.path = Split(assetPath, "/(\\w)+\\.json")[0];
            
            string[] parts = Split(path, "/");
            this.parentPath = $"{parts[0]}/{parts[1]}";
            this.thisPath = parts[2];
        }

        private static string[] Split(string input, string regex) {
            return Regex.Split(input, regex);
        }
        
        private void LoadFromJson() {
            JsonUtility.FromJsonOverwrite(json.text, tiledInfo);
            
            Debug.Log(tiledInfo);
        }

        /// <summary>
        /// Process spritesheets from their png files to create tile prefabs for future use.
        /// </summary>
        public void ProcessSpritesFromSheet() {
            ParseTsxFiles();
        }

        private void ParseTsxFiles() {
            var tilesetsInfo = tiledInfo.tilesets;
            int offset = 0;
            
            // Create assets for each tileset
            foreach (TiledInfo.Tileset tileset in tilesetsInfo) {
                XmlDocument xml = new XmlDocument();
                string tilesetSource = tileset.source;
                string tilesetName = tilesetSource.Split('.')[0];
                string file = FromAbsolutePath(tilesetSource);
                
                Debug.LogFormat("file [{0}]", file);
                xml.Load(file);
                
                // tag hierarchy: <tileset><image ...>; Attributes of the tag, get value by name.
                XmlNode node = xml.SelectSingleNode("tileset/image");
                string sheetPath = FromAbsolutePath(node.Attributes["source"].Value);

                Debug.LogFormat("sheetPath [{0}]", sheetPath);

                offset = ProcessSpritesFromSheet(tilesetName, sheetPath, offset);
            }
        }

        private string FromAbsolutePath(string file) {
            return $"{this.path}/{file}";
        }

        private int ProcessSpritesFromSheet(string tilesetName, string filename, int offset) {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(filename)
                                       .OfType<Sprite>()
                                       .ToArray();
            int newOffset = sprites.Length;
            
            TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();
            asset.offset = offset;
            
            foreach (Sprite sprite in sprites) {
                Debug.LogFormat("sprite [{0}]", sprite);
                CreateTilesetPrefabs(asset, sprite);
            }
            
            AssetDatabase.CreateAsset(asset, $"{parentPath}/{tilesetName}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            
            tilesets.Add(asset);
            return offset + newOffset;
        }
        
        private void CreateTilesetPrefabs(TilesetData asset, Sprite sprite) {
            string tilename = sprite.name;

            string assetPath = $"{path}/{tilename}.prefab";

            GameObject newTile = CreateTileObjects(sprite);
            Debug.LogFormat("NEW SPRITE [{0}]", newTile);
            
//            Object prefab = EditorUtility.CreateEmptyPrefab(string.Format("{0}/{1}.prefab", path, tilename));
            
            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newTile, assetPath, InteractionMode.UserAction);
            
            asset.tilePrefabs.Add(prefab);
            
//            DestroyImmediate(prefab);
            // TODO - add each tile to the correct tileset
        }

        private static GameObject CreateTileObjects(Sprite sprite) {
            GameObject newTile = new GameObject(sprite.name);
            newTile.AddComponent<SpriteRenderer>().sprite = sprite;

            return newTile;
        }

        private void CreateTilemapAsset(string filename) {
            TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();

            AssetDatabase.CreateAsset(asset, $"{parentPath}/{filename}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
        
        public void BuildTileset() {
            if (tiledInfo != null) {
                Debug.LogFormat("I'm building the thing!");

//                var tilesetsInfo = tiledInfo.tilesets;
//                foreach (TiledInfo.Tileset tileset in tilesetsInfo) {
//                    CreateTilemapAsset(tileset.source);
//                }
            } else {
                Debug.Log("Tiled info does not seem to be processed. Please make sure to process the JSON file.");
            }
        }
    }
}
