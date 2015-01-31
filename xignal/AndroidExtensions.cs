// AndroidExtensions.cs
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
	public static class AndroidExtensions{

		public static IObservable<EventPattern<EventArgs>> Clicks(this Button button){
			return Observable.FromEventPattern (x => button.Click += x, x => button.Click -= x);
		}
		//LongClick
		public static IObservable<EventPattern<Android.Views.View.LongClickEventArgs>> LongClicks(this Button button){
			return Observable.FromEventPattern<Android.Views.View.LongClickEventArgs>(x => button.LongClick += x, x => button.LongClick -= x);
		}

		public static IObservable<EventPattern<TouchEventArgs>> Touchs(this View view){
			return Observable.FromEventPattern<TouchEventArgs> (x => view.Touch += x, x => view.Touch -= x);
		}

		public static IObservable<EventPattern<EventArgs>> Clicks(this View view){
			return Observable.FromEventPattern (x => view.Click += x, x => view.Click -= x);
		}
		//LongClick
		public static IObservable<EventPattern<Android.Views.View.LongClickEventArgs>> LongClicks(this View view){
			return Observable.FromEventPattern<Android.Views.View.LongClickEventArgs>(x => view.LongClick += x, x => view.LongClick -= x);
		}
							
	}		
	
}
