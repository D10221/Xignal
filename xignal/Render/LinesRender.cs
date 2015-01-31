// Lines.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using Android.Graphics;
using Xignal.Render;
using System.Linq;

namespace Xignal.Render
{

	public class LinesRender  :ISignalRender
	{
		public Guid Identifier {get; private set ;}
			
		public string Title { get; private set; }

		public string Name { get; private set; }

		public bool IsEnabled { get; set;}

		Paint SignalPaint {	get;set;}

		public LinesRender(){

			Title = "Lines";

			Name = "lines";

			SignalPaint = new Paint { 
				Color = Color.AliceBlue,
				StrokeWidth = 2,
				AntiAlias = true, 
			};

			IsEnabled = true;

			Identifier = Guid.NewGuid ();
		}

		public Action<Canvas> GetAction (XPoint[] xpoints, ActivityState context)
		{
			return new Action<Canvas> ((canvas) => {

				canvas.RotateY (context);

				foreach (var group in xpoints.GroupByNext ()) {
					var start = group.First ();
					var end = group.Last ();
					canvas.DrawLine (start.X, start.Y, end.X, end.Y, SignalPaint);
				}

				canvas.RotateY (context);

			});
		}

	}
}
