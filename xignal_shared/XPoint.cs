// XPoint.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel
using System;

namespace Xignal
{

	public class XPoint<TX,TY>{

		public TX X {get;private set;}
		public TY Y {get;private set;}

		public XPoint(TX x ,TY y  ){
			X = x;
			Y = y;
		}
		public override string ToString(){
			return "{" + " X: {0} ; Y: {1} ".Formatify(X,Y) +"}";
		}
		public bool IsEmpty { get {return Equals (X, 0) && Equals (Y, 0); } }
	}

	public static class XPointFty{
		public static XPoint<TX,TY> New<TX,TY>(TX x , TY y){
			return new XPoint<TX,TY> (x, y);
		}
	}




}
