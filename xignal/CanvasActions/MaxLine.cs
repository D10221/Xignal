// MaxLine.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel
using System;
using System.Linq;
using Android.Graphics;
using XPoint = Xignal.XPoint<float,float>;

namespace Xignal.CanvasActions
{
	public class MaxLine : ICanvasAction
	{

		public CanvasActionOrder Order { get; private set; }

		public Action<Canvas> GetAction (XPoint[] xpoints, ActivityState context)
		{
			var max = xpoints.Max (x => x.Y);
			var maxPaint = new Paint{ Color = Color.Red, StrokeWidth = 1, AntiAlias = true };
			return canvas => {
				canvas.RotateY (context);
				canvas.DrawLine (new HLine (context.Width, max), maxPaint);
				canvas.RotateY (context);
				canvas.DrawRotatedText (context, "Max: " + (max), max, maxPaint);
				canvas.RotateY (context);

			};
		}

		public bool IsEnabled { get; set;	}

		public string Name { get; private set;}

		public string Title { get; private set;	}


		public MaxLine ()
		{
			Name = "max";
			Title = "Max";
			Order = CanvasActionOrder.AfterSignal;
		}
	}


}

