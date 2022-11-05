using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    [Header("Designers")]
    public string ambience;
    public string[] accents;
    public float timeBetweenAccents;
    public float timeVariance;
    [Header("Programmers")]
    public AudioSource accentSource;

    private float timeToNextAccent;
    private float rotationSpeed;

    private AudioManager audioManager;
    private Camera mainCamera;

    private void Start()
    {
        audioManager = AudioManager.instance;
        audioManager.PlayAmbience(ambience);
        rotationSpeed = 0.01f;
        PlayAccent();
    }

    private void Update()
    {
        if (!mainCamera)
        {
            var mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCameraObject)
                mainCamera = mainCameraObject.GetComponent<Camera>();
            return;
        }

        transform.position = mainCamera.transform.position;
        transform.rotation.eulerAngles.Set(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z + rotationSpeed);

        timeToNextAccent -= Time.deltaTime;
        if (timeToNextAccent <= 0f)
        {
            PlayAccent();
        }

    }

    private void PlayAccent()
    {
        int randomIndex = Random.Range(0, accents.Length);
        var randomAccent = accents[randomIndex];
        audioManager.PlayAmbience(randomAccent, accentSource);
        timeToNextAccent = timeBetweenAccents + Random.Range(-timeVariance, timeVariance);
    }
}
