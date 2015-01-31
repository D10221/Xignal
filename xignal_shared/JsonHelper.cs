using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Xignal
{
	public static class JsonHelper
	{
		/// <summary>
		/// JSON Serialization
		/// </summary>
		public static string ToJson<T>(this T t)
		{
			var ser = new DataContractJsonSerializer(typeof(T));

			string jsonString;

			using (var ms = new MemoryStream())
			{
				ser.WriteObject(ms, t);
				jsonString = Encoding.UTF8.GetString(ms.ToArray(),0,(int) ms.Length);
			}

			return jsonString;
		}
		/// <summary>
		/// JSON Deserialization
		/// </summary>
		public static T FromJson<T>(this string jsonString)
		{
			if (string.IsNullOrEmpty (jsonString))
				return default(T);

			//Debug.WriteLine ("Json: {0}", (object)jsonString);

			var ser = new DataContractJsonSerializer(typeof(T));
			var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
			var obj = (T)ser.ReadObject(ms);
			return obj;
		}
	}
}

