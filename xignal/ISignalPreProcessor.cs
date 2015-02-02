using System;
using System.Collections.Generic;

using XPoint = Xignal.XPoint<float,float>;
using XPoints = System.Collections.Generic.IEnumerable<Xignal.XPoint<float,float>>;

namespace Xignal
{
	interface ISignalPreProcessor
	{
		Guid Identifier  { get; }
		List<XPoint> Points { get; }
		XPoints Arrange (ActivityState context, XPoint rawSignal);
	}	


}

