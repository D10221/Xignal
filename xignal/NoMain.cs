using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Graphics;

using System.Reactive.Linq;
using Xignal.CanvasActions;
using Xignal.Render;
using XPoint = Xignal.XPoint<float,float>;
using XPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<float,float>>;

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

		readonly List<ISignalRender> SignalRenders;
		readonly List<ICanvasAction> CanvasActions;

		#region Constructor

		public NoMain ()
		{					
			                                   				
			SignalRenders = new List<ISignalRender> ();

			SignalRenders.Add (new LinesRender());
			SignalRenders.Add (new PathsRender());					

			CanvasActions = new List<ICanvasAction> ();
			CanvasActions.Add (new CanvasGrid ());
			CanvasActions.Add (new MaxLine ());
			CanvasActions.Add (new MinLine ());
			CanvasActions.Add (new AvgLine ());
								
		}

		#endregion

		PreProcessorType  PreProcessorType = PreProcessorType.Endless;

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


			Func<float> getSignal = () => {
				var halfHeight = ActivityState.Height / 2;
				var r = _random.Next (-halfHeight, halfHeight);
				return halfHeight + r;
			};
						
			var signalSource = Observable.Interval (TimeSpan.FromMilliseconds (ActivityState.SignalRate))
				.Select (x => getSignal());

			var timer = Observable.Interval (TimeSpan.FromMilliseconds (ActivityState.SamplingRate))
				.Select (x=> (int)x);

			var signalSources = timer
			              // Setup , dimensions 
				.Do (x => 
					ActivityState.SetDimensions (_surface.Width, _surface.Height))
			              //Merge Signal and TimeLine
				.Zip (signalSource, (x, y) => XPointFty.New (x,y));

			//IEnumerable<XPoint> points = new List<XPoint> ();
										
			_signalPreProcessor =  PreProcessorFty.Create (_signalPreProcessor,PreProcessorType);

			subscribe = () => {
				//points = new List<XPoint>();
				if(_subscription!=null ) _subscription.Dispose ();
				_subscription = signalSources
					.TakeWhile (x => ActivityState.State == States.Running)	
					//Arrange
					.Select ( rawSignal=> {				
						return _signalPreProcessor.Arrange (ActivityState, rawSignal).ToArray ();
					})					
					.Subscribe (xpoints=>{
					
						var canvasActions = new List<Action<Canvas>>();

						//Before Signal
						canvasActions.AddRange (CanvasActions
							.Where (plugin=> plugin.IsEnabled && plugin.Order == CanvasActionOrder.BeforeSignal)
							.Select(plugin=> plugin.GetAction(xpoints,ActivityState)));

						// Highlander principle : Only One
						canvasActions.Add (SignalRenders.FirstOrDefault (x=> x.IsEnabled).GetAction(xpoints,ActivityState));

						//After Signal
						canvasActions.AddRange (CanvasActions
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
									
		}
		ISignalPreProcessor _signalPreProcessor;


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
			
		void SetStartSTopIcon(IMenuItem item){
			item.SetIcon (ActivityState.State.IsStopped ()
				? Resource.Drawable.ic_action_play 
				:Resource.Drawable.ic_action_pause);
		}

		public override bool OnCreateOptionsMenu(IMenu menu) {

			MenuInflater.Inflate(Resource.Layout.main_activity_actions,menu);

			menu.FindItem (Resource.Id.action_play)
				.DoWith(SetStartSTopIcon)
				.SetOnMenuItemClickListener ( 
					new MenuItemClickListener (
						item =>{ 
							StartStop ();
							SetStartSTopIcon(item);
						}
					)
			);
								
			foreach(var plugin in SignalRenders){
				menu.Add (plugin.Title).OnClick (
					item => SignalRenders.Enable (plugin)
				);
			}

			foreach(var plugin in CanvasActions){
				menu.Add (plugin.Title).OnClick (item => 
					plugin.IsEnabled = !plugin.IsEnabled
				);
			}
			menu.Add ("ByFrame").OnClick (item => {

				if(PreProcessorType == PreProcessorType.Endless ){
					PreProcessorType = PreProcessorType.ByFrame;
					item.SetTitle ("EndLess");
				}else{
					PreProcessorType = PreProcessorType.Endless;
					item.SetTitle ("ByFrame");
				}
				_signalPreProcessor = PreProcessorFty.Create (_signalPreProcessor, PreProcessorType);
			});
												
			return base.OnCreateOptionsMenu(menu);
		}						
	}

	public static class ObservableXPointExtensions{

		public static IObservable<XPoints> Arrange(
			this IObservable<XPoint> incoming,
			Func<IObservable<XPoint>,
			IObservable<XPoints>> transform){
			return transform (incoming);
		}

		public static XPoints Arrange(
			this XPoint incoming,
			Func<XPoint,XPoints>  transform
		){
			return transform (incoming);
		}
			

	}

}

