using System.IO;
using Newtonsoft.Json;

namespace Tilemaps {
    public class CustomXmlJsonWriter : JsonTextWriter{
        public CustomXmlJsonWriter(TextWriter writer): base(writer){}

        public override void WritePropertyName(string name) {
            if (name.StartsWith("@") || name.StartsWith("#")) {
                base.WritePropertyName(name.Substring(1));
            } else {
                base.WritePropertyName(name);
            }
        }
    }
}
