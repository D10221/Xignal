using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Graphics;

using System.Reactive.Linq;
using System.ComponentModel;

namespace Xignal
{
	[Activity (Label = "Xignal", MainLauncher = true, Icon = "@drawable/icon")]
	public class NoMain : Activity , IMenuItemOnMenuItemClickListener
	{
		const string TAG = "NoMain";

		Xurface _surface;

		ActivityState ActivityState;
		readonly Random _random =  new Random();
		IDisposable _subscription ;
		Action subscribe ;
		Paint _signalPaint;
		string _mode;
		string[] _modes;

		int _gridFactor;
		bool _showGrid;
		public Paint GridPaint { get; set; }

		public NoMain(){
			_gridFactor = 4;
			_showGrid = false;

			GridPaint = new Paint {
				Color = Color.LightSalmon,
				StrokeWidth = 1,
				AntiAlias = true , 
			};

			_signalPaint = new Paint { 
				Color = Color.AliceBlue,
				StrokeWidth = 2,
				AntiAlias = true , 
			};

			Func<XPoint[],Action<Canvas>> _renderLines = torender => new Action<Canvas>((canvas) => {

				foreach (var group in torender.GroupByNext ()) {
					var start = group.First ();
					var end = group.Last ();
					canvas.DrawLine (start.X, start.Y, end.X, end.Y, _signalPaint);
				}

			});

			Func<XPoint[],Action<Canvas>> _renderPaths = torender => new Action<Canvas>( (canvas) =>
				{
					var path = new Path ();
					var firstY = torender.First ().Y;
					path.MoveTo (0, 0);

					foreach (var point in torender.ToArray ())
						path.LineTo (point.X, point.Y);

					path.LineTo (ActivityState.Height, ActivityState.Height);
					path.LineTo (0, ActivityState.Height);
					path.LineTo (0, firstY);
					path.Close ();
					canvas.DrawPath (path, _signalPaint);

				}
			);
				
			Render = new Dictionary<string,Func<XPoint[],Action<Canvas>>>{
				{"lines", _renderLines},
				{"paths", _renderPaths}
			};

			_modes = new[]{"lines", "paths"};
			_mode = "lines";
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			_surface = FindViewById<Xurface> (Resource.Id.surfaceView1);		

			ActivityState = new ActivityState ();

			ActivityState.Scale = 10;
			ActivityState.SamplingRate = 200;
			ActivityState.SignalRate = 200;


			Func<int> getSignal = () => {
				var halfHeight = ActivityState.Height / 2;
				var r = _random.Next (-halfHeight, halfHeight);
				return halfHeight + r;
			};

		

			var signalSource = Observable.Interval (TimeSpan.FromMilliseconds (ActivityState.SignalRate))
				.Select (x => getSignal());

			var timer = Observable.Interval (TimeSpan.FromMilliseconds (ActivityState.SamplingRate));

			var signalSources = timer
			              // Setup , dimensions 
				.Do (x => 
					ActivityState.SetDimensions (_surface.Width, _surface.Height))
			              //Merge Signal and TimeLine
				.Zip (signalSource, (x, y) => new Signal ((int)x,y));

			var points = new List<XPoint> ();

							
			Func<Signal,XPoint> toXPoint = rsignal=> new XPoint(rsignal.X * ActivityState.Step ,rsignal.Y);


			Func<Signal,Signal> frameByFrame = (rawSignal)=> {

				if (ActivityState.IsThereRoomFor (rawSignal)) return rawSignal;

				points = points
					.Skip(points.Count)
					.Select(x=> shifXtLeftBy(x, points.Count))
					.ToList ();

				return new Signal (rawSignal.X - points.Count , rawSignal.Y);
			};

			Func<Signal,Signal> continuosSignal = (rawSignal)=> {

				if (ActivityState.IsThereRoomFor (rawSignal)) return rawSignal;

				points = points.Skip(1).Select(shifXtLeft).ToList ();

				return new Signal (points.Count , rawSignal.Y);
			};

			IDictionary<string,Func<Signal,Signal>> signalPreProcessors = new Dictionary<string,Func<Signal,Signal>> {
				{"continuosSignal", continuosSignal},
				{"frameByFrame", frameByFrame }
			};

			const string renderMode = "continuosSignal";

			subscribe = () => {
				points = new List<XPoint>();
				if(_subscription!=null ) _subscription.Dispose ();
				_subscription = signalSources
					.TakeWhile (x => ActivityState.State == States.Running)				
					.Select (rawSignal => signalPreProcessors [renderMode] (rawSignal))
					.Select (toXPoint)
					.Select (point=> {
						points.Add(point);
						return points.ToArray ();
					})
					.Subscribe (xpoints=>{
					
						/*c.DrawText ("Avg x: "+avgX,100,100,_textPaint);
						c.DrawText ("Min x: "+min,100,125,_textPaint);
						c.DrawText ("Max x: "+max,100,145,_textPaint);*/

						var _actions = new List<Action<Canvas>>();

						if(_showGrid){
							var grid = new XGrid(ActivityState){IsVisible = _showGrid};
							Action<Canvas> drawGrid = c => {						
								foreach(var line in grid.Vlines)
									c.DrawLine(line.XStart,line.YStart,line.XEnd,line.YEnd,GridPaint);
								foreach(var line in grid.Hlines)
									c.DrawLine(line.XStart,line.YStart,line.XEnd,line.YEnd,GridPaint);
							};
							_actions.Add(drawGrid);
						}

						_actions.Add(Render[_mode](xpoints));

						using (var _canvas = new Canvas ()){																
							_surface.Drawit (_canvas,_actions.ToArray ());
						}

						RunOnUiThread (_surface.Invalidate);

					}, /*onCompleted*/subscribe);
			};

			Action Unsubscribe = () => {
				if(_subscription!=null) _subscription.Dispose ();
			};
				
			//subscribe ();

			ActivityState
				.Changed				
				.Where(p=> p.Name == "State")
				.Subscribe(p => {
					var state = p.Value as States?;

					switch(state){
						case States.Running:
							subscribe();
							break;
						case States.Stopped:
							Unsubscribe();
							break;
					}
				});

			_surface.Touchs ()
				.Throttle (TimeSpan.FromMilliseconds (500))
				.Subscribe (e => {
				switch(ActivityState.State){
					case States.Running:
						ActivityState.State = States.Stopped;
						break;
					case States.Stopped:
						ActivityState.State = States.Running;
						break;
				}										
			});


						
		}
			
		IDictionary<string,Func<XPoint[],Action<Canvas>>> Render;

		XPoint shifXtLeft (XPoint point){
			return shifXtLeftBy (point, 1);
		}

		XPoint shifXtLeftBy (XPoint point, int howMany )
		{
			var p = new XPoint (point.X - ActivityState.Step*howMany, point.Y);
			return p;
		}

		readonly IDictionary<string,IntPtr> _ptrs = new Dictionary<string,IntPtr>();

		public override bool OnCreateOptionsMenu(IMenu menu) {

			MenuInflater.Inflate(Resource.Layout.main_activity_actions,menu);

			var linesItem = menu.Add ("Lines");
			linesItem.SetOnMenuItemClickListener (this);
			_ptrs ["lines"] = linesItem.Handle;

			var pathsItem = menu.Add ("paths");
			_ptrs ["paths"] = pathsItem.Handle;
			pathsItem.SetOnMenuItemClickListener (this);

			var grid = menu.Add ("Grid");
			_ptrs ["grid"] = grid.Handle;
			grid.SetOnMenuItemClickListener (this);

			return base.OnCreateOptionsMenu(menu);
		}

		public bool OnMenuItemClick (IMenuItem item)
		{
			var m = _ptrs.FirstOrDefault (p=> p.Value == item.Handle );
			if(m.Key == "grid") {
				_showGrid = !_showGrid;
				return true;
			}
			_mode = m.Key ?? "lines";
			return true;
		}

		#region Unused

		string GetNextStateDescription ()
		{

			switch (ActivityState.State) {
				case States.Stopped:
					return "Start";
				case States.Running:
					return "Freeze";
				case States.Paused:
					return "Continue";
			}

			throw new InvalidEnumArgumentException (
				"Dont' know what to do with {0}:{1}"
				.Formatify (ActivityState.State.ToString (), (int)ActivityState.State)
			);
		}




		bool PauseContinue ()
		{
			var changed = false;
			switch (ActivityState.State) {
				case States.Running:
					ActivityState.State = States.Paused;
					changed = true;
					break;
				case States.Paused:
					ActivityState.State = States.Running;
					changed = true;
					break;
			}
			return changed;
		}

		#endregion


	}

}

