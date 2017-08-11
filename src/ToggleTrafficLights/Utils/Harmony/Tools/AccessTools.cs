using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	internal static class AccessTools
	{
		public static BindingFlags all = BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.Instance
			| BindingFlags.Static
			| BindingFlags.GetField
			| BindingFlags.SetField
			| BindingFlags.GetProperty
			| BindingFlags.SetProperty;

		public static Type TypeByName(string name)
		{
			var type = Type.GetType(name, false);
			if (type == null)
				type = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.FirstOrDefault(x => x.FullName == name);
			if (type == null)
				type = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.FirstOrDefault(x => x.Name == name);
			return type;
		}

		public static T FindRecursive<T>(Type type, Func<Type, T> action)
		{
			while (true)
			{
				var result = action(type);
				if (result != null) return result;
				if (type == typeof(object)) return default(T);
				type = type.BaseType;
			}
		}

		public static FieldInfo Field(Type type, string name)
		{
			if (type == null || name == null) return null;
			return FindRecursive(type, t => t.GetField(name, all));
		}

		public static PropertyInfo Property(Type type, string name)
		{
			if (type == null || name == null) return null;
			return FindRecursive(type, t => t.GetProperty(name, all));
		}

		public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			if (type == null || name == null) return null;
			MethodInfo result;
			if (parameters == null)
				result = FindRecursive(type, t => t.GetMethod(name, all));
			else
			{
				var modifiers = new ParameterModifier[] { };
				result = FindRecursive(type, t => t.GetMethod(name, all, null, parameters, modifiers));
			}
			if (result == null) return null;
			if (generics != null) result = result.MakeGenericMethod(generics);
			return result;
		}

		public static Type GetReturnedType(MethodBase method)
		{
			var constructor = method as ConstructorInfo;
			if (constructor != null) return typeof(void);
			return ((MethodInfo)method).ReturnType;
		}

		public static Type Inner(Type type, string name)
		{
			if (type == null || name == null) return null;
			return FindRecursive(type, t => t.GetNestedType(name, all));
		}

		public static Type[] GetTypes(object[] parameters)
		{
			if (parameters == null) return new Type[0];
			return parameters.Select(p => p == null ? typeof(object) : p.GetType()).ToArray();
		}

		public static List<string> GetFieldNames(Type type)
		{
			return type.GetFields(all).Select(f => f.Name).ToList();
		}

		public static List<string> GetFieldNames(object instance)
		{
			if (instance == null) return new List<string>();
			return GetFieldNames(instance.GetType());
		}

		public static List<string> GetPropertyNames(Type type)
		{
			return type.GetProperties(all).Select(f => f.Name).ToList();
		}

		public static List<string> GetPropertyNames(object instance)
		{
			if (instance == null) return new List<string>();
			return GetPropertyNames(instance.GetType());
		}

		public static bool isStruct(Type type)
		{
			return type.IsValueType && !isValue(type) && !isVoid(type);
		}

		public static bool isClass(Type type)
		{
			return !type.IsValueType;
		}

		public static bool isValue(Type type)
		{
			return type.IsPrimitive || type.IsEnum;
		}

		public static bool isVoid(Type type)
		{
			return type == typeof(void);
		}
	}
}
