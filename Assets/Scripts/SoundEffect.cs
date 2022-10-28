using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effect", menuName = "Sound Effect")]
public class SoundEffect : ScriptableObject
{
    public string soundName;
    public AudioClip[] sounds;

    private void OnValidate()
    {
        soundName = name.ToLower();
    }

    public AudioClip GetRandomClip()
    {
        var length = sounds.Length;
        var randomIndex = UnityEngine.Random.Range(0, length);
        return sounds[randomIndex];
    }
}
