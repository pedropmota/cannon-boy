using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	//___ Unity object properties:

	public GameObject tapIcon;

	public Sprite tapLabel;
	public Sprite fireLabel;

	//___


	private bool fired;

	private float changeTime = 0;

	private float initialX;
	private float xDifDistance = -1.5f;//1.05f; //Difference between the tap and fire labels x position

	// Use this for initialization
	void Start () {
		initialX = transform.position.x;

		GameEventManager.GameStart += GameStart;

		GameStart ();
	}

	void GameStart() {
		tapIcon.GetComponent<SpriteRenderer> ().enabled = true;

		this.GetComponent<SpriteRenderer> ().enabled = true;

		setTapLabel ();
		changeTime = (int)(Time.time * 0.9);

		fired = false;
	}

	void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
	}
	
	// Update is called once per frame
	void Update () {
		ChangeLabel ();
	}

	void ChangeLabel() {

		if (fired)
			return;

		if (TouchManager.Tapped) {
			fired = true;

			tapIcon.GetComponent<SpriteRenderer> ().enabled = false;
			
			this.GetComponent<SpriteRenderer> ().enabled = false;

			return;
		}

		if ((int)(Time.time * 0.9) > (int)changeTime) {

			if (this.GetComponent<SpriteRenderer>().sprite.name == tapLabel.name)
				setFireLabel();
			else
				setTapLabel();

			changeTime = (int)(Time.time * 0.9);

		}
	}

	void setTapLabel() {

		this.GetComponent<SpriteRenderer>().sprite = tapLabel;

		this.transform.position = new Vector2 (this.initialX, transform.position.y);
	}

	void setFireLabel() {
		this.GetComponent<SpriteRenderer>().sprite = fireLabel;

		this.transform.position = new Vector2 (this.initialX + this.xDifDistance, transform.position.y);
	}
}
