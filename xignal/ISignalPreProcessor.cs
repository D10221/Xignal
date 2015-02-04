using System;
using System.Collections.Generic;

using XPoint = Xignal.XPoint<float,float>;
using XPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<float,float>>;
using RPoint = Xignal.XPoint<int,float>;

namespace Xignal
{
	interface ISignalPreProcessor
	{
		Guid Identifier  { get; }
		List<RPoint> Points { get; }
		XPoints Arrange (ActivityState context, RPoint rawSignal);
	}	


}

