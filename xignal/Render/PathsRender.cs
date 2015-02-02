// Paths.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using Android.Graphics;
using Xignal.Render;
using System.Linq;
using XPoint = Xignal.XPoint<float,float>;

namespace Xignal.Render
{

	public class PathsRender :ISignalRender
	{
		public Guid Identifier {get; private set ;}

		public string Title { get; private set; }

		public string Name { get; private set; }

		public bool IsEnabled { get; set;}

		Paint SignalPaint {	get;set;}

		public PathsRender(){

			SignalPaint = new Paint { 
				Color = Color.AliceBlue,
				StrokeWidth = 2,
				AntiAlias = true, 
			};

			Title = "Paths";
			Name = "paths";
			Identifier = Guid.NewGuid ();

		}

		public Action<Canvas> GetAction (XPoint[] xpoints, ActivityState context)
		{
			return new Action<Canvas> ((canvas) => {

				canvas.RotateY (context);

				var path = new Path ();
				var first = xpoints.First ();
				var min = xpoints.Min (x=>x.Y);
				var last = xpoints.Last ();
				path.MoveTo (0, 0);

				foreach (var point in xpoints.ToArray ())
					path.LineTo (point.X, point.Y);

				path.LineTo (last.X, min);
				path.LineTo (first.X, min);
				path.LineTo (first.X, first.Y);
				path.Close ();
				canvas.DrawPath (path, SignalPaint) ;

				canvas.RotateY (context);
			});
		}
			
	}

}
