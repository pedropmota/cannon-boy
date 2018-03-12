using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.SocialPlatforms;
using System.Linq;
using GooglePlayGames;

public class ScoreManager : MonoBehaviour {

	//___ Unity object properties:

	public GUIText ingameScore, ingameOutline, gameoverScore, gameoverOutline;
	public bool reportScores;

	//___


	//android:
	public static string LeaderboardID = "Android leaderboardId";

	//ios:
	//public static string LeaderboardID = "IOS leaderboardId";

	public static int Score { get; private set; }


	private readonly Dictionary<string, int> achievementScores = new Dictionary<string, int> ()
	{
		//android:
		{ "CgkIsKHfyP4TEAIQAA", 5 },
		{ "CgkIsKHfyP4TEAIQAQ", 10 },
		{ "CgkIsKHfyP4TEAIQAg", 20 },
		{ "CgkIsKHfyP4TEAIQAw", 40 },
		{ "CgkIsKHfyP4TEAIQBA", 60 },
		{ "CgkIsKHfyP4TEAIQBQ", 80 },
		{ "CgkIsKHfyP4TEAIQBg", 100 },
		{ "CgkIsKHfyP4TEAIQBw", 150 },
		{ "CgkIsKHfyP4TEAIQCA", 300 },
		{ "CgkIsKHfyP4TEAIQCQ", 500 }

		//ios:
//		{ "com.pedropmota.cannonboy.achievements.gettingthehang", 5 },
//		{ "com.pedropmota.cannonboy.achievements.begginner", 10 },
//		{ "com.pedropmota.cannonboy.achievements.sunset", 20 },
//		{ "com.pedropmota.cannonboy.achievements.night", 40 },
//		{ "com.pedropmota.cannonboy.achievements.morning", 60 },
//		{ "com.pedropmota.cannonboy.achievements.blue", 80 },
//		{ "com.pedropmota.cannonboy.achievements.specialist", 100 },
//		{ "com.pedropmota.cannonboy.achievements.mentor", 150 },
//		{ "com.pedropmota.cannonboy.achievements.master", 300 },
//		{ "com.pedropmota.cannonboy.achievements.conqueror", 500 }
	};

	private readonly Dictionary<string, int> achievementScoresALL = new Dictionary<string, int> ()
	{
		//android:
		{ "CgkIsKHfyP4TEAIQCg", 5000 },
		{ "CgkIsKHfyP4TEAIQCw", 10000 }

		//ios:
//		{ "com.pedropmota.cannonboy.achievements.hero", 5000 },
//		{ "com.pedropmota.cannonboy.achievements.heroii", 10000 }
	};

	void Start () {

		//Commented for WebGL deployment:
		//PlayGamesPlatform.Activate ();

		//GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

		GameEventManager.GameStart += GameStart;
		GameEventManager.GameOver += GameOver;

		GameStart ();
	}

	void OnGUI() {

		ingameScore.text = Score.ToString ();
		ingameOutline.text = Score.ToString ();

		//GUI.Label (new Rect (1, 1, 600, 200), "Achivements loaded: " + (achievementsLoaded ? "Yes ": "No ") + achievementsLoadedQt + " noinfo2 " + achieventsInfo);

		/*
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;

		ingameScore.text = height.ToString() + " - " + width.ToString ();
		ingameOutline.text = height.ToString() + " - " + width.ToString ();
		*/

		/*
		var player = FindObjectOfType<Player> ();
		ingameScore.text = player.GetComponent<Rigidbody2D> ().velocity.ToString();
		ingameOutline.text = player.GetComponent<Rigidbody2D> ().velocity.ToString();
		*/
	}

	public static void AddPoint() {
		Score++;
	}

	void GameStart() {
		Score = 0;

		enabled = true;
	}

	void GameOver () {

		int bestScore = GetStoredBest ();

		if (Score > bestScore)
			SaveBest (Score);

		//Debug.Log("Test version - Not reporting score to the leaderboard"); //Comment line below the if
		if (reportScores && Social.localUser.authenticated)
		{
			Social.ReportScore(Score, LeaderboardID, (bool ok) => { });
		};

		var text = string.Format ("SCORE: {0}  BEST: {1}", Score, Mathf.Max (Score, bestScore));

		gameoverScore.text = text;
		gameoverOutline.text = text;

		ingameScore.text = "";
		ingameOutline.text = "";

		ApplyAchievements ();
	}

	int GetStoredBest() {
		return PlayerPrefs.GetInt("bestscore", 0);
	}

	void SaveBest(int points) {
		PlayerPrefs.SetInt("bestscore", points);	
	}

	void ApplyAchievements() {

		if (!reportScores)
			return;

		PlayerPrefs.SetInt ("totalCannonsAllTime", PlayerPrefs.GetInt ("totalCannonsAllTime") + Score);

		if (!Social.localUser.authenticated)
			return;

		#region All playthroughs achievements:

		int totalAllTime = PlayerPrefs.GetInt ("totalCannonsAllTime");

		var toReportAllTime = totalAllTime <= achievementScoresALL.First ().Value ? 
			achievementScoresALL.ElementAt (0) : 
			achievementScoresALL.ElementAt (1);

		Social.ReportProgress (toReportAllTime.Key,
		                       totalAllTime / toReportAllTime.Value * 100f,
		                       (ok) => { });

		#endregion


		#region High score achievements:

		if (achievementScores.Any (a => Score >= a.Value)) 
		{
			var achieveds = achievementScores.Where(a => Score >= a.Value);

			foreach (var item in achieveds) {
				GKAchievementReporter.ReportAchievement(item.Key, 100, true);
			}
		}

		#endregion
	}

	//Commented for WebGL Deployment:
	public static void OpenLeaderboard() {
//		//var leaderboard = Social.CreateLeaderboard ();
//		//leaderboard.id = LeaderboardID; 
//		
//		if (!Social.localUser.authenticated) {
//			Social.localUser.Authenticate ((bool ok) => 
//			                               {
//				((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (LeaderboardID); 
//				//GameCenterPlatform.ShowLeaderboardUI (ScoreManager.LeaderboardID, TimeScope.AllTime);
//			});
//		} else {
//			((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (LeaderboardID); 
//			//GameCenterPlatform.ShowLeaderboardUI (ScoreManager.LeaderboardID, TimeScope.AllTime);
//		}
	}

	void OnDestroy () {
		GameEventManager.GameStart -= GameStart;
		GameEventManager.GameOver -= GameOver;
	}



}

