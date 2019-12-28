using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tilemaps {
	[Serializable]
	public class TsxInfo {
		public Tileset tileset;
        
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

					if (value is IEnumerable values) {
						foreach (object v in values) {
							sb.Append($"{v}, ");
						}
					}

					sb.Remove(sb.Length - 2, 2);
					valueString = sb.ToString();
				} else {
					valueString = value.ToString();
				}
				builder.Append($"{name}={valueString} ");
			}
			builder.Remove(builder.Length - 1, 1);

			return builder.ToString();
		}

		public override string ToString() {
			return $"[{GetString(this)}]";
		}

		[Serializable]
		public class Tileset {
			public string version;
			public string tiledversion;
			public string name;

			public override string ToString() {
				return $"[{GetString(this)}]";
			}
			
			[Serializable]
			public class EditorSettings {
				public Export export;
        
				public override string ToString() {
					return $"EditorSettings [{GetString(this)}]";
				}
            
				[Serializable]
				public class Export {
					public string format;
					public string target;
                
					public override string ToString() {
						return $"Export [{GetString(this)}]";
					}
				}
			}
		}
	}
}
