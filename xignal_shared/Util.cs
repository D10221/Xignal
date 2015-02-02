using System;
using System.Collections.Generic;
using System.Linq;
using XPoint = Xignal.XPoint<float,float>;

namespace Xignal
{
	public class Util{

		ActivityState ActivityState {get; set;}

		public Util (ActivityState activityState)
		{
			ActivityState = activityState;
		}
			
		public IEnumerable<XPoint> ToPoints(XPoint point){
			{
				var allPoints = ActivityState.AllPoints;
				//var _step = ActivityState.Step;
				var _steps = ActivityState.Steps;
				var _width = ActivityState.Width;
				var _state = ActivityState.State;

				if (ActivityState.State!= States.Paused) {				
					var xlist = allPoints.ToList ();
					xlist.Add (point);
					allPoints = xlist.ToArray ();
				}

				var i = allPoints.Count() - _steps;
				var last =  i > 0 ? allPoints.Last().X : _width ;
				var xfirst = i > 0 ? allPoints[i].X : 0 ;

				var ret = allPoints
					.Select(p =>  i > 0  && _state!= States.Paused ? new XPoint (p.X - _width , p.Y): p )
					.Where (x => 
						x.X >= 0 && x.X <= _width 
						&& x.X >= xfirst && x.X <= last
					)
					.ToArray ();;

				return ret;
			}
		}
	}
		



}

