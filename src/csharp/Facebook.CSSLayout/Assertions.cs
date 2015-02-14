using System.Diagnostics;

namespace Facebook.CSSLayout
{
	static class Assertions
	{
		public static T assertNotNull<T>(T v) where T : class
		{
			Debug.Assert(v != null);
			return v;
		}
	}
}
