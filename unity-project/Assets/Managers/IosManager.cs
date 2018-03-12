using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using GooglePlayGames;

public class IosManager {

	public static IEnumerator ShareImage(int score) {

		yield return new WaitForEndOfFrame();
		/*
		Texture2D texture = new Texture2D(Screen.width, Screen.height);
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		texture.Apply();

		var bytes = texture.EncodeToPNG ();

		Object.Destroy(texture);

		string path = Application.persistentDataPath + "/cannonball-bestscore.png";

		File.WriteAllBytes(path, bytes);

		string shareMessage = string.Format("Wow!! I just scored {0} on Cannonball Boy! Do you think you can beat me?", score);

		sampleMethod (path, shareMessage); 
		*/

	}



	//[DllImport("__Internal")]
	//private static extern void sampleMethod (string iosPath, string message);

	//[DllImport("__Internal")]
	//private static extern void sampleTextMethod (string message);


}
