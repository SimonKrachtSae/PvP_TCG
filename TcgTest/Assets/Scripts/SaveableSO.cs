using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class SaveableSO : ScriptableObject
{
	List<object> values = new List<object>();
	string path { get => Application.persistentDataPath + "/SaveData/" + name + ".fun"; }

	private void OnEnable()
	{ 
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(path, FileMode.Open);
		SaveableSO saveable = this;
		//(SaveableSO) this = (SaveableSO)formatter.Deserialize(stream);
		stream.Close();
	}
    protected void OnValidate()
    {
		if (File.Exists(path)) File.Delete(path);
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(path, FileMode.Create);
		formatter.Serialize(stream, this);
		stream.Close();
	}
}
