using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace com.rater193.twodspacefun
{
	public class JsonArray
	{
		// Internal list to hold array elements
		private List<object> data = new List<object>();

		// Add method to add an element to the array
		public JsonArray Add(object value)
		{
			data.Add(value);
			return this;
		}

		// Generic Get method to retrieve an element by index
		public T Get<T>(int index)
		{
			if (index >= 0 && index < data.Count)
			{
				object value = data[index];

				// Handle nested JsonObject or JsonArray deserialization
				if (typeof(T) == typeof(JsonObject) && value is JObject jObject)
				{
					return (T)(object)JsonObject.FromJObject(jObject);
				}
				else if (typeof(T) == typeof(JsonArray) && value is JArray jArray)
				{
					return (T)(object)FromJArray(jArray);
				}

				return (T)value;
			}
			throw new Exception($"Index '{index}' is out of range.");
		}

		// Convert JsonArray to JSON string using Json.NET
		public string ToJson()
		{
			List<object> serializableData = new List<object>();

			foreach (var value in data)
			{
				if (value is JsonObject nestedObject)
				{
					serializableData.Add(nestedObject.ToDictionary());
				}
				else if (value is JsonArray nestedArray)
				{
					serializableData.Add(nestedArray.ToList());
				}
				else
				{
					serializableData.Add(value);
				}
			}

			return JsonConvert.SerializeObject(serializableData, Formatting.Indented);
		}

		// Deserialize a JSON string into a JsonArray
		public static JsonArray FromJson(string jsonString)
		{
			JsonArray jsonArray = new JsonArray();
			jsonArray.data = JsonConvert.DeserializeObject<List<object>>(jsonString);
			return jsonArray;
		}

		// Convert JArray to JsonArray
		public static JsonArray FromJArray(JArray jArray)
		{
			JsonArray jsonArray = new JsonArray();
			jsonArray.data = jArray.ToObject<List<object>>();
			return jsonArray;
		}

		// Convert JsonArray to a list for serialization
		public List<object> ToList()
		{
			List<object> list = new List<object>();

			foreach (var value in data)
			{
				if (value is JsonObject nestedObject)
				{
					list.Add(nestedObject.ToDictionary());
				}
				else if (value is JsonArray nestedArray)
				{
					list.Add(nestedArray.ToList());
				}
				else
				{
					list.Add(value);
				}
			}

			return list;
		}
	}

}
