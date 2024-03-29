using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<string, float>, ISerializationCallbackReceiver
{
    //Sometimes one of these doesn't show the values in inspector properly, idk why. so I duped the keys so at least one shows up.
    [SerializeField] private List<string> keys2 = new List<string>();
    [SerializeField] private List<string> keys = new List<string>();
	[SerializeField] private List<float> values = new List<float>();
	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys2.Clear();
        keys.Clear();
		values.Clear();
		foreach(KeyValuePair<string, float> pair in this)
		{
			keys2.Add(pair.Key);
            keys.Add(pair.Key);
            values.Add(pair.Value);
		}
	}
	
	// load dictionary from lists
	public void OnAfterDeserialize()
	{
		this.Clear();

		if(keys.Count != values.Count)
        {
			throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
        }
		for(int i = 0; i < keys.Count; i++)
		{
            this.Add(keys[i], values[i]);
        }
    }
}