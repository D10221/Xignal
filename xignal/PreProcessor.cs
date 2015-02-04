// PreProcessor.cs
//
// Author:
//       Daniel <>
//
// Copyright (c) 2015 Daniel

using System;
using System.Collections.Generic;
using System.Linq;

using XPoint = Xignal.XPoint<float,float>;
using XPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<float,float>>;
using RPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<int,float>>;
using RPoint = Xignal.XPoint<int,float>;

namespace Xignal
{

	class PreProcessor : ISignalPreProcessor
	{

		public Guid Identifier  { get; private set; }

		public List<RPoint> Points { get; private set;}

		readonly Func<ActivityState,RPoints,RPoint,XPoints> _arranger;

		public PreProcessor(Func<ActivityState,RPoints,RPoint,XPoints> arranger,IEnumerable<RPoint> points = null){

			Identifier = Guid.NewGuid ();

			Points = points != null
				? points.ToList () 
				: new List<RPoint> ();
				
			_arranger = arranger;
		}

		public 	XPoints Arrange(ActivityState context , RPoint rawSignal){
		
			return _arranger (context,Points,rawSignal);
		}

	}
	
}
