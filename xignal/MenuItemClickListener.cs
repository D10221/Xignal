// MenuItemClickListener.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Graphics;

using System.Reactive.Linq;
using System.ComponentModel;

namespace Xignal
{
	
	class MenuItemClickListener :  Java.Lang.Object,IMenuItemOnMenuItemClickListener
	{
		readonly Action<IMenuItem> _action ;

		public MenuItemClickListener (Action<IMenuItem> action)
		{
			if (action == null)
				throw new ArgumentException ("action can't be null");

			_action = action;
		}

		public bool OnMenuItemClick (IMenuItem item)
		{
			_action (item);
			return true;
		}
	}
}
