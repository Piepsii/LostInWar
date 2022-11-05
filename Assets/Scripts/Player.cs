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

    [Header("Sequences")]
    public GameObject introSequence;
    public GameObject gameSequence;
    public GameObject lostSequence;
    public GameObject wonSequence;
    public AudioClip bgMusic;

    [Header("Game Settings")]
    public AudioSource footstepsSource;
    public AudioClip swipeIn, swipeOut;
    [SerializeField, Range(0.1f, 10f)]
    public float deathTime = 1f;
    [SerializeField, Range(1f, 10f)]
    public float breakTime = 5f;
    [SerializeField, Range(1f, 10f)]
    public float airborneTime = 3f;
    public Vector3 movement;

    private GameState state = GameState.Intro;

    private float timeUntilNextShot = 0f;
    private float timeInAir = 0f;
    private float timeThatKills = 0f;

    private bool isAirborne = false;
    private bool isDying = false;
    private bool isMoving = false;

    private bool isPlaying;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            SetStateToWon();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position += movement;
        }
    }

    private void Start()
    {
        SetStateToIntro();
        timeUntilNextShot = breakTime;
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetStateToIntro();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetStateToGame();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetStateToLost();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetStateToWon();
        }

        switch (state)
        {
            case GameState.Intro:
                break;

            case GameState.Game:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isMoving = false;
                    footstepsSource.Stop();
                    AudioManager.instance.PlaySound(swipeIn);
                }

                if (Input.GetKeyUp(KeyCode.Space))
                {
                    isMoving = true;
                    footstepsSource.Play();
                    AudioManager.instance.PlaySound(swipeOut);
                }
                break;

            case GameState.Lost:
                break;

            case GameState.Won:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = GameState.Intro;
                }
                break;
        }
    }

    private void Update()
    {
        CheckInput();

        switch (state)
        {
            case GameState.Intro:
                break;

            case GameState.Game:
                timeUntilNextShot -= Time.deltaTime;
                if (timeUntilNextShot <= 0f)
                {
                    timeUntilNextShot = breakTime;
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

                if (isMoving && isDying)
                {
                    isMoving = false;
                    SetStateToLost();
                }


                break;
            case GameState.Lost:
                break;
            case GameState.Won:
                break;
        }
    }

    public void SetStateToIntro()
    {
        state = GameState.Intro;

        introSequence.SetActive(true);
        gameSequence.SetActive(false);
        lostSequence.SetActive(false);
        wonSequence.SetActive(false);
    }

    public void SetStateToGame()
    {
        state = GameState.Game;
        isMoving = true;
        isDying = false;
        AudioManager.instance.StopMusic();

        transform.position = Vector3.zero;
        footstepsSource.Play();

        introSequence.SetActive(false);
        gameSequence.SetActive(true);
        lostSequence.SetActive(false);
        wonSequence.SetActive(false);
    }
    public void SetStateToLost()
    {
        state = GameState.Lost;
        isMoving = false;
        footstepsSource.Stop();

        introSequence.SetActive(false);
        gameSequence.SetActive(false);
        lostSequence.SetActive(true);
        wonSequence.SetActive(false);
    }

    public void SetStateToWon()
    {
        state = GameState.Won;
        isMoving = false;
        footstepsSource.Stop();

        introSequence.SetActive(false);
        gameSequence.SetActive(false);
        lostSequence.SetActive(false);
        wonSequence.SetActive(true);
    }
}
