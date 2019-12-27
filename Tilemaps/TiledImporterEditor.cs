using UnityEditor;
using UnityEngine;

namespace Tilemaps {
    [CustomEditor(typeof(TiledImporter))]
    public class TiledImporterEditor : Editor {
        public override void OnInspectorGUI() {
            // Allows for default fields to be drawn
            DrawDefaultInspector();
            
            TiledImporter importer = (TiledImporter) target;
            GUIStyle style = new GUIStyle {richText = true};

            DrawLineAndHeader($"<b>Tiled Tilemap JSON Processing - {importer.Json}.json</b>", style);
            
            if (GUILayout.Button("Process JSON")) {
                importer.ProcessJSON();
            }
            
            if (GUILayout.Button("Build Tileset")) {
                importer.BuildTileset();
            }
            
            DrawLineAndHeader("<b>Prefab and Asset Creation</b>", style);
            
            if (GUILayout.Button("Process Sprites From Spritesheets")) {
                importer.ProcessSpritesFromSheet();
            }
        }

        private static void DrawLineAndHeader(string header, GUIStyle style) {
            DrawUILine(Color.grey);
            EditorGUILayout.LabelField(header, style);
        }
        
        private static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
