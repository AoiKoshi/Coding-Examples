using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;
    public static AudioSource MusicPlayer;

    [SerializeField]
    private AudioSource userInterfaceSource;
    public static AudioSource UIPlayer;

    private void Awake()
    {
        MusicPlayer = musicSource;
        UIPlayer = userInterfaceSource;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.Stop();
    }

    public static void PlaySFX(AudioSource source, AudioClip sfx)
    {
        source.PlayOneShot(sfx);
    }
}
