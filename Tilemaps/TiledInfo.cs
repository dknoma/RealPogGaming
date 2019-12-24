using System.Collections.Generic;
using System.Text;

/// <summary>
/// Unity class used for holding serialized Tiled JSON tilemap info.
/// </summary>
[System.Serializable]
public class TiledInfo {
    public int compressionlevel;
    public EditorSettings editorsettings;
    public int height;
    public bool infinite;
    public Layer[] layers;
    public int nextlayerid;
    public int nextobjectid;
    public string oritentation;
    public string renderorder;
    public string tiledversion;
    public int tileheight;
    public Tileset[] tilesets;
    public int tilewidth;
    public string type;
    public double version;
    public int width;
    
    [System.Serializable]
    public class EditorSettings {
        public Export export;
        
        [System.Serializable]
        public class Export {
            public string format;
            public string target;
        }
    }

    [System.Serializable]
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
        
        [System.Serializable]
        public class Chunk {
            public int[] data;
            public int height;
            public int width;
            public int x;
            public int y;
        }
    }

    [System.Serializable]
    public class Tileset {
        public int firstgid;
        public string source;
    }
}