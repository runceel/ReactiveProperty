using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Reactive.Bindings.Helpers
{
    [Obsolete("Please use JSON.NET")]
    public static class SerializeHelper
    {
        static IEnumerable<PropertyInfo> GetIValueProperties(object target) =>
            target.GetType().GetRuntimeProperties()
                .Where(pi => typeof(IReactiveProperty).GetTypeInfo().IsAssignableFrom(pi.PropertyType.GetTypeInfo()));

        /// <summary>
        /// Serialize target contains ReactiveProperty's Value.
        /// </summary>
        /// <param name="target">ReactiveProperty holder(such as ViewModel).</param>
        /// <returns>Serialized string.</returns>
        public static string PackReactivePropertyValue(object target)
        {
            var values = GetIValueProperties(target)
                .Where(pi => pi.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false).FirstOrDefault() == null)
                .ToDictionary(pi => pi.Name, pi =>
                {
                    var ivalue = (IReactiveProperty)pi.GetValue(target, null);
                    return ivalue?.Value;
                });

            var sb = new StringBuilder();
            var serializer = new DataContractSerializer(values.GetType());
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.WriteObject(writer, values);
            }
            return sb.ToString();
        }

        /// <summary>
        /// <para>Deserialize target's ReactiveProperty value.</para>
        /// <para>Deserialize order is at first DataMemberAttribute' Order, second alphabetical order.</para>
        /// </summary>
        /// <param name="target">ReactiveProperty holder(such as ViewModel).</param>
        /// <param name="packedData">Serialized string.</param>
        public static void UnpackReactivePropertyValue(object target, string packedData)
        {
            Dictionary<string, object> values;
            var serializer = new DataContractSerializer(typeof(Dictionary<string, object>));
            using (var sr = new StringReader(packedData))
            using (var reader = XmlReader.Create(sr))
            {
                values = (Dictionary<string, object>)serializer.ReadObject(reader);
            }

            var query = GetIValueProperties(target)
                .Select(pi =>
                {
                    var attr = (DataMemberAttribute)pi.GetCustomAttributes(typeof(DataMemberAttribute), false).FirstOrDefault();
                    var order = (attr != null) ? attr.Order : int.MinValue;
                    return new { pi, order };
                })
                .OrderBy(a => a.order)
                .ThenBy(a => a.pi.Name);

            foreach (var item in query)
            {
                object value;
                if (values.TryGetValue(item.pi.Name, out value))
                {
                    var ivalue = (IReactiveProperty)item.pi.GetValue(target, null);
                    if (ivalue != null) ivalue.Value = value;
                }
            }
        }
    }
}