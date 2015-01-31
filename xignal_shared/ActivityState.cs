// ActivityState.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace Xignal
{

	public class ActivityState
	{
		readonly ReplaySubject<PropertyChange> _subject;

		public IObservable<PropertyChange> Changed {get; private set;}

		public ActivityState ()
		{
			allPoints = new XPoint[0];
			_subject = new ReplaySubject<PropertyChange> ();
			Changed = _subject.AsObservable ();
		}
		
		public XPoint[] allPoints { get; set;}

		int _width;

		public void SetDimensions (int width, int height)
		{
			if (width != 0 && height != 0 
				&& (Width == 0 && Height == 0)
			) {
				Width = width;
				Height = height;
				Step = Width / Scale;
				Steps = Width / Step;
				HalfHeight = Height / 2;
				VStep = Height / Scale;
				VSteps = Height / VStep;
				if ((int)GridFactor == 0)
					GridFactor = 1;
			}

		}

		public bool IsScreenFull(Signal x) { 
			return x.X > Steps || Steps == 0 ; 
		}
		public bool IsThereRoomFor(Signal x) { return ! IsScreenFull (x) ; }

		public int Width {
			get{
				return _width;
			}
			set {
				if (Equals (value, _width))	return;
				_width = value;
			}
		}
		States _state;
		public States State {
			get {
				return _state;
			}
			set {
				if (_state == value) return;
				_state = value;
				NotifyChanged (value);
			}
		}

		void NotifyChanged (States value,[CallerMemberName] string name = null )
		{
			_subject.OnNext (new PropertyChange(name,value));
		}

		public int Scale {get;set;}
		public int SamplingRate {get;set;} 
		public int Height {get;set;} 
		public int SignalRate {get;set;} 
		public int Step {get;set;} 
		public int Steps {get;set;} 
		public int VStep {get;set;} 
		public int VSteps {get;set;} 
		public int HalfHeight {get;set;} 
		public float GridFactor {get;set;}

	}
	
}
