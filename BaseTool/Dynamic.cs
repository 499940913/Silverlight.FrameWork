using System;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections.Generic;

namespace BaseTool
{

    public class Dynamic
    {
        public Dynamic Add<T>(string key, T value)
        {
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic.dll");
            var typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());
            typeBuilder.SetParent(GetType());
            var propertyBuilder = typeBuilder.DefineProperty(key, PropertyAttributes.None, typeof(T), Type.EmptyTypes);
            var getMethodBuilder = typeBuilder.DefineMethod("get_" + key, MethodAttributes.Public, CallingConventions.HasThis, typeof(T), Type.EmptyTypes);
            var getter = getMethodBuilder.GetILGenerator();
            getter.Emit(OpCodes.Ldarg_0);
            getter.Emit(OpCodes.Ldstr, key);
            getter.Emit(OpCodes.Callvirt, typeof(Dynamic).GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(T)));
            getter.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);
            var type = typeBuilder.UnderlyingSystemType ?? typeBuilder.CreateType(); 
            var child = (Dynamic)Activator.CreateInstance(type);
            child._dictionary = _dictionary;
            _dictionary.Add(key, value);
            return child;
        }

        protected T Get<T>(string key)
        {
            return (T)_dictionary[key];
        }

        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();
    }
}
