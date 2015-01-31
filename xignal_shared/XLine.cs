using System;
using Xignal;

namespace Xignal
{

	public class XLine{
		public float XStart { get; set;}
		public float YStart { get; set;}
		public float XEnd { get; set;}
		public float YEnd { get; set;}
		public XLine (float xStart, float yStart, float xEnd, float yEnd)
		{
			XStart = xStart;
			YStart = yStart;
			XEnd = xEnd;
			YEnd = yEnd;
		}			
	}

	public class HLine: XLine{
		public HLine (float width,float yPos) 
			: base (0, yPos, width, yPos)
		{

		}
		
	}


}
