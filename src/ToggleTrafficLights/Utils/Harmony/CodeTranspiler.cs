using System.Collections.Generic;
using Harmony.ILCopying;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System;
using System.Collections;

namespace Harmony
{
	internal sealed class CodeTranspiler
	{
		private IEnumerable<CodeInstruction> codeInstructions;

		public CodeTranspiler(List<ILInstruction> ilInstructions)
		{
			codeInstructions = ilInstructions
				.Select(ilInstruction => ilInstruction.GetCodeInstruction())
				.ToList().AsEnumerable();
		}

		public static IEnumerable ConvertInstructions(Type type, IEnumerable enumerable)
		{
			var enumerableAssembly = type.GetGenericTypeDefinition().Assembly;
			var genericListType = enumerableAssembly.GetType(typeof(List<>).FullName);
			var elementType = type.GetGenericArguments()[0];
			var listType = enumerableAssembly.GetType(genericListType.MakeGenericType(new Type[] { elementType }).FullName);
			var list = Activator.CreateInstance(listType);
			var listAdd = list.GetType().GetMethod("Add");

			foreach (var op in enumerable)
			{
				var elementTo = Activator.CreateInstance(elementType, new object[] { OpCodes.Nop, null });
				Traverse.IterateFields(op, elementTo, (trvFrom, trvDest) => trvDest.SetValue(trvFrom.GetValue()));
				listAdd.Invoke(list, new object[] { elementTo });
			}
			return list as IEnumerable;
		}

		public IEnumerable<CodeInstruction> GetResult(ILGenerator generator, MethodBase method)
		{
			IEnumerable instructions = codeInstructions;
			instructions = ConvertInstructions(typeof(IEnumerable<CodeInstruction>), instructions);
			return instructions as IEnumerable<CodeInstruction>;
		}
	}
}
