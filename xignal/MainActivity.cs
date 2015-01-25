// MainActivity.cs
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
using System.Reactive.Linq;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System.ComponentModel;

namespace Xignal
{
	[Activity (Label = "Xignal", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int _width;
		int _scale;
		int _samplingRate;
		int _height;
		int _signalRate;
		int _step;
		int _steps;

		int _halfHeight;

		float _signalThinknes;
		Color _signalColor;
		Paint _signalPaint;

		Paint SignalPaint{ get { return _signalPaint; } }

		Paint _gridPaint;

		public Paint GridPaint {
			get {
				return _gridPaint;
			}
			set {
				_gridPaint = value;
			}
		}

		public MainActivity ()
		{
			_scale = 20;
			_samplingRate = 150;
			_signalColor = Color.Yellow;
			_signalRate = 200;
			_signalThinknes = 2;

			_signalPaint = new Paint { 
					Color = _signalColor,
					StrokeWidth = _signalThinknes,
					AntiAlias = true
				};

			_gridPaint = new Paint{ Color = Color.White, StrokeWidth = 2 };
		}
		
		Xurface _surface;
		Button _startStopBtn;

		IDisposable _signalSubscription;

		States _state;

		XPoint<float>[] list;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			_surface = FindViewById<Xurface> (Resource.Id.surfaceView1);

			_startStopBtn = FindViewById<Button> (Resource.Id.startStopBtn);

			list = new XPoint<float>[0];

			Func<int> getSignal = () => {
				var halfHeight = _height / 2;
				var r = _random.Next (-halfHeight, halfHeight);
				return halfHeight + r;
			};

			var signal = Observable.Interval (TimeSpan.FromMilliseconds (_signalRate))
				.Select (x => getSignal());

			var signals = Observable.Interval (TimeSpan.FromMilliseconds (_samplingRate))
				//.Take(_scale)	
				//.Where(n => n > 0 )	
			.Do (x =>  {
				if (_width == 0)
					_width = _surface.Width;
				if (_height == 0)
					_height = _surface.Height;
				if (_step == 0)
					_step = _width / _scale;
				if (_steps == 0 && _step != 0 )
						_steps = _width / _step;
				if (_halfHeight == 0)
					_halfHeight = _height / 2;
			})
			.Select (i => i == 0 ? 0 : _step * i)
			.Zip (signal, (x, y) => new {
				X = x,
				Y = y
			})
			.Select (x => new XPoint<float> (x.X, x.Y)).Select (point =>  {
				
				if (_state!= States.Paused) {
					var xlist = list.ToList ();
					xlist.Add (point);
					if (point.X > _width) {
						xlist = xlist
								.Select (x => 
									new XPoint<float> (x.X - (_step), x.Y)
								)
								.Where (x => x.X >= 0).ToList ();
					}
					list = xlist
							.Take(_steps)
							.ToArray ();
				}
				return list;
			});

			Action subscribe = () => {

				list =  new XPoint<float>[0];

				_signalSubscription = signals.Subscribe( points => {		

					Action<Canvas,View> action = (c,view)=>{									
											 
							c.DrawLine (0, _halfHeight, _width, _halfHeight, GridPaint);
							foreach (var group in points.GroupByNext ()) {
								var start = group.First ();
								var end = group.Last ();
								c.DrawLine (start.X, start.Y, end.X, end.Y, SignalPaint);
								RunOnUiThread (view.Invalidate);
							}							
					};

					using(var _canvas = new Canvas())
						_surface.Drawit (_canvas, action);
				});
			};

			_startStopBtn.Clicks ().Subscribe (e => {
				var changed = PauseContinue();
				if(!changed)
					StartStop (subscribe);
				_startStopBtn.Text = GetNextStateDescription ();
			});

			_startStopBtn.LongClicks().Subscribe (e => {
				StartStop (subscribe);
				_startStopBtn.Text = GetNextStateDescription ();

			});
							
			if (_state == States.Running)
				StartStop (subscribe);
		}

		void StartStop (Action subscribe)
		{
			if (_signalSubscription != null) {
				_signalSubscription.Dispose ();
				_signalSubscription = null;
				_state = States.Stopped;
			}
			else {
				subscribe ();
				_state = States.Running;
			}				
		}

		bool PauseContinue ()
		{
			var changed = false;
			switch (_state) {
				case States.Running:
					_state = States.Paused;
					changed = true;
					break;
				case States.Paused:
					_state = States.Running;
					changed = true;
					break;
			}
			return changed;
		}
			
		readonly Random _random = new Random ();

		string GetNextStateDescription(){

			switch (_state) {
				case States.Stopped : return "Start";
				case States.Running : return "Freeze";
				case States.Paused:	  return "Continue";
			}

			throw new InvalidEnumArgumentException (
				"Dont' know what to do with {0}:{1}"
				.Formatify(_state.ToString (), (int)_state)
			);
		}
	}		
		

	public enum States{
		Stopped,
		Running,
		Paused
	}
}

