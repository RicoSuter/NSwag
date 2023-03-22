using System;
using System.Linq;

namespace NSwag.Generation
{

	internal static class GenericResultWrapperTypes
	{
		private readonly static String[] WrapperTypeNames =
		{
			"Task`1",
			"ValueTask`1",
			"JsonResult`1",
			"ActionResult`1"
		};

		internal static bool IsGenericWrapperType(string typeName)
			=> WrapperTypeNames.Contains( typeName );

		internal static void RemoveGenericWrapperTypes<T>(ref T o,Func<T,string> getName,Func<T,T> unwrap)
		{
			// We iterate because a common signature is public async Task<ActionResult<T>> Action()
			while (IsGenericWrapperType(getName(o)))
			{
				o = unwrap(o);
			}
		}
	}
}
