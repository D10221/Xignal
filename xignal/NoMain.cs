using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Graphics;

using System.Reactive.Linq;
using System.ComponentModel;
using Xignal.CanvasActions;
using Xignal.Render;


namespace Xignal
{
	[Activity (Label = "Xignal", MainLauncher = true, Icon = "@drawable/icon")]
	public class NoMain : Activity 
	{
		const string TAG = "NoMain";

		Xurface _surface;

		ActivityState ActivityState;
		readonly Random _random =  new Random();
		IDisposable _subscription ;
		Action subscribe ;




		List<ICanvasAction> ICanvasActions;

		#region Constructor

		public NoMain ()
		{						


			/*Func<XPoint[],Action<Canvas>> _renderLines = torender => new Action<Canvas> ((canvas) => {

				canvas.RotateY (ActivityState);

				foreach (var group in torender.GroupByNext ()) {
					var start = group.First ();
					var end = group.Last ();
					canvas.DrawLine (start.X, start.Y, end.X, end.Y, _signalPaint);
				}

				canvas.RotateY (ActivityState);

			});*/

			/*Func<XPoint[],Action<Canvas>> _renderPaths = xpoints => new Action<Canvas> ((canvas) => {

				canvas.RotateY (ActivityState);

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
				canvas.DrawPath (path, _signalPaint);

				canvas.RotateY (ActivityState);
			}
		);
		*/
			                                   
				
			SignalRenders = new List<ISignalRender> ();

			SignalRenders.Add (new LinesRender());
			SignalRenders.Add (new PathsRender());
					

			ICanvasActions = new List<ICanvasAction> ();
			ICanvasActions.Add (new CanvasGrid ());
			ICanvasActions.Add (new MaxLine ());
			ICanvasActions.Add (new MinLine ());
			ICanvasActions.Add (new AvgLine ());
								
		}

		#endregion

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			base.OnCreate (savedInstanceState);

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
					
						var canvasActions = new List<Action<Canvas>>();

						canvasActions.AddRange (ICanvasActions
							.Where (plugin=> plugin.IsEnabled && plugin.Order == CanvasActionOrder.BeforeSignal)
							.Select(plugin=> plugin.GetAction(xpoints,ActivityState)));

					//Onl;y One
						canvasActions.Add (SignalRenders.FirstOrDefault (x=> x.IsEnabled).GetAction(xpoints,ActivityState));

						canvasActions.AddRange (ICanvasActions
							.Where (plugin=> plugin.IsEnabled && plugin.Order == CanvasActionOrder.AfterSignal)
							.Select(plugin=> plugin.GetAction(xpoints,ActivityState)));

						using (var _canvas = new Canvas ()){																
							_surface.Drawit (_canvas,canvasActions.ToArray ());
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

			/*_surface.Touchs ()
				.Throttle (TimeSpan.FromMilliseconds (150))
				.Subscribe (e => StartStop());
			*/
									
		}


		void StartStop(){
			switch(ActivityState.State){
				case States.Running:
					ActivityState.State = States.Stopped;
					break;
				case States.Stopped:
					ActivityState.State = States.Running;
					break;
			}	
		}
			
		List<ISignalRender> SignalRenders;

		XPoint shifXtLeft (XPoint point){
			if (point == null)
				throw new ArgumentNullException ("point");
			return shifXtLeftBy (point, 1);
		}

		XPoint shifXtLeftBy (XPoint point, int howMany )
		{
			var p = new XPoint (point.X - ActivityState.Step*howMany, point.Y);
			return p;
		}			

		void SetStartSTopIcon(IMenuItem item){
			item.SetIcon (ActivityState.State.IsStopped ()
				? Resource.Drawable.ic_action_play 
				:Resource.Drawable.ic_action_pause);
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {

			MenuInflater.Inflate(Resource.Layout.main_activity_actions,menu);

			menu.FindItem (Resource.Id.action_play)
				.With(SetStartSTopIcon)
				.SetOnMenuItemClickListener ( 
					new MenuItemClickListener (
						item =>{ 
							StartStop ();
							SetStartSTopIcon(item);
						}
					)
			);
								
			foreach(var plugin in SignalRenders){
				menu.Add (plugin.Title).OnCLick (item => SignalRenders.Enable (plugin)
			);
		}


			foreach(var plugin in ICanvasActions){
				menu.Add (plugin.Title).OnCLick (item => 
					plugin.IsEnabled = !plugin.IsEnabled
				);
			}
				
			return base.OnCreateOptionsMenu(menu);
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

