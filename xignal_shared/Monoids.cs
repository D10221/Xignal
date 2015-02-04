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

		public static T DoWith<T>(this T target, Action<T> action){
			if (Equals (target, default(T)) || action == null )
				action(target);
			return  target;
		}

		public static void With<T>(this T target, Action<T> action){
			if (Equals (target, default(T)) || action == null )
				return;
			action(target);
		}

		public static R With<T,R>(this T plugin, Func<T,R> action){
			if (Equals (plugin, default(T)) || action == null )
				return default(R);
			return action(plugin);
		}
	}

}
