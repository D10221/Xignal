// XPoint.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

namespace Xignal
{

	class XPoint<T>{

		public T X {get;private set;}
		public T Y {get;private set;}

		public XPoint(T x ,T y  ){
			X = x;
			Y = y;
		}
		public override string ToString(){
			return "{" + " X: {0} ; Y: {1} ".Formatify(X,Y) +"}";
		}
		public bool IsEmpty { get {return Equals (X, default(T)) && Equals (Y, default(T)); } }
	}

}
