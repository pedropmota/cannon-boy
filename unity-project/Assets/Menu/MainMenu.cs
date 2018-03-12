using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	void Awake() {
		Application.targetFrameRate = 60;
	}

	// Use this for initialization
	void Start () {
		if (!Social.localUser.authenticated)
			Social.localUser.Authenticate ((bool ok) => { });
	}

}
