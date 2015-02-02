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

		public static R With<T,R>(this T plugin, Func<T,R> action){
			if (Equals (plugin, default(T)) || action == null )
				return default(R);
			return action(plugin);
		}
	}

}
