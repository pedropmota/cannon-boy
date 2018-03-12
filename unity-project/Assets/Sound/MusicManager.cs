using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour 
{
	public static MusicManager Instance;
	
	void Awake()
	{
		if (!Instance) 
		{
			DontDestroyOnLoad (gameObject);
			Instance = this;
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}
}
