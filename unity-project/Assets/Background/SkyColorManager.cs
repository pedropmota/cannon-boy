using UnityEngine;
using System.Collections;

public class SkyColorManager : MonoBehaviour {

	public Sprite daylightSky;
	public Sprite sunsetSky;
	public Sprite darkSky;
	public Sprite sunriseSky;

	private int scoreToChangeSky = 20;

	private bool changedSky = false;

	// Use this for initialization
	void Start () {
		GameEventManager.GameStart += GameStart;

		GameStart ();
	}

	private void GameStart() {
		GetComponent<SpriteRenderer>().sprite = this.daylightSky;
	}

	// Update is called once per frame
	void Update () {
		ChangeSky ();
	}



	private void ChangeSky() {

		var score = ScoreManager.Score;

		if (score == 0) {
			return;
		}

		if (score % scoreToChangeSky != 0) {
			changedSky = false;
			return;
		}

		if (!changedSky) {

			var spriteRenderer = GetComponent<SpriteRenderer> ();

			if (spriteRenderer.sprite.name == this.daylightSky.name)
				spriteRenderer.sprite = this.sunsetSky;
			else if (spriteRenderer.sprite.name == this.sunsetSky.name)
				spriteRenderer.sprite = this.darkSky;
			else if (spriteRenderer.sprite.name == this.darkSky.name)
				spriteRenderer.sprite = this.sunriseSky;
			else if (spriteRenderer.sprite.name == this.sunriseSky.name)
				spriteRenderer.sprite = this.daylightSky;

			changedSky = true;
			
		} 
	}

	void OnDestroy ()
	{
		GameEventManager.GameStart -= GameStart;
	}
}
