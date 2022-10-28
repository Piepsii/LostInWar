using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Intro,
    Game,
    Lost,
    Won
};

public class Player : MonoBehaviour
{

    [Header("Intro Settings")]
    [SerializeField, Range(1f, 30f)]
    public float introScreenTime = 3f;
    public AudioClip introClip;

    [Header("Game Settings")]
    public List<GameObject> ambienceObjects;
    public AudioSource footstepsSource;
    [SerializeField, Range(1f, 10f)]
    public float deathTime = 1f;
    [SerializeField, Range(1f, 10f)]
    public float breakTime = 5f;
    [SerializeField, Range(1f, 10f)]
    public float airborneTime = 3f;
    public Vector3 movement;

    [Header("Game Over Settings")]
    public List<string> loseSounds;
    public List<string> winSounds;
    public float timeWhenLoseMusicStarts = 7f;
    [SerializeField, Range(1f, 30f)]
    public float loseScreenTime = 3f;

    private Dictionary<string, GameObject> currentSounds = new Dictionary<string, GameObject>();
    private List<AudioClip> clipList = new List<AudioClip>();

    // Private
    private GameState previousState = GameState.Intro;
    private GameState state = GameState.Intro;

    private float timeUntilNextShot = 0f;
    private float timeInAir = 0f;
    private float timeThatKills = 0f;
    private float timeInLoseScreen = 0f;
    private float timeInIntroScreen = 0f;

    private bool isAirborne = false;
    private bool isDying = false;
    private bool isMoving = false;

    private bool isPlaying;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            state = GameState.Won;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position += movement;
        }
    }

    void Update()
    {
        // deciding which state to go to
        switch (state)
        {
            case GameState.Intro:
                timeInIntroScreen += Time.deltaTime;
                if (timeInIntroScreen > introScreenTime)
                    state = GameState.Game;
                break;

            case GameState.Game:
                if (isMoving && isDying)
                {
                    isMoving = false;
                    state = GameState.Lost;
                }
                break;

            case GameState.Lost:
                timeInLoseScreen += Time.deltaTime;

                if (timeInLoseScreen > loseScreenTime)
                {
                    state = GameState.Intro;
                }

                break;

            case GameState.Won:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = GameState.Intro;
                    footstepsSource.Play();
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    footstepsSource.Stop();
                }
                break;

        }

        // updating the current state
        switch (state)
        {
            case GameState.Intro:
                if(previousState != state)
                {
                    AudioManager.instance.PlaySound("sfx_trailer_drum");
                }
                AudioManager.instance.PlayMusic(introClip);
                break;

            case GameState.Game:
                if (previousState != state)
                {
                    AudioManager.instance.StopMusic();

                    foreach (var ambienceObject in ambienceObjects)
                        ambienceObject.SetActive(true);
                }
                timeUntilNextShot += Time.deltaTime;
                if (timeUntilNextShot > breakTime)
                {
                    timeUntilNextShot = 0f;
                    isAirborne = true;
                }

                if (isAirborne)
                {
                    timeInAir += Time.deltaTime;
                    if (!isPlaying)
                    {
                        AudioManager.instance.PlaySound("sfx_airborne");
                        isPlaying = true;
                    }
                    if (timeInAir > airborneTime)
                    {
                        isPlaying = false;
                        isDying = true;
                        if (!isPlaying)
                        {
                            AudioManager.instance.PlaySound("sfx_explosion");
                            isPlaying = true;
                        }
                        timeInAir = 0f;
                        isAirborne = false;
                    }
                }

                if (isDying)
                {
                    timeThatKills += Time.deltaTime;
                    if (timeThatKills > deathTime)
                    {
                        isPlaying = false;
                        isDying = false;
                        timeThatKills = 0f;
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isMoving = true;
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    isMoving = false;
                }

                break;
            case GameState.Lost:
                if(previousState != state)
                {
                    foreach (var sound in loseSounds)
                        AudioManager.instance.PlaySound(sound);
                    foreach (var ambienceObject in ambienceObjects)
                        ambienceObject.SetActive(false);
                }
                if (timeInLoseScreen > timeWhenLoseMusicStarts)
                {
                    AudioManager.instance.PlayMusic(introClip);
                }
                break;
            case GameState.Won:
                if (previousState != state)
                {
                    foreach (var sound in winSounds)
                        AudioManager.instance.PlaySound(sound);
                    AudioManager.instance.PlayMusic(introClip);
                    foreach (var ambienceObject in ambienceObjects)
                        ambienceObject.SetActive(false);
                }
                break;
        }

        previousState = state;
    }

    public IEnumerator RemoveFromClipList(AudioClip audioClip, float t)
    {
        yield return new WaitForSeconds(t);
        clipList.Remove(audioClip);
    }
}
