// ISignalRender.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel
using System;
using Android.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace Xignal.Render
{
	public interface ISignalRender
	{

		Action<Canvas>  GetAction(XPoint[] xpoints, ActivityState context);

		bool IsEnabled { get; set; }

		string Title { get; }
		string Name  { get; }
		Guid Identifier {get; }

	}

	public static class SignalRenderExtensions{

		public static void Enable(this IEnumerable<ISignalRender> signalRenders, ISignalRender render){

			foreach(var p in signalRenders.Where(r=> r.Identifier != render.Identifier)){
				p.IsEnabled = false;
			}

			render.IsEnabled = true;
		}
	}
}

