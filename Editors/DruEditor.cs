using System;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorStyles;
using static UnityEngine.GUILayout;

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
				
				(string title, Action action) = headerActionTuples[0];
				MiniButton(title, action);
				
				EditorGUILayout.EndHorizontal();
			} else if(len > 1) {
				EditorGUILayout.BeginHorizontal();
				
				(string title, Action action) = headerActionTuples[0];
				LeftButton(title, action);

				for(int i = 1; i < len - 1; i++) {
					(string titleV, Action actionV) = headerActionTuples[i];
					MidButton(titleV, actionV);
				}
				
				(string titleL, Action actionL) = headerActionTuples[len-1];
				RightButton(titleL, actionL);
				
				EditorGUILayout.EndHorizontal();
			}
		}

		protected static void MiniButton(string text, Action action) {
			if (Button(text, miniButton)) {
				action();
			}
		}
		
		protected static void LeftButton(string text, Action action) {
			if (Button(text, miniButtonLeft)) {
				action();
			}
		}

		protected static void MidButton(string text, Action action) {
			if (Button(text, miniButtonMid)) {
				action();
			}
		}

		protected static void RightButton(string text, Action action) {
			if(Button(text, miniButtonRight)) {
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
				LabelField(header, style);
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
			Rect r = GetControlRect(Height(padding+thickness));
			r.height = thickness;
			// ReSharper disable once PossibleLossOfFraction
			r.y += padding / 2;
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}
	}
}
