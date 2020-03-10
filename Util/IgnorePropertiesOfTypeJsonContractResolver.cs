using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    public class IgnorePropertiesOfTypeJsonContractResolver : DefaultContractResolver
    {
        private readonly IEnumerable<Type> types;
        public IgnorePropertiesOfTypeJsonContractResolver(IEnumerable<Type> theTypes)
        {
            types = theTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            prop.ShouldSerialize = (x) => !types.Contains(prop.PropertyType);
            return prop;
        }
    }
}
