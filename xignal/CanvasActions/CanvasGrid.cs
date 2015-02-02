// CanvasGrid.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using Android.Graphics;
using XPoint = Xignal.XPoint<float,float>;

namespace Xignal.CanvasActions
{
	public class CanvasGrid : ICanvasAction
	{
		public CanvasActionOrder Order { get; private set; }

		Paint _paint ;

		float _gridFactor;

		public bool IsEnabled{ get; set;}

		public string Name { get; private set;}

		public string Title { get; private set;}


		public CanvasGrid ()
		{
			Name = "grid";

			Title = "Grid";

			_paint = new Paint {
				Color = Color.LightSalmon,
				StrokeWidth = 1,
				AntiAlias = true, 
			};

			_gridFactor = 1;

			Order = CanvasActionOrder.BeforeSignal;
		}

		public Action<Canvas> GetAction (XPoint[] xpoints,ActivityState context){
			context.GridFactor = _gridFactor;
			var grid = new XGrid (context);
			return c => c.DrawGrid (grid, _paint);
		}
	}

}
