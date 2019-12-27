using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tilemaps {
    /// <summary>
    /// Unity class used for holding serialized Tiled JSON tilemap info.
    /// </summary>
    [Serializable]
    public class TiledInfo {
        public int compressionlevel;
        public EditorSettings editorsettings;
        public int height;
        public bool infinite;
        public Layer[] layers;
        public int nextlayerid;
        public int nextobjectid;
        public string orientation;
        public string renderorder;
        public string tiledversion;
        public int tileheight;
        public Tileset[] tilesets;
        public int tilewidth;
        public string type;
        public double version;
        public int width;
        
        /// <summary>
        /// Utility method to get the right String  representation of these objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string GetString(object obj) {
            var fieldValues = obj.GetType()
                .GetFields()
                .Select(field => field.GetValue(obj))
                .ToList();
            
            var fieldNames = obj.GetType().GetFields()
                .Select(field => field.Name)
                .ToList();
            
            StringBuilder builder = new StringBuilder();
            
            for(int i = 0; i < fieldValues.Count; i++) {
                object value = fieldValues[i];
                string name = fieldNames[i];

                string valueString;
                if (value.GetType().IsArray) {
                    StringBuilder sb = new StringBuilder();
                    
                    IEnumerable values = value as IEnumerable;
                    if (values != null) {
                        foreach (object v in values) {
                            sb.Append(string.Format("{0}, ", v));
                        }
                    }

                    sb.Remove(sb.Length - 2, 2);
                    valueString = sb.ToString();
                } else {
                    valueString = value.ToString();
                }
                builder.Append(string.Format("{0}={1} ", name, valueString));
            }
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public override string ToString() {
            return string.Format("INFO [{0}]", GetString(this));
        }
        
        /// <summary>
        /// Subclasses
        /// </summary>
        [Serializable]
        public class EditorSettings {
            public Export export;
        
            public override string ToString() {
                return string.Format("EditorSettings [{0}]", GetString(this));
            }
            
            [Serializable]
            public class Export {
                public string format;
                public string target;
                
                public override string ToString() {
                    return string.Format("Export [{0}]", GetString(this));
                }
            }
        }

        [Serializable]
        public class Layer {
            public Chunk[] chunks;
            public int height;
            public int id;
            public string name;
            public int opacity;
            public int startx;
            public int starty;
            public string type;
            public bool visible;
            public int width;
            public int x;
            public int y;
        
            public override string ToString() {
                return string.Format("Layer [{0}]", GetString(this));
            }
            
            [Serializable]
            public class Chunk {
                public int[] data;
                public int height;
                public int width;
                public int x;
                public int y;
        
                public override string ToString() {
                    return string.Format("Chunk [{0}]", GetString(this));
                }
            }
        }

        [Serializable]
        public class Tileset {
            public int firstgid;
            public string source;
            
            public override string ToString() {
                return string.Format("Tileset [{0}]", GetString(this));
            }
        }
    }
}