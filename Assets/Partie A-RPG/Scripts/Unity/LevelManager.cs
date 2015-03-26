using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager
{
	private static Dictionary<string, string> _parameters = new Dictionary<string, string>();
	public static object GetParameter(string key)
	{
		if (_parameters.ContainsKey(key))
			return _parameters[key];
		return null;
	}
	
	public static void SetParameter(string key, string value)
	{
		if (_parameters.ContainsKey(key))
			_parameters[key] = value;
		else
			_parameters.Add(key, value);
	}
	
	public static void ClearParameters()
	{
		_parameters.Clear();
	}
	
	public static void LoadLevel(string level)
	{
		LoadLevel(level, null);
	}
	
	public static void LoadLevel(string level, Dictionary<string, string> parameters)
	{
		if (parameters != null)
			_parameters = parameters;
		Application.LoadLevel(level);
	}
}