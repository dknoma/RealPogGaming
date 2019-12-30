using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Tilemaps {
	[Serializable]
	public class TilesetInfo {
		public int columns;
		public string image;
		public int imageheight;
		public int imagewidth;
		public int margin;
		public string name;
		public int spacing;
		public int tilecount;
		public string tiledversion;
		public int tileheight;
		public int tilewidth;
		public string version;
		public string type;
		public EditorSettings editorsettings;
		public Tile[] tiles;
        
		/// <summary>
		/// Utility method to get the right String  representation of these objects.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static string GetString(object obj) {
			string result;
			if(obj == null) {
				result = "N/A";
			} else {
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
					if(value == null) {
						valueString = "N/A";
					} else {
						if(value.GetType().IsArray) {
							StringBuilder sb = new StringBuilder();
	
							if(value is IEnumerable values) {
								foreach(object v in values) {
									sb.Append($"{v}, ");
								}
							}
	
							sb.Remove(sb.Length - 2, 2);
							valueString = sb.ToString();
						} else {
							valueString = value.ToString();
						}
					}

					builder.Append($"{name}={valueString} ");
				}

				builder.Remove(builder.Length - 1, 1);
				result = builder.ToString();
			}

			return result;
		}

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
		
		[Serializable]
		public class Tile {
			public int id;
			public ObjectGroup objectgroup;
    
			public override string ToString() {
				return $"EditorSettings [{GetString(this)}]";
			}
			
        
			[Serializable]
			public class ObjectGroup {
				public string draworder;
				public string name;
				public Object[] objects;
            
				public override string ToString() {
					return $"Export [{GetString(this)}]";
				}

				[Serializable]
				public class Object {
					public int id;
					public int x;
					public int y;
					public int width;
					public int height;
					public int roation;
					public string type;
					public bool visible;
					
					public override string ToString() {
						return $"Export [{GetString(this)}]";
					}
				}
			}
		}
	}
}
