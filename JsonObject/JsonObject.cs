namespace com.rater193.twodspacefun
{
	using System;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	public class JsonObject
	{
		// Internal dictionary to hold key-value pairs
		private Dictionary<string, object> data = new Dictionary<string, object>();

		// Set method to add or update a key-value pair
		public JsonObject Set(string key, object value)
		{
			// Directly store the JsonObject reference instead of converting to a dictionary
			data[key] = value;
			return this;
		}

		// Generic Get method to retrieve values by key
		public T Get<T>(string key)
		{
			if (data.ContainsKey(key))
			{
				object value = data[key];

				// Handle nested JsonObject deserialization
				if (typeof(T) == typeof(JsonObject) && value is JObject jObject)
				{
					return (T)(object)FromJObject(jObject);
				}
				else if (typeof(T) == typeof(JsonArray) && value is JArray jArray)
				{
					return (T)(object)JsonArray.FromJArray(jArray);
				}

				// Return the value if the type matches
				return (T)value;
			}
			throw new Exception($"Key '{key}' not found in JsonObject.");
		}

		// Convert JsonObject to JSON string using Json.NET
		public string ToJson()
		{
			Dictionary<string, object> serializableData = new Dictionary<string, object>();

			foreach (var pair in data)
			{
				// If the value is another JsonObject, call its ToDictionary for serialization
				if (pair.Value is JsonObject nestedObject)
				{
					serializableData[pair.Key] = nestedObject.ToDictionary();
				}
				else if (pair.Value is JsonArray nestedArray)
				{
					serializableData[pair.Key] = nestedArray.ToList();
				}
				else
				{
					serializableData[pair.Key] = pair.Value;
				}
			}

			return JsonConvert.SerializeObject(serializableData, Formatting.Indented);
		}

		// Deserialize a JSON string into a JsonObject
		public static JsonObject FromJson(string jsonString)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
			return jsonObject;
		}

		// Convert JObject to JsonObject
		public static JsonObject FromJObject(JObject jObject)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.data = jObject.ToObject<Dictionary<string, object>>();
			return jsonObject;
		}

		// Convert JsonObject to a dictionary for serialization
		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();

			foreach (var pair in data)
			{
				if (pair.Value is JsonObject nestedObject)
				{
					// Recursively convert nested JsonObject to a dictionary
					dict[pair.Key] = nestedObject.ToDictionary();
				}
				else if (pair.Value is JsonArray nestedArray)
				{
					dict[pair.Key] = nestedArray.ToList();
				}
				else
				{
					dict[pair.Key] = pair.Value;
				}
			}

			return dict;
		}
	}
}
