using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {
    static BGMManager _instance;
    public static BGMManager instance { get {

            if(_instance == null)
            {
                var go = new GameObject();
                _instance = go.AddComponent<BGMManager>();
                
            }
            return _instance;


        } }

    AudioSource ready;
    AudioSource playing;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }

        ready = gameObject.AddComponent<AudioSource>();
        playing = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void Stop()
    {
        AudioSource tmp = ready;
        ready = playing;
        playing = tmp;
        StartCoroutine(Fade(0.05f));
    }
    public void FadeStop()
    {
        AudioSource tmp = ready;
        ready = playing;
        playing = tmp;
        StartCoroutine(Fade(0.005f));
    }

    public void Play(AudioClip clip)
    {
        if (playing.clip == clip) return;
        ready.clip = clip;
        ready.volume = 0;

        AudioSource tmp = ready;
        ready = playing;
        playing = tmp;
        playing.Play();
        StartCoroutine(Fade(0.05f));
    }

    IEnumerator Fade(float speed)
    {
        while(playing.volume < 1 || ready.volume > 0)
        {
            ready.volume -= speed;
            playing.volume += speed;
            yield return null;
        }
        ready.Stop();
    }
}
