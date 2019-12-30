using System;
using UnityEditor;
using UnityEngine;

namespace Editors {
	public class DruEditor : Editor {
	

		/// <summary>
		/// Button Methods
		/// </summary>

		/// <summary>
		/// Create horizontal button groups from tuples of (title, action)
		/// </summary>
		/// <param name="buttons"></param>
		protected static void ButtonGroup(params (string, Action)[] buttons) {
			var headerActionTuples = buttons;
			int len = headerActionTuples.Length;
			
			if(len == 1) {
				EditorGUILayout.BeginHorizontal();
				
				if (GUILayout.Button(headerActionTuples[0].Item1, EditorStyles.miniButton)) {
					headerActionTuples[0].Item2();
				}
				
				EditorGUILayout.EndHorizontal();
			} else if(len > 1) {
				EditorGUILayout.BeginHorizontal();
				
				(string, Action) firstTup = headerActionTuples[0];
				if (GUILayout.Button(firstTup.Item1, EditorStyles.miniButtonLeft)) {
					firstTup.Item2();
				}

				for(int i = 1; i < len - 1; i++) {
					(string, Action) tup = headerActionTuples[i];
					if (GUILayout.Button(tup.Item1, EditorStyles.miniButtonMid)) {
						tup.Item2();
					}
				}
				
				(string, Action) lastTup = headerActionTuples[len-1];
				if (GUILayout.Button(lastTup.Item1, EditorStyles.miniButtonRight)) {
					lastTup.Item2();
				}
				
				EditorGUILayout.EndHorizontal();
			}
		}

		protected static void LeftButton(string text, Action action) {
			if (GUILayout.Button(text, EditorStyles.miniButtonLeft)) {
				action();
			}
		}

		protected static void InnerButton(string text, Action action) {
			if (GUILayout.Button(text, EditorStyles.miniButtonMid)) {
				action();
			}
		}

		protected static void RightButton(string text, Action action) {
			if(GUILayout.Button(text, EditorStyles.miniButtonRight)) {
				action();
			}
		}

		/// <summary>
		/// Header methods
		/// </summary>
		/// <param name="style">GUIStyle</param>
		/// <param name="headers">Header fields</param>
		protected static void DrawLineAndHeader(GUIStyle style, params string[] headers) {
			DrawUILine(Color.grey);
			foreach(string header in headers) {
				EditorGUILayout.LabelField(header, style);
			}
		}
		
		/// <summary>
		/// Draw a line with the default grey color. Allows for changing optional parameters thickness and padding.
		/// </summary>
		/// <param name="thickness">Line thickness</param>
		/// <param name="padding">Line padding</param>
		protected static void DrawUILine(int thickness = 2, int padding = 10) {
			DrawUILine(Color.grey, thickness, padding);
		}

		protected static void DrawUILine(Color color, int thickness = 2, int padding = 10){
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
			r.height = thickness;
			// ReSharper disable once PossibleLossOfFraction
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}
	}
}
