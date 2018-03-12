using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip explosion1;
	public AudioClip explosion2;
	public AudioClip explosion3;

	public AudioClip fall1;
	public AudioClip fall2;
	public AudioClip fall3;

	public AudioClip suck1;
	public AudioClip suck2;

	public AudioSource suckSFXAudioSource;

	public enum SoundOption
	{
		MusicAndFX,
		FXOnly,
		MusicOnly,
		Off
	}
	
	private static AudioClip[] explosionSounds;
	private static AudioClip[] fallSounds;
	private static AudioClip[] suckSounds;

	private static AudioSource instance;
	private static AudioSource instanceSuckSFX;
	
	// Use this for initialization
	void Start () {

		instance = GetComponent<AudioSource> ();
		instanceSuckSFX = suckSFXAudioSource;

		explosionSounds = new AudioClip[] { explosion1, explosion2, explosion3 };
		fallSounds = new AudioClip[] { fall1, fall2, fall3 };
		suckSounds = new AudioClip[] { suck1, suck2 };

		ManageStartSoundOption ();
	}

	public static void PlayExplosion() {
		PlayRandomSound (explosionSounds, instance);
	}

	public static void PlayFall() {
		PlayRandomSound (fallSounds, instance);
	}

	public static void PlaySuck() {
		PlayRandomSound (suckSounds, instanceSuckSFX);
	}

	public static void SetSoundOption(SoundOption option) {
		
		PlayerPrefs.SetInt ("soundOption", (int)option);
	
	}

	public static SoundOption GetCurrSoundOption() {
	
		return (SoundOption)PlayerPrefs.GetInt ("soundOption", (int)SoundOption.MusicAndFX);
	}

	private void ManageStartSoundOption() {
		var soundOption = GetCurrSoundOption ();

		if (soundOption == SoundOption.MusicOnly ||
			soundOption == SoundOption.Off) {

			instance.enabled = false;
			suckSFXAudioSource.enabled = false;
		}
	}

	private static void PlayRandomSound(AudioClip[] clips, AudioSource instance) {
	
		if (!instance.enabled)
			return;

		var random = clips [Random.Range (0, clips.Length)];

		instance.clip = random;
		instance.Play();
	}


}
