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

namespace Xignal
{

	class ContinuousPreProcessor: ISignalPreProcessor{
	
		public List<XPoint> Points { get; private set;}

		public Guid Identifier  { get; private set; }


		public ContinuousPreProcessor( IEnumerable<XPoint> points = null){

			Identifier = Guid.NewGuid ();

			Points = points != null
				? points.ToList () 
				: new List<XPoint> ();
				
		}

		public 	XPoints Arrange(ActivityState context, XPoint rawSignal){

			//_context = context;

			var point = Expand (context, Shift (context,rawSignal)  );

			Points.Add(point);

			return Points.ToArray ();
		}
			
		XPoint Shift(ActivityState context,XPoint rawSignal){
		
			if (context.IsThereRoomFor (rawSignal)) return rawSignal;

			Points = Points.Skip(1).Select(
				point => new XPoint (point.X - context.Step * 1, point.Y)
			).ToList ();

			return XPointFty.New((float)Points.Count , rawSignal.Y);
		}

		XPoint Expand(ActivityState  context, XPoint rawSignal){
			return XPointFty.New (rawSignal.X * context.Step, rawSignal.Y);
		}
			
	}


	class FramePreProcessor: ISignalPreProcessor
	{

		public Guid Identifier  { get; private set; }

		public List<XPoint> Points { get; private set;}


		public FramePreProcessor(IEnumerable<XPoint> points = null){

			Identifier = Guid.NewGuid ();

			Points = points != null
				? points.ToList () 
				: new List<XPoint> ();
											
		}

		public 	XPoints Arrange(ActivityState context , XPoint rawSignal){


			var index = (int) rawSignal.X % (context.Steps+1)  ;

			if(index == 0 )
				Points = new List<XPoint> ();
				
			var step = (float)((index) * context.Step);

			Points.Add(XPointFty.New (step , rawSignal.Y));

			return Points.ToArray ();

		}
			
	}


	class PreProcessor : ISignalPreProcessor
	{

		public Guid Identifier  { get; private set; }

		public List<XPoint> Points { get; private set;}

		readonly Func<ActivityState,XPoints,XPoint,XPoints> _arranger;

		public PreProcessor(Func<ActivityState,XPoints,XPoint,XPoints> arranger,IEnumerable<XPoint> points = null){

			Identifier = Guid.NewGuid ();

			Points = points != null
				? points.ToList () 
				: new List<XPoint> ();
				
			_arranger = arranger;
		}

		public 	XPoints Arrange(ActivityState context , XPoint rawSignal){
		
			return _arranger (context,Points,rawSignal);
		}

	}

	static class PreProcessorFty{

		static List<XPoint> Points ;

		public static ISignalPreProcessor Create(ISignalPreProcessor previous,PreProcessorType type= PreProcessorType.Endless){

			if (previous != null)
				Points = previous.Points;

			switch(type){
				case PreProcessorType.ByFrame:
					return new PreProcessor ((context,xpoints,rawSignal) => {

						var index = (int)rawSignal.X % (context.Steps + 1);

						if (index == 0)
							Points = new List<XPoint> ();

						var step = (float)((index) * context.Step);

						Points.Add (XPointFty.New (step, rawSignal.Y));

						return Points.ToArray ();

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
