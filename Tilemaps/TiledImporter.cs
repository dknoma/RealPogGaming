using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

using static Tilemaps.TiledRenderOrder;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] 
        private TextAsset json;
        
        [SerializeField] [DisableInspectorEdit] 
        private string path;

        [DisableInspectorEdit]
        private const string PARENT_PATH = "Assets/Tilesets";

        [SerializeField] [DisableInspectorEdit] 
        private string thisPath;

        private TiledInfo tiledInfo;
        
        [SerializeField] [DisableInspectorEdit] 
        private RenderOrder renderOrder;

        [SerializeField] 
        private List<TilesetData> tilesets = new List<TilesetData>();
        [SerializeField] [DisableInspectorEdit]
        private int[] tilesetOffsets;
        [SerializeField] [DisableInspectorEdit]
        private IDictionary<string, int> tilesetSourceOffsets = new Dictionary<string, int>();

        [HideInInspector]
        public bool overwriteTilesetAssets;
        
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
            this.thisPath = parts[2];
        }

        private static string[] Split(string input, string regex) {
            return Regex.Split(input, regex);
        }
        
        private void LoadFromJson() {
            JsonUtility.FromJsonOverwrite(json.text, tiledInfo);
            this.renderOrder = GetRenderOrder(tiledInfo.renderorder);
            
            Debug.Log(tiledInfo);
        }

        /// <summary>
        /// Process spritesheets from their png files to create tile prefabs for future use.
        /// </summary>
        public void ProcessSpritesFromSheetIfNecessary() {
            ParseTsxFiles();
        }

        private void ParseTsxFiles() {
            var tilesetsInfo = tiledInfo.tilesets;
            
            this.tilesets = new List<TilesetData>();
            this.tilesetOffsets = new int[tilesetsInfo.Length];
            this.tilesetSourceOffsets = new Dictionary<string, int>();
            
            // Create assets for each tileset
            int i = 0;
            foreach (TiledInfo.Tileset tileset in tilesetsInfo) {
                XmlDocument xml = new XmlDocument();
                string tilesetSource = tileset.source;
                string tilesetName = tilesetSource.Split('.')[0];
                string tilesetAssetFilePath = GetAssetPath(PARENT_PATH, tilesetName);
                int firstgid = tileset.firstgid;
                
                bool tilesetAssetExists = TilesetDataExists(tilesetAssetFilePath);
                Debug.LogFormat($"{tilesetAssetFilePath} exists = {tilesetAssetExists}");
                
                // Add existing assets if they don't exist and don't need to be overwritten
                if(tilesetAssetExists && !overwriteTilesetAssets) {
                    AddExistingTilesetDataAsset(tilesetAssetFilePath);
                } else {
                    string tilesetSourceFilePath = FromAbsolutePath(tilesetSource);
#if DEBUG
                    Debug.LogFormat("file [{0}]", tilesetSourceFilePath);
#endif
                    xml.Load(tilesetSourceFilePath);

                    // tag hierarchy: <tileset><image ...>; Attributes of the tag, get value by name.
                    XmlNode imageNode = xml.SelectSingleNode("tileset/image");
                    string sheetPath = FromAbsolutePath(imageNode.Attributes["source"].Value);

#if DEBUG
                    Debug.LogFormat("sheetPath [{0}]", sheetPath);
#endif

                    ProcessSpritesFromSheet(tilesetName, sheetPath);
                }
                this.tilesetOffsets[i] = firstgid;
                this.tilesetSourceOffsets[tilesetName] = firstgid;
                i++;
            }
        }
        
        private static bool TilesetDataExists(string tilesetAssetFilePath) {
            return File.Exists(tilesetAssetFilePath);
        }

        private void AddExistingTilesetDataAsset(string filePath) {
            TilesetData tilesetAsset = AssetDatabase.LoadAssetAtPath<TilesetData>(filePath);
    
            tilesets.Add(tilesetAsset);
        }

        private static string GetAssetPath(string path, string sourcename) {
            return $"{path}/{sourcename}.asset";
        }

        private string FromAbsolutePath(string file) {
            return $"{this.path}/{file}";
        }

        private void ProcessSpritesFromSheet(string tilesetName, string filename) {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(filename)
                                       .OfType<Sprite>()
                                       .ToArray();
            
            TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();
            
            foreach (Sprite sprite in sprites) {
#if DEBUG
                Debug.LogFormat("sprite [{0}]", sprite);
#endif
                CreateTilesetPrefabs(asset, sprite);
            }
            
            AssetDatabase.CreateAsset(asset, $"{PARENT_PATH}/{tilesetName}.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            
            tilesets.Add(asset);
        }
        
        private void CreateTilesetPrefabs(TilesetData asset, Sprite sprite) {
            string tilename = sprite.name;

            string assetPath = $"{path}/{tilename}.prefab";

            GameObject newTile = CreateTileObjects(sprite);
            
#if DEBUG
            Debug.LogFormat("NEW SPRITE [{0}]", newTile);
#endif
            
            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(newTile, assetPath, InteractionMode.UserAction);
            
            asset.tilePrefabs.Add(prefab);
            
            DestroyImmediate(newTile);
            // TODO - add each tile to the correct tileset
        }

        private static GameObject CreateTileObjects(Sprite sprite) {
            GameObject newTile = new GameObject(sprite.name);
            newTile.AddComponent<SpriteRenderer>().sprite = sprite;

            return newTile;
        }
        
        /// <summary>
        /// Build tileset
        /// </summary>
        public void BuildTileset() {
            if (tiledInfo != null) {
                Debug.LogFormat("I'm building the thing!");
            } else {
                Debug.Log("Tiled info does not seem to be processed. Please make sure to process the JSON file.");
            }
        }
    }
}
