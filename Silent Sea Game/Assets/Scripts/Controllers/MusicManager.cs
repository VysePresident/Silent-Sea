/*
 * Coded by Isaac Burns using the following tutorial:
 * https://www.youtube.com/watch?v=ToVL_f9G9Yk
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private static bool end = false;

    void Awake()
    {
        if (instance == null)
        {
            //If I am the first instance, make me the Singleton
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (end == true)
        {
            end = false;
            Destroy(gameObject);
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        //audioSource.Stop();
        audioSource.volume = startVolume;
        end = true;
        //audioSource.volume = startVolume;
    }
}