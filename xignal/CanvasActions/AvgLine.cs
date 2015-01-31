// AvgLine.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Linq;
using Android.Graphics;

namespace Xignal.CanvasActions
{

	public class AvgLine : ICanvasAction
	{

		public CanvasActionOrder Order { get; private set; }

		const string label = "Avg: ";
		Color PaintColor = Color.Green; 
		const float StrokeWidth = 1 ;

		public Action<Canvas> GetAction (XPoint[] xpoints, ActivityState context)
		{
			var paint = new Paint{ Color = PaintColor, StrokeWidth = StrokeWidth, AntiAlias = true };

			var y = xpoints.Average (x => x.Y);

			return canvas => {
				canvas.RotateY (context);
				canvas.DrawLine (new HLine (context.Width, y), paint);
				canvas.RotateY (context);
				canvas.DrawRotatedText (context, label + (y), y, paint);
			};

		}

		public bool IsEnabled { get; set;	}

		public string Name { get; private set;}

		public string Title { get; private set;	}

		public AvgLine ()
		{
			Name = "avg";
			Title = "Avg";
			Order = CanvasActionOrder.AfterSignal;
		}
	}
}
