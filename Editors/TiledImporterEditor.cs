using Tilemaps;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;

namespace Editors {
    [CustomEditor(typeof(TiledImporter))]
    public class TiledImporterEditor : DruEditor {
    
        private SerializedProperty overwriteTilesetAssets;
        private SerializedProperty overwriteLevelGrid;
        private SerializedProperty gridX;
        private SerializedProperty gridY;
        private SerializedProperty gridZ;

        private void OnEnable() {
            overwriteTilesetAssets = serializedObject.FindProperty("overwriteTilesetAssets");
            overwriteLevelGrid = serializedObject.FindProperty("overwriteLevelGrid");
            gridX = serializedObject.FindProperty("gridX");
            gridY = serializedObject.FindProperty("gridY");
            gridZ = serializedObject.FindProperty("gridZ");
        }

        public override void OnInspectorGUI() {
            // Allows for default fields to be drawn
            DrawDefaultInspector();
        
            TiledImporter importer = (TiledImporter) target;
            GUIStyle style = new GUIStyle {richText = true};
            
            LabelField("Cell Size");
            BeginHorizontal();
            float originalWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 8f;
            PropertyField(gridX, new GUIContent("x"));
            PropertyField(gridY, new GUIContent("y"));
            PropertyField(gridZ, new GUIContent("z"));
            EditorGUIUtility.labelWidth = originalWidth;
            EndHorizontal();
            
            DrawLineAndHeader(style, "<b>Overwrite Flags</b>");
            DrawUILine(1);
                
            PropertyField(overwriteLevelGrid, new GUIContent("Overwrite Grid"));
            PropertyField(overwriteTilesetAssets, new GUIContent("Overwrite Tiles"));
        
            DrawLineAndHeader(style, $"<b>Tiled Tilemap JSON Processing - {importer.Json}.json</b>");
        
            if (GUILayout.Button("Process JSON and Build")) {
                importer.ProcessJsonThenBuild();
            }
        
            DrawLineAndHeader(style, "<b>Individual Processing</b>");

            ButtonGroup(("JSON", importer.ProcessJSON), 
                        ("Tiles", importer.ProcessSpritesFromSheetIfNecessary),
                        ("Build", importer.BuildTileset));
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}
