using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CannonManager : MonoBehaviour {

	//___ Unity object properties:

	public Transform prefab;
	public int numberOfObjects;
	public float recycleOffset;
	public float startXDistance;

	//___


	//best ipad friendly min and max distances:
	//5.5  x 1.6(1.8 for easier y start)
	//11.5 x 7.4
	public Vector2 minDistance, maxDistance;

	private Vector2 nextPosition;
	private Queue<Transform> objectQueue;
	private float startYposition;
	private bool gameStartLoaded;
	private bool disabledFarCannons;

	void Start () {

		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;

		objectQueue = new Queue<Transform>(numberOfObjects);
		for(int i = 0; i < numberOfObjects; i++){
			objectQueue.Enqueue((Transform)Instantiate(prefab));
		}

		GameStart ();
	}

	void OnDestroy () {
		GameEventManager.GameStart -= GameStart;
		GameEventManager.GameOver -= GameOver;
	}

	private void GameStart () {

		enabled = true;

		startYposition = Player.StartPosition.y;

		nextPosition = new Vector2(startXDistance, startYposition);

		foreach (var cannon in objectQueue.ToArray()) {
			cannon.GetComponent<SpriteRenderer>().enabled = true;
			cannon.gameObject.SetActive(true);
		}

		PositionFirstAndSecond ();
		for(int i = 0; i < numberOfObjects - 2; i++){
			RecycleAndReposition();
		}

	}
	
	private void GameOver () {

		foreach (var cannon in objectQueue.ToArray()) {
			cannon.GetComponent<SpriteRenderer>().enabled = false;
			cannon.gameObject.SetActive(false);
		}

		enabled = false;
	}
	
	void Update () {
		if(objectQueue.Peek().position.x + recycleOffset < Player.Position.x){
			RecycleAndReposition();
		}

		HideAndDisableFarCannons ();
	}

	private void PositionFirstAndSecond() {
		Transform first = objectQueue.Dequeue ();
		first.position = Player.StartPosition;
		objectQueue.Enqueue (first);

		Transform second = objectQueue.Dequeue();
		second.position = nextPosition;
		objectQueue.Enqueue(second);
	}
	
	private void RecycleAndReposition () {

		Vector2 distance = new Vector2(
			Random.Range(minDistance.x, maxDistance.x),
			Random.Range(minDistance.y, maxDistance.y));

		nextPosition.x += distance.x;
		nextPosition.y -= distance.y;
		
		Transform o = objectQueue.Dequeue();
		o.position = nextPosition;
		objectQueue.Enqueue(o);
	}

	/// <summary>
	/// Hides the and disables cannons that surpasses the minimum device width. That way the game will have the same gameplay on any device.
	/// </summary>
	void HideAndDisableFarCannons () {

		if (!Player.IsInCannon)
			disabledFarCannons = false;


		if (disabledFarCannons || !Player.IsInCannon) {
			return;
		}

		var inFrontOfPlayer = objectQueue
			.Where (o => o.position.x > Player.Position.x)
			.OrderBy (o => o.position.x)
			.First ();

		foreach (var cannon in objectQueue.ToArray()) {

			var enable = cannon.position.x <= inFrontOfPlayer.position.x;

			cannon.GetComponent<SpriteRenderer>().enabled = enable;
			cannon.GetComponent<BoxCollider2D>().enabled = enable;
		}

		disabledFarCannons = true;

	}
}