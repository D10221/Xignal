// MenuItemExtensions.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using Android.Views;
using TouchEventArgs = Android.Views.View.TouchEventArgs;

namespace Xignal
{

	public static class MenuItemExtensions{

		/*public static IMenuItem With(this IMenuItem item, Action<IMenuItem> itemAction){
			itemAction (item);
			return item;
		}*/
		public static IMenuItem OnClick(this IMenuItem item, Action<IMenuItem> clickAction){
			return item.SetOnMenuItemClickListener (new MenuItemClickListener(clickAction));
		}
	}
	
}
