using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BackgroundManager : MonoBehaviour
{
	public Transform prefabBigCloud;
	public float advancingSpeed;
	public bool startsMoving;
	
	private float yPosition = 0;
	private float zPosition = 2;
	private float recycleOffset = 0;
	private Vector3 nextPosition;
	private Queue<Transform> bigCloudQueue;
	private Bounds bigCloudBounds;
	private Vector2 bigCloudScale;
	private bool isFirst;
	private int numberOfObjects = 2;

	void Start ()
	{

		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;

		bigCloudBounds = prefabBigCloud.GetComponent<SpriteRenderer> ().sprite.bounds;
		bigCloudScale = prefabBigCloud.transform.localScale;

		bigCloudQueue = new Queue<Transform> (2);

		for (int i = 0; i < numberOfObjects; i++) {

			var instance = (Transform)Instantiate (prefabBigCloud);

			instance.parent = Camera.main.transform;
			bigCloudQueue.Enqueue (instance);
		}

		nextPosition = new Vector3 (-5, yPosition, zPosition);

		isFirst = true;

		for (int i = 0; i < numberOfObjects; i++) {
			RecycleAndReposition ();
		}

		if (startsMoving)
			StartMoving ();
		else
			enabled = false;
	}

	private void GameStart ()
	{
		nextPosition = new Vector3 (-5, yPosition, zPosition);

		//if (checkIfNeedsReposition ())
			isFirst = true;

		foreach (var cloud in bigCloudQueue.ToArray())
			cloud.position = nextPosition;

		for (int i = 0; i < numberOfObjects; i++) {
		//	if (checkIfNeedsReposition ()) {
				RecycleAndReposition ();
		//	}
		}
		//enabled = true;

		StartMoving ();
	}
	
	private void GameOver ()
	{
		//enabled = false;

		StopMoving ();
	}

	void OnDestroy ()
	{
		
		GameEventManager.GameStart -= GameStart;
		GameEventManager.GameOver -= GameOver;
	}

	void OnGUI () {

		/*
		GUI.Label (new Rect (1, 1, 100, 100), bigCloudBounds.size.ToString());
		GUI.Label (new Rect (1, 51, 100, 200), startsMoving.ToString () + " - " +
			bigCloudQueue.ToList ().First ().GetComponent<Rigidbody2D> ().velocity.ToString () + " - " +
		    needsRepositionCount);
		    */
	}

	void FixedUpdate ()
	{
		if (checkIfNeedsReposition ()) {
			RecycleAndReposition ();
		}
	}
	 
	private void StartMoving ()
	{

		foreach (var item in bigCloudQueue.ToList()) {
			item.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-advancingSpeed, 0);
		}
	}

	private void StopMoving ()
	{
		foreach (var item in bigCloudQueue.ToList()) {
			item.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		}
	}

	/*public void Move() {

		if (bigCloudQueue == null)
			return;

		foreach (var item in bigCloudQueue.ToArray()) {

			item.transform.position = Vector3.MoveTowards (item.transform.position,
				new Vector3 (item.transform.position.x - advanceDistance, item.transform.position.y, zPosition),
			advancingSpeed * Time.deltaTime);
		}
	}*/

	private bool checkIfNeedsReposition ()
	{
		return bigCloudQueue.Peek ().localPosition.x < (-bigCloudBounds.size.x * bigCloudScale.x) - recycleOffset;
	}

	private void RecycleAndReposition ()
	{

		if (!isFirst) {
			var forwardMostPositionX = bigCloudQueue.First().localPosition.x;

			foreach (var item in bigCloudQueue.ToArray())
				if (item.localPosition.x > forwardMostPositionX)
					forwardMostPositionX = item.localPosition.x;

			//.OrderByDescending (c => c.localPosition.x).Select(c => c.localPosition.x).First();
			nextPosition.x = forwardMostPositionX + (bigCloudBounds.size.x * bigCloudScale.x);// + Time.deltaTime;
			//debugInfo = "forward most: " + forwardMostPositionX;

		}

		var cloud = bigCloudQueue.Dequeue ();
		cloud.localPosition = nextPosition;
		bigCloudQueue.Enqueue (cloud);

		isFirst = false;
	}


}
