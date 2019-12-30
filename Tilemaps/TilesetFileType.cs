using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tilemaps {
	public static class TilesetFileType {
		public enum Type {
			JSON,
			TSX
		}
		
		public const string JSON_EXTENSION = ".json";
		public const string TSX_EXTENSION = ".tsx";

		private static readonly Regex EXTENSION_PATTERN = new Regex("(\\.(.)+)");
		private static readonly IDictionary<string, Type> TYPES_BY_EXTENSION = new Dictionary<string, Type>();

		static TilesetFileType() {
			TYPES_BY_EXTENSION.Add(JSON_EXTENSION, Type.JSON);
			TYPES_BY_EXTENSION.Add(TSX_EXTENSION, Type.TSX);
		}

		public static Type GetTypeByExtension(string extension) {
			return TYPES_BY_EXTENSION.GetOrDefault(extension);
		}

		public static Type GetTypeFromSourcePath(string sourcePath) {
			MatchCollection matches = EXTENSION_PATTERN.Matches(sourcePath);
			Match ext = matches[matches.Count - 1];
			string extension = string.Intern(ext.ToString());
			
			return TYPES_BY_EXTENSION.GetOrDefault(extension);
		}
	}
}
