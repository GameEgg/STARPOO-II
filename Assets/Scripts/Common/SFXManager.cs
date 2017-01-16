using UnityEngine;

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

    new AudioSource audio;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }

        audio = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        audio.clip = clip;
        audio.Play();
    }

    public void Stop()
    {
        audio.Stop();
    }
}
