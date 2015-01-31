// Extensions.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using Android.Widget;
using Android.Views;
using TouchEventArgs = Android.Views.View.TouchEventArgs;

namespace Xignal
{

	public static class Extensions{

		public static string Formatify(this string s, params object[] parameters){
			return String.Format(s,parameters);
		}

		public static string AsStringValues(this IEnumerable<string> sss ){
			return sss.Aggregate((a,b)=> a+","+b);
		}

		public static IEnumerable<IEnumerable<T>> GroupByNext<T>(
			this IEnumerable<T> source)
		{
			using (var e = source.GetEnumerator())
			{
				if (e.MoveNext())
				{
					var list = new List<T> { e.Current };
					while (e.MoveNext())
					{                                        
						list.Add(e.Current);
						yield return list;
						list = new List<T> { e.Current };                    
					}                
				}
			}
		}

		public static IEnumerable<int> AsRange(this int max, int @from = 0 ){
			for(var i = @from ; i <= max ; i++)
				yield return i;		
		}

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
