using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
	static ScreenFader instance;
	static Color finalColor;

	public RawImage FadeImg;
    public float fadeDuration = 1.5f;
    public bool sceneStarting = true;
	public Color color;
    public AudioSource bgm;

	Color startColor;
	Color endColor;
    float basicVolume;
	float duration = 0;

    void Awake()
    {
		instance = this;
		duration = 0;
		FadeImg.gameObject.SetActive(sceneStarting);

        if (bgm != null)
            basicVolume = bgm.volume;
		if(finalColor == Color.clear){
			finalColor = color;
		}

		if(sceneStarting){
			StartCoroutine( StartScene() );
		}
    }

    void Fade()
    {
		duration += Time.deltaTime;
		FadeImg.color = Color.Lerp(startColor, endColor, duration/fadeDuration);
    }


    IEnumerator StartScene()
    {
		duration = 0;
		startColor = new Color(finalColor.r,finalColor.g,finalColor.b,1);
		endColor = new Color(finalColor.r,finalColor.g,finalColor.b,0);
		while(duration < fadeDuration){
        	Fade();
			yield return null;
		}
		FadeImg.gameObject.SetActive(false);
    }

	IEnumerator EndScene(string sceneName){
		duration = 0;
		startColor = new Color(color.r,color.g,color.b,0);
		endColor = new Color(color.r,color.g,color.b,1);
		FadeImg.gameObject.SetActive(true);

		while(duration < fadeDuration){
			Fade();
            if(bgm != null)
                bgm.volume = basicVolume * (1 - duration) / fadeDuration;
			yield return null;
		}
        if(bgm != null)
            bgm.volume = 0;
		finalColor = color;
		SceneManager.LoadScene(sceneName);

	}
		
	public void MoveScene(string sceneName){
		StartCoroutine( EndScene(sceneName) );
	}
	public static void MoveSceneGlobal(string sceneName)
	{
		if(instance == null){
			SceneManager.LoadScene(sceneName);
		}
		instance.MoveScene(sceneName);
	}

	void OnDestroy(){
		instance = null;
	}


}