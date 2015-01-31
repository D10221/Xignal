// Sface.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;

using Android.Content;
using Android.Util;
using Android.Views;
using Android.Graphics;
using Color = Android.Graphics.Color;
using PointF = System.Drawing.PointF;


namespace Xignal
{
	public class Xurface : View
	{
		#region Constructors

		public Xurface (Context context) :
			base (context)
		{
			Initialize ();
		}

		public Xurface (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public Xurface (Context context, IAttributeSet attrs, int defStyle) :
			base (context, attrs, defStyle)
		{

			Initialize ();
		}


		#endregion

		void Initialize ()
		{
		}

		public void Drawit(Canvas canvas, params Action<Canvas>[] actions){
			CanvasActions = actions;
			Draw (canvas);
		}


		Action<Canvas>[] CanvasActions {get;set;}

		protected override void OnDraw(Canvas canvas)
		{

			if (CanvasActions != null) {
				foreach(var canvasAction in CanvasActions)
					canvasAction (canvas);
			}

			base.OnDraw(canvas);
		}

		public void Clear ()
		{
			CanvasActions = null;
			using (var paint = new Paint {	Color = Color.Black	}) 
			using (var canvas = new Canvas ()) {
				canvas.DrawRect(0,0, this.Width,this.Height,paint);
				Draw (canvas);
			}
			Invalidate ();
		}


	}
}

