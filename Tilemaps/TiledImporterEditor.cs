using UnityEditor;
using UnityEngine;

namespace Tilemaps {
    [CustomEditor(typeof(TiledImporter))]
    public class TiledImporterEditor : Editor {
        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            TiledImporter importer = (TiledImporter) target;
            if (GUILayout.Button("Process JSON")) {
                importer.ProcessJSON();
            }
            
            if (GUILayout.Button("Build Tileset")) {
                importer.BuildTileset();
            }
        }
    }
}
