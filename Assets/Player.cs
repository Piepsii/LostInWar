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
    [SerializeField, Range(1f, 10f)]
    public float introScreenTime = 3f;
    public AudioClip introClip;

    [Header("Game Settings")]
    public AudioSource mortarSource;
    public AudioClip mortarClip;
    public AudioClip airborneClip;
    public AudioClip explosionClip;
    [SerializeField, Range(1f, 10f)]
    public float deathTime = 1f;
    [SerializeField, Range(1f, 10f)]
    public float breakTime = 5f;
    [SerializeField, Range(1f, 10f)]
    public float airborneTime = 3f;
    public Vector3 movement;

    [Header("Game Over Settings")]
    public AudioClip winClip;
    public AudioClip loseClip;

    [SerializeField, Range(1f, 10f)]
    public float loseScreenTime = 3f;

    [Header("Audio Sources")]
    public AudioSource stereoSource;

    private List<AudioClip> clipList = new List<AudioClip>();

    // Private
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

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

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
        switch (state)
        {
            case GameState.Intro:
                timeInIntroScreen += Time.deltaTime;

                if (timeInIntroScreen > introScreenTime)
                {
                    state = GameState.Game;
                    stereoSource.PlayOneShot(introClip);
                }
                break;
            case GameState.Game:
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
                        mortarSource.PlayOneShot(airborneClip);
                        isPlaying = true;
                    }
                    if (timeInAir > airborneTime)
                    {
                        isPlaying = false;
                        isDying = true;
                        if (!isPlaying)
                        {
                            mortarSource.PlayOneShot(explosionClip);
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


                if (isMoving && isDying)
                {
                    isMoving = false;
                    state = GameState.Lost;
                }
                break;
            case GameState.Lost:
                PlaySound(loseClip);
                timeInLoseScreen += Time.deltaTime;
                if (timeInLoseScreen > loseScreenTime)
                {
                    state = GameState.Intro;
                }
                break;
            case GameState.Won:
                PlaySound(winClip);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Application.Quit();
                }
                break;
        }
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (clipList.Contains(audioClip))
            return;
        clipList.Add(audioClip);
        GameObject sourceObject = new GameObject(audioClip.name);
        AudioSource audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        Destroy(sourceObject, audioClip.length);
    }
}
