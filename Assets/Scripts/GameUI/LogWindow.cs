using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogWindow : MonoBehaviour {

    [SerializeField]
    ScrollRect scrollView;
    [SerializeField]
    Text logText;

    AIScript script;
    RectTransform textRT;
    bool scrolledBottom;
    int isPrintedFrame;

    public void Init(AIScript script)
    {
        textRT = logText.GetComponent<RectTransform>();
        this.script = script;
        logText.text = "";
        logText.color = script.color + new Color(0.5f,0.5f,0.5f);
        scrollView.verticalNormalizedPosition = 0;
        scrolledBottom = true;
        GameEvents.onLog.AddListener(OnLog);
    }

    void OnDestroy()
    {
        GameEvents.onLog.RemoveListener(OnLog);
    }

    void OnLog(AIScript script, string message)
    {
        if(script == this.script){
            PrintLog(message);
        }
    }

    void PrintLog(string text)
    {
        isPrintedFrame = 2;
        scrolledBottom = scrollView.verticalNormalizedPosition <= 0.01f/textRT.rect.height;
        logText.text += "\n";
        logText.text += text;
        if(scrolledBottom)
            scrollView.verticalNormalizedPosition = -0.01f/textRT.rect.height;

    }
	
    void LateUpdate()
    {
        if(isPrintedFrame == 0 && scrolledBottom)
        {
            scrollView.verticalNormalizedPosition = -0.01f/textRT.rect.height;
        }
        isPrintedFrame--;
    }
}
