using UnityEngine;
using System.Collections.Generic;

public class SFXManager : MonoBehaviour
{
    static SFXManager _instance;
    public static SFXManager instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject();
                _instance = go.AddComponent<SFXManager>();
            }
            return _instance;
        }
    }

    const int audioCount = 8;
    int index = 0;
    List<AudioSource> audioArray = new List<AudioSource>();

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < audioCount; ++i)
        {
            audioArray.Add(gameObject.AddComponent<AudioSource>());
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        var audio = audioArray[(index++) % audioCount];
        audio.clip = clip;
        audio.Play();
    }
}
