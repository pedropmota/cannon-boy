using UnityEngine;
using System.Collections;

/*
	Unity configuration notes:
	
	GravityScale and difficulty (always with mass being = 1)

	* Very hard but still fun to play: 1.8 gravity
	* 10% less hard: 1.65 gravity


	Flying speed:

	* Fast and very well tested: 1000 flying speed / 0.4 flying gravity
 */

/// <summary>
/// The main (and only) character!
/// </summary>
public class Player : MonoBehaviour {

	//___ Unity object properties:

	public Transform explosion;

	public Vector2 fireVelocity;
	public float flyingGravity;

	//___

	public static Vector3 Position { get; private set; }
	public static Vector3 StartPosition { get; private set; }

	public static bool IsInCannon { get; private set; }

	private const float explosionAnimationSpeed = 1.3f;
	//private float explosionAnimationTime = 0.7f; //Time of the animation with speed 1.0
	private const float explosionTime = 0.5f;

	private GameObject currentCannon;
	private float startGravity;
	private float currentGravity;
	private bool fireButtonPressed;

	/// <summary>
	/// True if entered the first cannon (gravity can only start after that first collision)
	/// </summary>
	private bool enteredFirstCannon;

	private bool isDead;


	void Start () {
		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;

		StartPosition = Vector3.zero;
		startGravity = GetComponent<Rigidbody2D> ().gravityScale;
		currentGravity = startGravity;
		GetComponent<Rigidbody2D> ().isKinematic = true;

		explosion = Instantiate (explosion);
		explosion.GetComponent<Animator> ().speed = explosionAnimationSpeed;

		explosion.transform.position = new Vector2 (-100, -100);
	
		GameStart ();
	}

	void OnDestroy() {
		GameEventManager.GameStart -= GameStart;
		GameEventManager.GameOver -= GameOver;
	}

    void GameStart () {
		enabled = true;

		isDead = false;

		enteredFirstCannon = false;

		transform.position = StartPosition;

		currentGravity = startGravity;

		GetComponent<Rigidbody2D> ().isKinematic = false;
	}
	
    void GameOver () {

		if (this.currentCannon != null) {
			this.currentCannon.transform.parent = null;
			this.currentCannon.GetComponent<SpriteRenderer> ().enabled = true;
			this.currentCannon.SetActive(true);
		}

		GetComponent<Rigidbody2D> ().isKinematic = true;

		enabled = false;
	}


	void Update () {
		Position = transform.position;

		if(IsInCannon && TouchManager.Tapped){
			fireButtonPressed = true;
		}

		if (!isDead)
			CheckIfDied ();
	}



	void FixedUpdate () {

		if(fireButtonPressed){
			FireFromCannon();
			fireButtonPressed = false;
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		EnterCannon (coll.gameObject);
	}
	
	void OnCollisionExit2D () {

	}

	private void EnterCannon(GameObject cannon) {
		this.currentCannon = cannon;
		transform.position = cannon.transform.position;
		cannon.transform.parent = this.transform;
		cannon.SetActive (false);
		cannon.GetComponent<SpriteRenderer> ().enabled = false;

		this.GetComponent<Animator> ().Play ("in cannon");

		GetComponent<Rigidbody2D> ().gravityScale = currentGravity;

		if (!enteredFirstCannon) {
			StopGravity ();
			enteredFirstCannon = true;
		}
		else {
			ScoreManager.AddPoint();

			SoundManager.PlaySuck();

			IncreaseDifficulty();
		}

		IsInCannon = true;
	}

	private void StopGravity () {
		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		GetComponent<Rigidbody2D>().gravityScale = 0;
	}

	private void IncreaseDifficulty() {

		//Decrease divisor to increase difficulty faster:

		//sum => 0.05
		//0.027 takes gravity from 1.1 to 1.65 in 50 points; 0.022 makes it in 30 points.
		//0.024 takes gravity from 1.25 to 1.8 in 30 points.

		//sum => 0.04
		//0.024 takes gravity from 1.2 to 1.54 in 50 points, but doesn't increase much after that.
		//0.023 doesn't increase very fast and still can reach 1.6 to 1.7 at high scores like 100 to 300.
		//0.022 reaches 1.6 in 50 points and will not pass 1.8.

		var sum = 0.05f;
		var divisor = 0.025f;

		currentGravity = (currentGravity + sum) - (currentGravity * divisor);

		/*
		 * JS code for testing and seing the progression of ranges easily on a browser console:
		var current = 1.2; 
		var sum = 0.04;
		var div = 0.024;
		for (var i = 1; i <= 30; i++) {
			current = (current + sum) - (current * div);
			console.log( current )
		}
		 * */

	}

	private void FireFromCannon() {

		if (this.currentCannon != null) {
			ShowExplosion (this.currentCannon.transform);
			this.currentCannon.SetActive (true);
			this.currentCannon.GetComponent<SpriteRenderer> ().enabled = true;
			this.currentCannon.transform.parent = null;

			//TODO take out hardcoded value
			transform.position = new Vector3 (transform.position.x + 1.3f,
			                                  transform.position.y,
			                                  transform.position.z);
		}

		SoundManager.PlayExplosion ();

		this.GetComponent<Animator> ().Play ("flying");

		GetComponent<Rigidbody2D>().velocity = Vector3.zero;
		GetComponent<Rigidbody2D>().gravityScale = flyingGravity;
		GetComponent<Rigidbody2D>().AddForce(fireVelocity, ForceMode2D.Force);

		this.currentCannon = null;

		IsInCannon = false;
	}

	private void ShowExplosion(Transform cannon) {

		explosion.transform.position = new Vector3(cannon.position.x + 0.1f,
		                                           cannon.position.y - 0.01f, //Adding some height to align explosion with cannon
		                                           -0.5f);

		explosion.GetComponent<Animator> ().Play ("explosion");
		
		StartCoroutine (WaitExplosionEnd (explosionTime));

	}

	private IEnumerator WaitExplosionEnd(float waitTime) {
		yield return new WaitForSeconds (waitTime);

		explosion.transform.position = new Vector2(-100, -100);
	}

	private void CheckIfDied() {

		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;

		//TODO: take out the hardcoded players size
		if (this.transform.position.x > cam.transform.position.x + (width / 2) + 2 ||
		    this.transform.position.y < cam.transform.position.y - (height / 2) - 2) {

			//Stop player or else he'll keep falling while waiting to die:
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			GetComponent<Rigidbody2D>().gravityScale = 0f;

			SoundManager.PlayFall();

			isDead = true;

			StartCoroutine(WaitThenDie());
		}

	}

	/// <summary>
	/// Waiting a little before showing the GameOver screen
	/// </summary>
	private IEnumerator WaitThenDie() {
		yield return new WaitForSeconds (2.2f);

		GameEventManager.TriggerGameOver ();
	}

	void OnGUI () {
		//GUI.Label (new Rect (300, 5, 300, 100), "current difficulty: " + currentGravity);

		//GUI.Label (new Rect (5, 5, 150, 100), 
		//           GetComponent<Rigidbody2D> ().velocity.ToString () + "\n" +
		//           GetComponent<Rigidbody2D> ().gravityScale.ToString());
	}
		 
}
