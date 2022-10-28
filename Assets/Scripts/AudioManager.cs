using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private SoundEffect[] soundEffects;
    private AudioSource[] audioSources;
    private AudioSource soundSource;
    private AudioSource musicSource;
    private AudioSource ambienceSource;

    public AudioMixer audioMixer;

    private float masterVolume = 1f;
    private float soundVolume = 1f;
    private float musicVolume = 1f;
    private float ambienceVolume = 1f;

    private const string GroupMaster = "Master";
    private const string GroupSound = "Sound";
    private const string GroupMusic = "Music";
    private const string GroupAmbience = "Ambience";
    public static AudioManager instance;

    public float MasterVolume { get => masterVolume; set => UpdateMasterVolume(value); }
    public float SoundVolume { get => soundVolume; set => UpdateSoundVolume(value); }
    public float MusicVolume { get => musicVolume; set => UpdateMusicVolume(value); }
    public float AmbienceVolume { get => ambienceVolume; set => UpdateAmbienceVolume(value); }
    public void PlaySound(AudioClip clipToPlay) => soundSource.PlayOneShot(clipToPlay);
    public bool PlayingAmbience { get => ambienceSource.isPlaying; }
    public AudioClip CurrentBGM { get => musicSource.clip; }

    #region Unity Methods

    private void Awake()
    {
        audioSources = GetComponents<AudioSource>();
        soundSource = audioSources[0];
        musicSource = audioSources[1];
        ambienceSource = audioSources[2];
        instance = this;
    }

    private void Start()
    {
        soundEffects = Resources.LoadAll("Sound Effects", typeof(SoundEffect)).Cast<SoundEffect>().ToArray();

        musicSource.loop = true;
        ambienceSource.loop =    true;
    }

    #endregion

    #region Public Methods

    public void PlayMusic(AudioClip clipToPlay, bool fade = false)
    {
        if (musicSource.clip == clipToPlay) { return; }

        if (fade)
            StartCoroutine(StartFade(GroupMusic, 1f, 1f));
        musicSource.clip = clipToPlay;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (!musicSource.isPlaying) { return; }
        musicSource.Stop();
        musicSource.clip = null;
    }

    public void PlayAmbience(string name, bool fade = false)
    {
        SoundEffect soundEffectToPlay = FindSoundEffect(name);
        if (soundEffectToPlay != null)
        {
            if (fade)
                StartCoroutine(StartFade(GroupAmbience, 1f, 1f));
            ambienceSource.clip = soundEffectToPlay.GetRandomClip();
            ambienceSource.Play();
        }
    }
    public void PlayAmbience(string name, AudioSource source)
    {
        SoundEffect soundEffectToPlay = FindSoundEffect(name);
        if (soundEffectToPlay != null)
        {
            source.volume = ambienceVolume * masterVolume;
            source.clip = soundEffectToPlay.GetRandomClip();
            source.Play();
        }
    }

    public void StopAmbience()
    {
        if (!ambienceSource.isPlaying) { return; }
        ambienceSource.Stop();
    }

    public void PlaySound(string name)
    {
        SoundEffect soundEffectToPlay = FindSoundEffect(name);

        if (soundEffectToPlay != null)
            soundSource.PlayOneShot(soundEffectToPlay.GetRandomClip());

    }

    public void PlaySound(string name, AudioSource source)
    {
        SoundEffect soundEffectToPlay = FindSoundEffect(name);

        if (soundEffectToPlay != null)
        {
            source.volume = soundVolume * masterVolume;
            source.clip = soundEffectToPlay.GetRandomClip();
            source.Play();
        }
    }


    #endregion

    #region Private Methods

    private SoundEffect FindSoundEffect(string name)
    {
        foreach (SoundEffect sound in soundEffects)
        {
            if (sound.soundName == name)
            {
                return sound;
            }
        }
        return null;
    }

    private void UpdateMasterVolume(float volume)
    {
        masterVolume = volume;
        soundSource.volume = soundVolume * masterVolume;
        musicSource.volume = musicVolume * masterVolume;
        ambienceSource.volume = ambienceVolume * masterVolume;
    }

    private void UpdateSoundVolume(float volume)
    {
        soundVolume = volume;
        soundSource.volume = soundVolume * masterVolume;
    }

    private void UpdateMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume * masterVolume;
    }

    private void UpdateAmbienceVolume(float volume)
    {
        ambienceVolume = volume;
        ambienceSource.volume = ambienceVolume * masterVolume;
    }

    private IEnumerator StartFade(string audioGroup, float duration, float targetVolume)
    {
        float currentTime = 0;
        audioMixer.GetFloat(audioGroup, out var currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(audioGroup, Mathf.Log10(newVol) * 20);
            yield return null;
        }
    }

    #endregion
}
