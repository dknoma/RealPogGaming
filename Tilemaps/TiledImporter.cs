using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Tilemaps.TiledInfo;
using static Tilemaps.TiledInfo.Layer;
using static Tilemaps.TilemapConstants;
using static Tilemaps.TiledRenderOrder;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] 
        private TextAsset json;
        [SerializeField] 
        private GameObject levelGrid;
        
        [SerializeField] [DisableInspectorEdit] 
        private string path;

        [SerializeField] [DisableInspectorEdit] 
        private string thisPath;

        // Tiled
        private TiledInfo tiledInfo;
        
        [SerializeField] [DisableInspectorEdit] 
        private RenderOrder renderOrder;

        [SerializeField] 
        private List<TilesetData> tilesets = new List<TilesetData>();
        
        // Importer
        [SerializeField] [DisableInspectorEdit]
        private int[] tilesetOffsets;
        [SerializeField] [DisableInspectorEdit]
        private IDictionary<string, int> tilesetSourceOffsets = new Dictionary<string, int>();

        // Overwriting settings
        [HideInInspector]
        public bool overwriteTilesetAssets;
        [HideInInspector]
        public bool overwriteLevelGrid = true;
        
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
        /// Build tileset
        /// </summary>
        public void BuildTileset() {
            if (tiledInfo != null && levelGrid != null) {
                GameObject grid = GameObject.FindWithTag(ENVIRONMENT_GRID_TAG);
                if(grid != null && overwriteLevelGrid) {
#if DEBUG
                    Debug.Log("Grid exists, need to destroy before instantiating new one.");
#endif
                    DestroyImmediate(grid);
                    grid = Instantiate(levelGrid);
                } else if(grid != null && !overwriteLevelGrid) {
                    Debug.Log("Grid exists, do not overwrite grid.");
                } else if(grid == null) {
                    grid = Instantiate(levelGrid);
                }
                
                Debug.LogFormat("I'm building the thing!");
                var layers = tiledInfo.layers;
                foreach(Layer layer in layers) {
                    ProcessLayer(grid, layer);
                }
            } else {
                Debug.Log("Tiled info does not seem to be processed. Please make sure to process the JSON file.");
            }
        }

        private void ProcessLayer(GameObject grid, Layer layer) {
            string layerName = layer.name;
            int id = layer.id;
            int totalHeight = layer.height;
            int totalWidth = layer.width;
            int startX = layer.startx;
            int startY = layer.starty;
            int layerX = layer.x;
            int layerY = layer.y;

            Tilemap tilemap = NewTilemap(grid, layerName, layerX, layerY);
            
            // Get 
            TilesetData tilesetData = tilesets[id - 1];
            //      - data[index] = index of tile in data.tilePrefabs 
            var chunks = layer.chunks;
            foreach(Chunk chunk in chunks) {
                int[] data = chunk.data;
                int chunkStartX = chunk.x;
                int chunkStartY = chunk.y;
                int height = chunk.height;
                int width = chunk.width;
                int x = chunkStartX;
                int y;
                switch(renderOrder) {
                    case RenderOrder.RIGHT_DOWN:
                    case RenderOrder.LEFT_DOWN:
                        y = -chunkStartY;
                        break;
                    case RenderOrder.RIGHT_UP:
                    case RenderOrder.LEFT_UP:
                        y = chunkStartY;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Debug.Log($"CHUNK START [({x}, {y})]");

                foreach(int tileIndex in data) {
                    if(Mathf.Abs(x) - chunkStartX > width) {
                        Debug.LogError($"x={x} is larger than the expected width {width}");
//                        DestroyImmediate(grid);
//                        return;
                    }
                    if(Mathf.Abs(y) - chunkStartY > height) {
                        Debug.LogError($"y={y} is larger than the expected height {height}");
//                        DestroyImmediate(grid);
//                        return;
                    }
                    
                    if(tileIndex > 0) {
                        int offset = tilesetSourceOffsets[layer.name];
                        int index = tileIndex - offset;
                        Debug.Log($"CHUNK [{tileIndex}, {index}, {offset}, ({x}, {y})]");
                            
                        Tile tile = ScriptableObject.CreateInstance<Tile>();
                        tile.sprite = tilesetData.tilePrefabs[index].GetComponent<SpriteRenderer>().sprite;

                        Vector3Int pos = new Vector3Int(startX + x, startY + y, 0);
                        tilemap.SetTile(pos, tile);
                    }

                    (x, y) = AdvanceOffsets(x, y, width, chunkStartX);
                }
            }
            tilemap.RefreshAllTiles();
        }

        private Tilemap NewTilemap(GameObject grid, string layerName, int x, int y) {
            GameObject layer = new GameObject(layerName);
            Tilemap tilemap = layer.AddComponent<Tilemap>();
            layer.AddComponent<TilemapRenderer>();
            layer.transform.position = new Vector3(x, y, 0);
            layer.transform.parent = grid.transform;

            return tilemap;
        }

        private (int, int) AdvanceOffsets(int initialX, int initialY, int width, int chunkStartX) {
            int x = initialX;
            int y = initialY;
            
            switch(renderOrder) {
                case RenderOrder.RIGHT_DOWN:
//                    Debug.Log($"before x={x}");
                    x++;
                    int check = x - chunkStartX;
//                    Debug.Log($"check={check}");
                    if(check >= width) {
                        x = chunkStartX;
                        y--;
                    }
//                    Debug.Log($"after  x={x}");
                    break;
                case RenderOrder.RIGHT_UP:
                    x++;
                    if(x - chunkStartX >= width) {
                        x = chunkStartX;
                        y++;
                    }
                    break;
                case RenderOrder.LEFT_DOWN:
                    x--;
                    if(chunkStartX - x >= width) {
                        x = chunkStartX;
                        y--;
                    }
                    break;
                case RenderOrder.LEFT_UP:
                    x--;
                    if(chunkStartX - x >= width) {
                        x = chunkStartX;
                        y++;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (x, y);
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
            foreach (Tileset tileset in tilesetsInfo) {
                XmlDocument xml = new XmlDocument();
                string tilesetSource = tileset.source;
                string tilesetName = tilesetSource.Split('.')[0];
                string tilesetAssetFilePath = GetAssetPath(TILESETS_PATH, tilesetName);
                string layerName = tiledInfo.layers[i].name;
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

                    TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();
                    asset.name = layerName;
                    ProcessSpritesFromSheet(tilesetName, sheetPath, asset);
                }
                this.tilesetOffsets[i] = firstgid;
                this.tilesetSourceOffsets.Add(layerName, firstgid);
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

        private void ProcessSpritesFromSheet(string tilesetName, string filename, TilesetData asset) {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(filename)
                                       .OfType<Sprite>()
                                       .ToArray();
            
            foreach (Sprite sprite in sprites) {
#if DEBUG
                Debug.LogFormat("sprite [{0}]", sprite);
#endif
                CreateTilesetPrefabs(asset, sprite);
            }
            
            AssetDatabase.CreateAsset(asset, $"{TILESETS_PATH}/{tilesetName}.asset");
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
    }
}
