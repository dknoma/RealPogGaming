#undef DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Tilemaps.TiledInfo;
using static Tilemaps.TiledInfo.Layer;
using static Tilemaps.TilemapConstants;
using static Tilemaps.TiledRenderOrder;
using Object = UnityEngine.Object;

namespace Tilemaps {
    public class TiledImporter : MonoBehaviour {
        [SerializeField] 
        private TextAsset json;
        [SerializeField] 
        private GameObject levelGrid;
        
        [SerializeField] [DisableInspectorEdit] 
        private string path;

        [SerializeField] [DisableInspectorEdit] 
        private string tilesetFolder;

        // Tiled
        private TiledInfo tiledInfo;
        
        [SerializeField] [DisableInspectorEdit] 
        private RenderOrder renderOrder;

        [SerializeField] 
        private List<TilesetData> tilesets = new List<TilesetData>();
        
        // Importer
        [SerializeField] [DisableInspectorEdit]
        private IDictionary<string, int> tilesetSourceOffsets = new Dictionary<string, int>();

        // Overwriting settings
        [HideInInspector]
        public bool overwriteTilesetAssets;
        [HideInInspector]
        public bool overwriteLevelGrid = true;
        
        public string Json => json.name;

        /// <summary>
        /// Processes the tiled .json file, creates tiles from a spritesheet if necessary, then builds the tilemap.
        /// </summary>
        public void ProcessJsonThenBuild() {
            ProcessJSON();
            ProcessSpritesFromSheetIfNecessary();
            BuildTileset();
        }
        
        /// <summary>
        /// Individual processing methods
        /// </summary>
        public void ProcessJSON() {
            GetAssetPath();
            LoadFromJson();
        }

        private void GetAssetPath() {
            string assetPath = AssetDatabase.GetAssetPath(json);
            
            this.path = Split(assetPath, "/(\\w)+\\.json")[0];
            
            string[] parts = Split(path, "/");
            this.tilesetFolder = parts[2];
        }

        private static string[] Split(string input, string regex) {
            return Regex.Split(input, regex);
        }
        
        private void LoadFromJson() {
            if(json != null) {
                JsonUtility.FromJsonOverwrite(json.text, tiledInfo);
                this.renderOrder = GetRenderOrder(tiledInfo.renderorder);

                Debug.Log(tiledInfo);
            } else {
                Debug.LogError("Something went wrong. JSON file may be invalid");
            }
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

        /// <summary>
        /// Process a Layer from the whole tilemap
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="layer"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
#if DEBUG
                Debug.Log($"CHUNK START [({x}, {y})]");
#endif
                foreach(int tileIndex in data) {
                    if(Mathf.Abs(x) - chunkStartX > width) {
                        Debug.LogError($"x={x} is larger than the expected width {width}");
                        DestroyImmediate(grid);
                        return;
                    }
                    if(Mathf.Abs(y) - chunkStartY > height) {
                        Debug.LogError($"y={y} is larger than the expected height {height}");
                        DestroyImmediate(grid);
                        return;
                    }
                    
                    if(tileIndex > 0) {
                        int offset = tilesetSourceOffsets[layer.name];
                        int index = tileIndex - offset;
#if DEBUG
                        Debug.Log($"CHUNK [{tileIndex}, {index}, {offset}, ({x}, {y})]");
#endif
                        Tile tile = tilesetData.tilePrefabs[index];
                        Vector3Int pos = new Vector3Int(startX + x, startY + y, 0);
                        tilemap.SetTile(pos, tile);
                    }

                    (x, y) = AdvanceOffsets(x, y, width, chunkStartX);
                }
            }
        }

        private static Tilemap NewTilemap(GameObject grid, string layerName, int x, int y) {
            GameObject layer = new GameObject(layerName);
            layer.transform.position = new Vector3(x, y, 0);
            layer.transform.parent = grid.transform;
            Tilemap tilemap = layer.AddComponent<Tilemap>();
            TilemapRenderer renderer = layer.AddComponent<TilemapRenderer>();
            renderer.sortOrder = TilemapRenderer.SortOrder.TopRight;

            return tilemap;
        }

        private (int, int) AdvanceOffsets(int initialX, int initialY, int width, int chunkStartX) {
            int x = initialX;
            int y = initialY;
            
            switch(renderOrder) {
                case RenderOrder.RIGHT_DOWN:
                    x++;
                    int check = x - chunkStartX;
                    if(check >= width) {
                        x = chunkStartX;
                        y--;
                    }
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

        private static TsxInfo ParseXml(string tilesetSourcePath) {
#if DEBUG
            Debug.Log(tilesetSourcePath);
#endif
            XmlDocument doc = new XmlDocument();
            
            doc.Load(tilesetSourcePath);
            
            XDocument xDocument = XDocument.Parse(doc.OuterXml);
            StringBuilder builder = new StringBuilder();
            JsonSerializer.Create().Serialize(new CustomXmlJsonWriter(new StringWriter(builder)), xDocument);
            string jsonText = builder.ToString();
            TsxInfo tsxInfo = JsonUtility.FromJson<TsxInfo>(jsonText);
                
#if DEBUG
            Debug.Log(tsxInfo);
#endif
            return tsxInfo;
        }

        /// <summary>
        /// Parse Tiled .tsx files to get the correct values from the fields and attributes.
        /// </summary>
        private void ParseTsxFiles() {
            this.tilesets = new List<TilesetData>();
            this.tilesetSourceOffsets = new Dictionary<string, int>();
            var tilesetsInfo = tiledInfo.tilesets;
            
            // Create assets for each tileset
            int i = 0;
            foreach (Tileset tileset in tilesetsInfo) {
                string tilesetSource = tileset.source;
                TsxInfo tsxInfo = ParseXml(FromAbsolutePath(tilesetSource));
                string tilesetName = tsxInfo.tileset.name;
                string tilesetAssetFilePath = GetAssetPath(TILESETS_PATH, tilesetName);
                string layerName = tiledInfo.layers[i].name;
                
                bool tilesetAssetExists = TilesetDataExists(tilesetAssetFilePath);
#if DEBUG
                Debug.LogFormat($"{tilesetAssetFilePath} exists = {tilesetAssetExists}");
#endif       
                // Add existing assets if they don't exist and don't need to be overwritten
                if(tilesetAssetExists && !overwriteTilesetAssets) {
                    AddExistingTilesetDataAsset(tilesetAssetFilePath);
                } else {
                    string sheetPath = FromAbsolutePath(tsxInfo.tileset.image.source);
#if DEBUG
                    Debug.LogFormat("sheetPath [{0}]", sheetPath);
#endif
                    TilesetData asset = ScriptableObject.CreateInstance<TilesetData>();
                    asset.name = layerName;
                    ProcessSpritesFromSheet(tilesetName, sheetPath, asset);
                }
                this.tilesetSourceOffsets.Add(layerName, tileset.firstgid);
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

        /// <summary>
        /// Process sprites from a .png spritesheet. Spritesheet isn't automatically split and must be done
        /// manually beforehand. 
        /// </summary>
        /// <param name="tilesetName"></param>
        /// <param name="filename"></param>
        /// <param name="asset"></param>
        private void ProcessSpritesFromSheet(string tilesetName, string filename, TilesetData asset) {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(filename)
                                       .OfType<Sprite>();
            
            foreach (Sprite sprite in sprites) {
#if DEBUG
                Debug.LogFormat("sprite [{0}]", sprite);
#endif
                CreateTilesetAssets(asset, sprite);
            }

            SaveAssetToDatabase(asset, TILESETS_PATH, tilesetName);
            
            tilesets.Add(asset);
        }

        private static void SaveAssetToDatabase(Object asset, string path, string filename) {
            AssetDatabase.CreateAsset(asset, GetAssetPath(path, filename));
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
        }
        
        /// <summary>
        /// Create Tile assets for a given tileset
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="sprite"></param>
        private void CreateTilesetAssets(TilesetData asset, Sprite sprite) {
            string tilename = sprite.name;
            
            Tile tile = CreateTileAssets(sprite);
            SaveAssetToDatabase(tile, path, tilename);
#if DEBUG
            Debug.LogFormat("NEW SPRITE [{0}]", newTile);
#endif
            asset.tilePrefabs.Add(tile);
        }
        
        private static Tile CreateTileAssets(Sprite sprite) {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;

            return tile;
        }
    }
}
