using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets
{
    public class JsonAssetsJsonContractResolver : IgnorePropertiesOfTypeJsonContractResolver
    {
        private readonly JsonAssetsColorConverter colorConverter = new JsonAssetsColorConverter();
        private readonly JsonAssetsBigCraftableExtraIndicesConverter extraIndicesConverter = new JsonAssetsBigCraftableExtraIndicesConverter();

        public JsonAssetsJsonContractResolver()
        : base(new Type[] { typeof(ImageResourceReference) })
        {
        }


        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (objectType == typeof(Color))
                return colorConverter;
            else if (objectType == typeof(List<ImageResourceReference>))
                return extraIndicesConverter;
            return base.ResolveContractConverter(objectType);
        }
    }

    internal class JsonAssetsColorConverter : JsonConverter<Color>
    {
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var toks = serializer.Deserialize<string>(reader).Split(",");
            for (int i = 0; i < toks.Length; ++i)
                toks[i] = toks[i].Trim();

            return Godot.Color.Color8(byte.Parse(toks[0]), byte.Parse(toks[1]), byte.Parse(toks[2]), toks.Length > 3 ? byte.Parse(toks[3]) : (byte)255);
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, $"{value.r8}, {value.g8}, {value.b8}, {value.a8}");
        }
    }

    internal class JsonAssetsBigCraftableExtraIndicesConverter : JsonConverter<List<ImageResourceReference>>
    {
        public override List<ImageResourceReference> ReadJson(JsonReader reader, Type objectType, List<ImageResourceReference> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, List<ImageResourceReference> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Count);
        }
    }
}
