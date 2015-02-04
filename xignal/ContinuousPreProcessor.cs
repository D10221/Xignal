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
using RPoint = Xignal.XPoint<int,float>;
using RPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<int,float>>;


namespace Xignal
{

	class ContinuousPreProcessor: ISignalPreProcessor{
	
		public List<RPoint> Points { get; private set;}

		public Guid Identifier  { get; private set; }

		public ContinuousPreProcessor( RPoints points = null){

			Identifier = Guid.NewGuid ();

			Points = points != null
				? points.ToList () 
				: new List<RPoint> ();
				
		}

		public 	XPoints Arrange(ActivityState context, RPoint rawSignal){

			var step = context.Step;

			var last = context.Steps + 1;

			var m = rawSignal.X % (last); 

			var next = Points.Count + 1;

			var index = next == m+1 ? m : next;

			var skip = rawSignal.X / last != 0 ;

			if(skip)
			Points = Points
				//.Skip (skip ? 1: 0)
				.Select(x=>  new RPoint(x.X - 1 , x.Y))
				//.Take(last)
				.Where (x=> x.X >=0 )
				.ToList ();		

			Points.Add (new RPoint( skip? context.Steps: index , rawSignal.Y));

			//Scale
			return Points.Select(x=> new XPoint(x.X*step ,  x.Y)).ToArray ();
		}
			
		XPoint Expand(ActivityState  context, XPoint rawSignal){
			return XPointFty.New (rawSignal.X * context.Step, rawSignal.Y);
		}
			
	}



	static class PreProcessorFty{

		static List<RPoint> Points ;

		public static ISignalPreProcessor Create(ISignalPreProcessor previous,PreProcessorType type= PreProcessorType.Endless){

			if (previous != null)
				Points = previous.Points;

			switch(type){

				case PreProcessorType.ByFrame:

					return new PreProcessor ((context,xpoints,rawSignal) => {

						var last = context.Steps + 1;

						var m = rawSignal.X % (last); 

						var next = Points.Count + 1 <= last ? Points.Count + 1 : 0 ;

						var index = next == m+1 ? m : next;					

						if (index == 0)
							Points = new List<RPoint> ();
							
						Points.Add (new RPoint(index, rawSignal.Y));

						//Scale
						return Points.Select( x => { 

								var step = (float)((x.X) * context.Step);

								return XPointFty.New (step, x.Y);
							}
						).ToArray ();

					},Points);

				case PreProcessorType.Endless:
					return new ContinuousPreProcessor (Points);
			}

			throw new ArgumentException ("unkwon Preprocessor Type");

		}
	}

	enum PreProcessorType{
		Endless, ByFrame
	}
}
