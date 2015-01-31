// XPoint.cs
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

	public class XPoint{

		public float X {get;private set;}
		public float Y {get;private set;}

		public XPoint(float x ,float y  ){
			X = x;
			Y = y;
		}
		public override string ToString(){
			return "{" + " X: {0} ; Y: {1} ".Formatify(X,Y) +"}";
		}
		public bool IsEmpty { get {return Equals (X, 0) && Equals (Y, 0); } }
	}
	
}
