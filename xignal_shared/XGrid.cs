// XGrid.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Linq;

namespace Xignal
{

	public class XGrid{

		public XLine[] Vlines {	get; private set; }
		public XLine[] Hlines {	get; private set; }
		public bool IsVisible{ get; set;}
		//public float Factor{ get; set;}

		public XGrid(ActivityState activityState){
		
			var factor = activityState.GridFactor;
			var height = activityState.Height;
			var width = activityState.Width;
			var hsteps = (int)(activityState.Steps / factor);
			var hstep = (int)(activityState.Step * factor);
			var vsteps = (int)(activityState.VSteps / factor);
			var vstep = (int)(activityState.VStep * factor);

			Func<int,XLine> vline = x => new XLine (x, 0, x, height);
			Func<int,XLine> hline = y => new XLine (0, y, width, y);

			//Calculate Grid
			Vlines = Enumerable
				.Range (0,hsteps)
				.Select(x=> x*hstep)
				.Select (vline)
				.ToArray ();
			//Log.Debug(TAG, "x: "  + vlines.Select(x=> x.XEnd).Select (x=>x.ToString ()).AsStringValues ());
			Hlines =  Enumerable
				.Range (0,vsteps)
				.Select(x=> x*vstep)
				.Select (hline)
				.ToArray ();
			//Log.Debug(TAG, "x: "  + hlines.Select(x=> x.YEnd).Select (x=>x.ToString ()).AsStringValues ());
		}
	}
}
