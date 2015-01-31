// CanvasExtensions.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Reactive.Linq;
using System.Reactive;
using Android.Widget;
using Android.Views;
using TouchEventArgs = Android.Views.View.TouchEventArgs;
using Android.Graphics;

namespace Xignal
{

	public static class CanvasExtensions{

		public static void DrawLine(this Canvas canvas, XLine line, Paint paint){
			canvas.DrawLine (line.XStart, line.YStart, line.XEnd, line.YEnd, paint);
		}

		public static void DrawGrid(this Canvas canvas, XGrid grid,Paint paint){
			foreach(var line in grid.Vlines)
				canvas.DrawLine(line.XStart,line.YStart,line.XEnd,line.YEnd,paint);
			foreach(var line in grid.Hlines)
				canvas.DrawLine(line.XStart,line.YStart,line.XEnd,line.YEnd,paint);
		}

		public static Canvas RotateY(this Canvas canvas,ActivityState activityState){
			canvas.Scale (1,-1,activityState.Width/2,activityState.Height/2);
			return canvas;
		}

		public static Canvas RotateY(this Canvas canvas,float width,float height){
			canvas.Scale (1,-1,width/2,height/2);
			return canvas;
		}

		public static void DrawRotatedText(this Canvas canvas, 
			ActivityState activityState,
			string text,
			float yPosition,Paint paint,
			float margin = 2 ,bool textOnTop = true)
		{
			var textGap = margin + paint.TextSize;
			var invertedY = activityState.Height - yPosition;
			var upDown =  invertedY >=  activityState.Height-textGap ? -1 : 1 ;
			var textYPosition = (activityState.Height - yPosition + (textGap*upDown));
			var middle = activityState.HalfWidth;
			canvas
				//.RotateY (activityState)
				.DrawText (text,middle,textYPosition, paint);
			canvas.RotateY (activityState);

		}
	}
	
}
