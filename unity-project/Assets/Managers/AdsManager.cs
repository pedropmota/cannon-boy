using UnityEngine;
using ChartboostSDK;
//using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour {

	public int AmountOfPlaysWithoutAd;
	public int ScoreToReachUntilStartShowingAds;

	public int ScoreAlwaysShowsAds;

	private int timesPlayedWithoutAds = 0;

	private byte adPluginToShow = 0; //0 -> AppLovin ; 1 -> ChartBoost

	//private BannerView bannerView;

	// Use this for initialization
	void Start () {
		GameEventManager.GameOver += ShowAds;
		GameEventManager.GameStart += GameStart;

		//AppLovin.SetSdkKey("put key here");

		AppLovin.InitializeSdk();

		AppLovin.SetUnityAdListener(this.gameObject.name);

		AppLovin.PreloadInterstitial ();

		Chartboost.cacheInterstitial (CBLocation.GameOver);
		//Chartboost.setAutoCacheAds(true);


		/*
		#if UNITY_ANDROID
		string adUnitId = "INSERT_ANDROID_BANNER_AD_UNIT_ID_HERE";
		#elif UNITY_IPHONE
		string adUnitId = "";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Create a 320x50 banner at the bottom of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);

		bannerView.Hide ();
		*/
	}

	//void OnGUI() {
		//GUI.Label (new Rect (1, 1, 100, 100), (lastAdPluginShown == 0 ? "Shown - Applovin" : "Shown - Chartboost"));
	//}

	void OnDestroy () {
		GameEventManager.GameOver -= ShowAds;
		GameEventManager.GameStart -= GameStart;

		//if (bannerView != null)
		//	bannerView.Destroy ();
	}


	void ShowAds() {
		ShowBanner ();
		TryShowInterstitial ();
	}


	void ShowBanner() {


		//bannerView.Show();
	}
	
	void GameStart() {
		//bannerView.Hide ();

		Chartboost.cacheInterstitial (CBLocation.GameOver);
	}

	void TryShowInterstitial() {

		//This is score - 1, so starting counting after the start cannon
		PlayerPrefs.SetInt ("totalScoreAllTime", PlayerPrefs.GetInt ("totalScoreAllTime") + ScoreManager.Score - 1);

		int totalScore = PlayerPrefs.GetInt ("totalScoreAllTime");

		if (totalScore < ScoreToReachUntilStartShowingAds) {
			return;
		}


		if (timesPlayedWithoutAds < AmountOfPlaysWithoutAd && ScoreManager.Score < ScoreAlwaysShowsAds) {
			timesPlayedWithoutAds ++;
			return;
		}


		if (adPluginToShow == 0 && !AppLovin.HasPreloadedInterstitial ()) {

			if (Chartboost.hasInterstitial (CBLocation.GameOver)) {
				adPluginToShow = 1;
			} else {
				return;
			}
		}

		if (adPluginToShow == 1 && !Chartboost.hasInterstitial(CBLocation.GameOver)) {

			if (AppLovin.HasPreloadedInterstitial ()) {
				adPluginToShow = 0;
			} else {
				return;
			}
		}


		if (adPluginToShow == 0) {

			AppLovin.ShowInterstitial();

			//lastAdPluginShown = 0;

			adPluginToShow = 1;

		} else {// if (adPluginToShow == 1) {

			Chartboost.showInterstitial(CBLocation.GameOver);

			//lastAdPluginShown = 1;

			adPluginToShow = 0;
		}

		timesPlayedWithoutAds = 0;
	}

	// Update is called once per frame
	void Update () {
		//if(TouchManager.Tapped){

				//AppLovin.ShowInterstitial();
		//		Chartboost.showInterstitial(CBLocation.GameOver);

		//}
	}



	void onAppLovinEventReceived(string ev){
		if(ev.Contains("DISPLAYEDINTER")) {
			// An ad was shown.  Pause the game.
		}
		else if(ev.Contains("HIDDENINTER")) {
			// Ad ad was closed.  Resume the game.
			// If you're using PreloadInterstitial/HasPreloadedInterstitial, make a preload call here.
			AppLovin.PreloadInterstitial();
		}
		else if(ev.Contains("LOADEDINTER")) {
			// An interstitial ad was successfully loaded.
		}
		else if(string.Equals(ev, "LOADINTERFAILED")) {
			// An interstitial ad failed to load.
		}
	}

}
