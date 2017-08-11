using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace Harmony.ILCopying
{
	internal static class Emitter
	{
		public static string CodePos(ILGenerator il)
		{
			var offset = Traverse.Create(il).Field("code_len").GetValue<int>();
			return "L_" + offset.ToString("x4") + ": ";
		}

		public static string FormatArgument(object argument)
		{
			if (argument == null) return "NULL";
			var type = argument.GetType();

			if (type == typeof(string))
				return "\"" + argument + "\"";
			if (type == typeof(Label))
				return "Label #" + ((Label)argument).GetHashCode();
			if (type == typeof(Label[]))
				return "Labels " + string.Join(" ", ((Label[])argument).Select(l => "#" + l.GetHashCode()).ToArray());
			if (type == typeof(LocalBuilder))
				return ((LocalBuilder)argument).LocalIndex + " (" + ((LocalBuilder)argument).LocalType + ")";

			return "" + argument;
		}

		public static void MarkLabel(ILGenerator il, Label label)
		{
			il.MarkLabel(label);
		}

		public static void Emit(ILGenerator il, OpCode opcode)
		{
			il.Emit(opcode);
		}

		public static void Emit(ILGenerator il, OpCode opcode, LocalBuilder local)
		{
			il.Emit(opcode, local);
		}

		public static void Emit(ILGenerator il, OpCode opcode, FieldInfo field)
		{
			il.Emit(opcode, field);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Label[] labels)
		{
			il.Emit(opcode, labels);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Label label)
		{
			il.Emit(opcode, label);
		}

		public static void Emit(ILGenerator il, OpCode opcode, string str)
		{
			il.Emit(opcode, str);
		}

		public static void Emit(ILGenerator il, OpCode opcode, float arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, byte arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, sbyte arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, double arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, int arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, MethodInfo meth)
		{
			il.Emit(opcode, meth);
		}

		public static void Emit(ILGenerator il, OpCode opcode, short arg)
		{
			il.Emit(opcode, arg);
		}

		public static void Emit(ILGenerator il, OpCode opcode, SignatureHelper signature)
		{
			il.Emit(opcode, signature);
		}

		public static void Emit(ILGenerator il, OpCode opcode, ConstructorInfo con)
		{
			il.Emit(opcode, con);
		}

		public static void Emit(ILGenerator il, OpCode opcode, Type cls)
		{
			il.Emit(opcode, cls);
		}

		public static void Emit(ILGenerator il, OpCode opcode, long arg)
		{
			il.Emit(opcode, arg);
		}

		public static void EmitCall(ILGenerator il, OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			il.EmitCall(opcode, methodInfo, optionalParameterTypes);
		}

		public static void EmitCalli(ILGenerator il, OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			il.EmitCalli(opcode, unmanagedCallConv, returnType, parameterTypes);
		}

		public static void EmitCalli(ILGenerator il, OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			il.EmitCalli(opcode, callingConvention, returnType, parameterTypes, optionalParameterTypes);
		}

		public static void EmitWriteLine(ILGenerator il, string value)
		{
			il.EmitWriteLine(value);
		}

		public static void EmitWriteLine(ILGenerator il, LocalBuilder localBuilder)
		{
			il.EmitWriteLine(localBuilder);
		}

		public static void EmitWriteLine(ILGenerator il, FieldInfo fld)
		{
			il.EmitWriteLine(fld);
		}

	}
}
