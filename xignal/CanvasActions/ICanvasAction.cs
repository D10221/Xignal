// Grid.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel
using System;
using Android.Graphics;

namespace Xignal.CanvasActions
{

	public interface ICanvasAction
	{
		CanvasActionOrder Order { get; }
		bool IsEnabled{ get; set;}
		string Name {get;}
		string Title {get;}
		Action<Canvas> GetAction (XPoint[] xpoints,ActivityState context);
	}

	public enum CanvasActionOrder {
		AfterSignal, //Default
		BeforeSignal
	}
		
}

