using UnityEngine;
using System.Collections;

/// <summary>
/// Improves the on-screen text sizes for multiple platforms
/// </summary>
public class GUIOptimizer : MonoBehaviour {

	void Start () {
		SetupGUIScaling ();
	}

	void SetupGUIScaling() {
		//Base value of resolution screen for which the text is made
		float origW = 1200.0f;
		float origH = 720.0f;
		
		float scaleX = (float)(Screen.width) / origW; //your scale x
		float scaleY = (float)(Screen.height) / origH; //your scale y
		//Find all GUIText object on your scene
		GUIText[] texts =  FindObjectsOfType<GUIText>(); 
		foreach (GUIText myText in texts) { //find your element of text
			Vector2 pixOff = myText.pixelOffset; //your pixel offset on screen
			int origSizeText = myText.fontSize;
			myText.pixelOffset = new Vector2 (pixOff.x * scaleX, pixOff.y * scaleY); //new position
			myText.fontSize = (int)(origSizeText * scaleX); //new size font
		}
		
	}
}
