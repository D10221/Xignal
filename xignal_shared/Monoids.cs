// Monoids.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;


namespace Xignal
{

	/// <summary>
	/// Monadic like extensions 
	/// </summary>
	public static class Monoids{

		public static void With<T>(this T plugin, Action<T> action){
			if (Equals (plugin, default(T)) || action == null )
				return;
			action(plugin);
		}
	}

}
