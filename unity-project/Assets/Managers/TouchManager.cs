using UnityEngine;

public static class TouchManager {

	public static bool Tapped 
	{
		get
		{
			if (Input.GetMouseButtonDown(0))
				return true;

			//For computer spacebar (comment for mobile):
			if (Input.GetButtonDown("Jump")) 
				return true;

			return false;
		}
	}

	public static Vector3 TapPosition
	{
		get 
		{
			if (Input.touchCount > 0) {
				return Input.GetTouch(0).position;
			}

			if (Input.GetMouseButtonDown(0))
				return Input.mousePosition;

			return Vector3.zero;
		}
	}
}

