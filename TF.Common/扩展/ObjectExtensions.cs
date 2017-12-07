using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml.Serialization;

namespace TF.Common
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T source)
            where T : class
        {
            if (Object.ReferenceEquals(source, null)) return null;
            if (!typeof(T).IsSerializable)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xml = new XmlSerializer(source.GetType());
                    xml.Serialize(ms, source);
                    ms.Position = 0;
                    return xml.Deserialize(ms) as T;
                }
            }
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static bool IsIn<T>(this T o, params T[] c)
            where T : struct
        {
            foreach (T i in c)
                if (i.Equals(o)) return true;
            return false;
        }

        public static bool IsIn<T>(this T o, List<T> c)
            where T : struct
        {
            foreach (T i in c)
                if (i.Equals(o)) return true;
            return false;
        }

        public static bool IsNotIn<T>(this T o, params T[] c)
            where T : struct
        {
            foreach (T i in c)
                if (i.Equals(o)) return false;
            return true;
        }

        public static bool IsNotIn<T>(this T o, List<T> c)
            where T : struct
        {
            foreach (T i in c)
                if (i.Equals(o)) return false;
            return true;
        }

        public static string ToStringFormat<T>(this T anObject, string aFormat)
            where T : class
        {
            return anObject.ToStringFormat(aFormat, null);
        }

        private static string ToStringFormat<T>(this T anObject, string aFormat, IFormatProvider formatProvider)
            where T : class
        {
            var sb = new StringBuilder();
            var reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(aFormat);

            int startIndex = 0;

            foreach (Match m in mc)
            {
                Group g = m.Groups[2];
                int length = g.Index - startIndex - 1;
                sb.Append(aFormat.Substring(startIndex, length));

                string getValue;
                string format = string.Empty;

                int formatIndex = g.Value.IndexOf(":", StringComparison.Ordinal);
                if (formatIndex == -1)
                {
                    getValue = g.Value;
                }
                else
                {
                    getValue = g.Value.Substring(0, formatIndex);
                    format = g.Value.Substring(formatIndex + 1);
                }

                // Make sure we're not dealing
                if (!getValue.StartsWith("{"))
                {
                    // with a string literal wrapped in {}
                    // Get the object's value using DataBinder.Eval.
                    object resultAsObject = DataBinder.Eval(anObject, getValue);

                    // Format the value based on the incoming formatProvider
                    // and format string
                    string result = string.Format(formatProvider, "{0:" + format + "}", resultAsObject);

                    sb.Append(result);
                }
                else
                {
                    // Property name started with a { which means we treat it as a literal.
                    sb.Append(g.Value);
                }

                startIndex = g.Index + g.Length + 1;
            }

            if (startIndex < aFormat.Length)
            {
                sb.Append(aFormat.Substring(startIndex));
            }

            return sb.ToString();
        }

        public static Dictionary<string, object> ToDictionary<T>(this T obj)
            where T : class
        {
            var dict = new Dictionary<string, object>();
            if (obj.GetType().IsSubclassOf(typeof(NameValueCollection)))
            {
                var nameValueCollection = (NameValueCollection)(object)obj;
                nameValueCollection.Cast<string>()
                     .Select(key => new KeyValuePair<string, object>(key, nameValueCollection[key]))
                     .ToList().ForEach(q => dict.Add(q.Key, q.Value));
            }
            else
            {
                var props = obj.GetType().GetProperties();
                props.Where(p => p.CanRead == true).ToList().ForEach(
                    item => dict.Add(item.Name, item.GetValue(obj, null)));
            }
            return dict;
        }


        public static string ToString(this DateTime? dt,string fmt)
        {
            if (dt!=null)
            {
                return ((DateTime)dt).ToString(fmt);
            }
            return "";
        }
        public static string ToString(this DateTime? dt, string fmt,DateTime @default)
        {
            if (dt != null)
            {
                return ((DateTime)dt).ToString(fmt);
            }
            return @default.ToString(fmt);
        }
        public static string ToString(this bool b, string trueString, string falseString)
        {
            return b ? trueString : falseString;
        }


    }
}