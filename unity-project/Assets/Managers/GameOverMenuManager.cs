using UnityEngine;
using System.Collections;

public class GameOverMenuManager : MonoBehaviour {

	//___ Unity object properties:

	public SpriteRenderer gameOverText, playButton, rateButton, leaderboardButton, homeButton, shareButton;

	public GUIText scoreText, scoreOutline;

	//___


	void Start () {
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;

		EnableOrDisableEverything (false);
	}

	void OnDestroy() {
	
		GameEventManager.GameStart -= GameStart;
		GameEventManager.GameOver -= GameOver;
	}

	private void GameStart () {

		MenuButtonsListener.SetEnabled (false);

		EnableOrDisableEverything (false);
	}
	
	private void GameOver () {

		MenuButtonsListener.SetEnabled (true);

		EnableOrDisableEverything (true);
	}

	private void EnableOrDisableEverything(bool enable) {
		gameOverText.enabled = enable;
		playButton.enabled = enable;
		rateButton.enabled = enable;
		leaderboardButton.enabled = enable;
		homeButton.enabled = enable;
		shareButton.enabled = enable;
		scoreText.enabled = enable;
		scoreOutline.enabled = enable;

		playButton.GetComponent<Collider2D>().enabled = enable;
		rateButton.GetComponent<Collider2D>().enabled = enable;
		leaderboardButton.GetComponent<Collider2D>().enabled = enable;
		homeButton.GetComponent<Collider2D>().enabled = enable;
		shareButton.GetComponent<Collider2D>().enabled = enable;
	}

}