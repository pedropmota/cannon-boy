using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SocialPlatforms.GameCenter;

public class MenuButtonsListener : MonoBehaviour {

	//___ Unity object properties:

	public bool mainMenu;

	//___


	private static MenuButtonsListener instance;

	public static void SetEnabled(bool enabled) {
		instance.enabled = enabled;
	}

	// Use this for initialization
	void Start () {
		instance = this;

		if (!mainMenu) {
			SetEnabled (false);
		} else {
			SetSoundOption(SoundManager.GetCurrSoundOption ());
			//SetSoundOption(SoundManager.SoundOption.MusicAndFX);
		}
	}
	
	void OnDestroy () {
		Destroy (instance);
	}

	void Update () {

		string pressedButton = null;

		if (TouchManager.Tapped) {
			pressedButton = getPressedButton(TouchManager.TapPosition);
		}

		if (pressedButton != null) {
			//Debug.Log(pressedButton);

			switch(pressedButton) {
				case "play-button":

					if (Application.loadedLevelName == "MainMenu") {
						FindObjectOfType<SceneFadeInOut>().FadeOutScene("GameScene");
					}
					if (Application.loadedLevelName == "GameScene") {
						FindObjectOfType<SceneFadeInOut>().FadeOutThenFadeIn(() => GameEventManager.TriggerGameStart ());
					}
					break;

				case "rate-button":
					

					#if UNITY_ANDROID
					Application.OpenURL("https://play.google.com/store/apps/details?id=com.pedropmota.cannonboy");
					#elif UNITY_IPHONE
					Application.OpenURL("itms-apps://itunes.apple.com/app/id1054132769");;
					#endif

					break;

				case "removeads-button":
					break;

				case "leaderboard-button":
					ScoreManager.OpenLeaderboard();
					break;

				case "home-button":
					FindObjectOfType<SceneFadeInOut>().FadeOutScene("MainMenu");

					break;	

				case "share-button":

					//StartCoroutine(IosManager.ShareImage(ScoreManager.score));
					break;

				case "sound1":
					SoundManager.SetSoundOption(SoundManager.SoundOption.FXOnly);
					SetSoundOption(SoundManager.SoundOption.FXOnly);
					break;

				case "sound2":
					SoundManager.SetSoundOption(SoundManager.SoundOption.MusicOnly);
					SetSoundOption(SoundManager.SoundOption.MusicOnly);
				
					break;

				case "sound3":
					SoundManager.SetSoundOption(SoundManager.SoundOption.Off);
					SetSoundOption(SoundManager.SoundOption.Off);
					break;

				case "sound4":
					SoundManager.SetSoundOption(SoundManager.SoundOption.MusicAndFX);
					SetSoundOption(SoundManager.SoundOption.MusicAndFX);
					break;
			}
		}

	}

	private void SetSoundOption(SoundManager.SoundOption soundOption) {

		MakeSoundOptionVisible (soundOption);

		//Turn music on or off:
		if (soundOption == SoundManager.SoundOption.FXOnly ||
			soundOption == SoundManager.SoundOption.Off) {
			
			FindObjectOfType<MusicManager> ().GetComponent<AudioSource>().enabled = false;
		} else {

			FindObjectOfType<MusicManager>().GetComponent<AudioSource>().enabled = true;
		}
		
	}

	private void MakeSoundOptionVisible(SoundManager.SoundOption option) {

		var sprites = FindObjectsOfType<SpriteRenderer> ()
			.Where(s => s.name.StartsWith("sound"))
				.OrderBy(s => s.name)
				.ToArray();
		
		foreach (var item in sprites) {
			item.enabled = false;
			item.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		}

		SpriteRenderer spriteToShow = null;

		switch (option) {
			
		case SoundManager.SoundOption.MusicAndFX:
			spriteToShow = sprites[0];
			break;
			
		case SoundManager.SoundOption.FXOnly:
			spriteToShow = sprites[1];
			break;
			
		case SoundManager.SoundOption.MusicOnly:
			spriteToShow = sprites[2];
			break;
			
		case SoundManager.SoundOption.Off:
			spriteToShow = sprites[3];
			break;
		}

		spriteToShow.enabled = true;
		spriteToShow.GetComponent<BoxCollider2D>().enabled = true;
	}

	string getPressedButton(Vector3 position) {

		Vector2 worldPoint = Camera.main.ScreenToWorldPoint(position);
		
		Collider2D[] col = Physics2D.OverlapPointAll(worldPoint);
		
		if (col.Length > 0) {
			return col [0].GetComponent<Collider2D> ().gameObject.name;
		} else {
			return null;
		}

	}
}
