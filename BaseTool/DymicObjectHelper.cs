
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace BaseTool
{
    public static class DymicObjectHelper
    {
        public static dynamic DynamicObject(object obj)
        {
            if (obj.GetType() == typeof(JObject)) return JObject2DynamicObject((JObject)obj);
            dynamic d = new ExpandoObject();
            var pis =obj.GetType().GetProperties().ToArray();
            foreach (var p in pis)
            {
                ((IDictionary<string, object>)d)[p.Name] = p.GetValue(obj, null);
            }
            return d;
        }

        public static dynamic JObject2DynamicObject(JObject jObject)
        {
            dynamic d = new ExpandoObject();
            var pis =jObject.Properties().ToArray();
            foreach (var p in pis)
            {
                if (!p.HasValues||p.Value.GetType()!= typeof(JValue)) continue;
                var jvalue = (JValue) p.Value;
               ((IDictionary<string, object>)d)[p.Name] =jvalue.Value;
            }
            return d;
        }
    }
}
