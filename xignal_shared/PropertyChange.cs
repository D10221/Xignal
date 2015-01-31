
namespace Xignal
{
	public class PropertyChange
	{
		public string Name {get; private set;}
		public object Value {get; private set;}
		public PropertyChange (string name, object value)
		{
			Name = name;
			Value = value;
		}
		
	}
		
}

