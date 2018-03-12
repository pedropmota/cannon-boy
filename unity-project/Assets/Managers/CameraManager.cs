using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public static float CameraHeight { get; private set; }

	public static float CameraWidth { get; private set; }

	public static float StartPositionX { get; private set; }

	public static float StartPositionY { get; private set; }

	//___ Unity object properties:

	public float advancingSpeed;

	//___

	private Player player;

	private Vector3 advanceTo;


	void Start () {
		//Camera.main.orthographicSize = (Screen.height / 100f / 2.0f); // 100f is the PixelPerUnit that you have set on your sprite. Default is 100.

		StartPositionX = transform.position.x;
		StartPositionY = transform.position.y;

		GameEventManager.GameStart += GameStart;

		GameStart ();
	}

	void OnDestroy () {
		GameEventManager.GameStart -= GameStart;
	}

	private void GameStart () {
		advanceTo = Vector3.zero;
		transform.position = new Vector3(StartPositionX, StartPositionY, transform.position.z);

		CameraHeight = 2f * Camera.main.orthographicSize;
		CameraWidth = CameraHeight * Camera.main.aspect;
	}

	void FixedUpdate () {

		if (player == null)
			player = FindObjectOfType<Player> ();

		if (!player.enabled)
			return;

		if (Player.IsInCannon) {

			if (advanceTo == Vector3.zero)
				advanceTo = player.transform.position;

			Advance ();

		} else {
			advanceTo = Vector3.zero;
		}
		
	}

	/// <summary>
	/// Makes camera smootly move to the next cannon
	/// </summary>
	public void Advance() {

		transform.position = Vector3.MoveTowards(
			transform.position, 
		    new Vector3(advanceTo.x + StartPositionX, advanceTo.y + StartPositionY, transform.position.z),
		    advancingSpeed * Time.deltaTime);
	}
}
