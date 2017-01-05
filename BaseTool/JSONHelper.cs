using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace BaseTool
{
    public static class JsonHelper
    {

        public static object ChangeType2(object value, Type type)
        {
            if (type == typeof(bool))
            {
                if (value != null)
                {
                    if (value.ToString().Trim().ToUpper() == "1" || value.ToString().Trim().ToUpper() == "TRUE")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == typeof(string))
            {
                var value1 = value.ToString();
                if (!string.IsNullOrEmpty(value1))
                {
                    value = value1;
                }
                else
                {
                    return null;
                }
            }
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                var s = value as string;
                if (s != null)
                    return Enum.Parse(type, s, true);
                return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ChangeType2(value, innerType);
                return Activator.CreateInstance(type, innerValue);
            }
            var s1 = value as string;
            if (s1 != null && type == typeof(Guid)) return new Guid(s1);
            var version = value as string;
            if (version != null && type == typeof(Version)) return new Version(version);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type, Provider);
        }

        public static CultureInfo Provider = new CultureInfo("zh-CN");

        public static object ChangeType(object value, Type type)
        {

            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == typeof(string))
            {
                var value1 = GetString(value.ToString());
                if (!string.IsNullOrEmpty(value1))
                {
                    value = value1;
                }
                else
                {
                    return null;
                }
            }
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                var s = value as string;
                if (s != null)
                    return Enum.Parse(type, s, true);
                return Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, innerValue);
            }
            var s1 = value as string;
            if (s1 != null && type == typeof(Guid)) return new Guid(s1);
            var version = value as string;
            if (version != null && type == typeof(Version)) return new Version(version);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type, Provider);
        }


        public static string SerializerJson(object o)
        {
            var mem = new MemoryStream();
            try
            {
                var jsondata = new DataContractJsonSerializer(o.GetType());
                jsondata.WriteObject(mem, o);
                return Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
            }
            catch
            {
               return null;
            }
            finally
            {
                mem.Close();
                mem.Dispose();
            }
        }

        public static T DeSerializerJson<T>(byte[] bytes, bool showerror) where  T:class 
        {
            var ms = new MemoryStream(bytes);
            try
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                return  (T)ser.ReadObject(ms);
            }
            catch (Exception exception)
            {
                if (showerror)
                    MessageBox.Show(exception.ToString());
                return null;
            }
            finally
            {
                ms.Close();
                ms.Dispose();
            }
        }

        public static Dictionary<string, string> GetDictionary(string json)
        {
            var dicts=new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(json))
            {
                return dicts;
            }
            var jObject = JObject.Parse(json);
            var properties=
            jObject.Properties();
            var names = properties.Select(p => p.Name).ToArray();
            foreach (var name in names)
            {
                dicts.Add(name,jObject[name].Value<string>());
            }
            return dicts;
        }


        public static bool IsDate(string str)
        {
            return Regex.IsMatch(str, "([0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]|[0-9][1-9][0-9]{2}|[1-9][0-9]{3})((0[13578]|10|12)(0[1-9]|1[0-9]|2[0-9]|3[01])|(0[469]|11)(0[1-9]|1[0-9]|2[0-9]|30)|02(0[1-9]|1[0-9]|2[0-8]))|(((0[48]|[13579][26]|[2468][048])000209)|([0-9]{2}(0[48]|[13579][26]|[2468][048])0209))");
        }

        public static string GetStringbyBase64(string input)
        {
            var c = Convert.FromBase64String(input);
            input = Encoding.UTF8.GetString(c, 0, c.Length);
            return input;
        }

        public static string Getbase64String(string input)
        {
            var b = Encoding.UTF8.GetBytes(input);
            input= Convert.ToBase64String(b);
            return input;
        }

        public static string GetString(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (input.StartsWith("\""))
            {
               input = input.Substring(1, input.Length - 1);
            }
            if (input.EndsWith("\""))
            {
                input = input.Substring(0, input.Length - 1);  
            }
            return input;
        }

        

        
    }
  
}
