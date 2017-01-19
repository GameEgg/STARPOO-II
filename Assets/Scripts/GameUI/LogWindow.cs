using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class LogWindow : MonoBehaviour {

    [SerializeField]
    ScrollRect scrollView;
    [SerializeField]
    InputField logText;

    AIScript script;
    RectTransform textRT;
    bool scrolledBottom;
    int isPrintedFrame;

    StringBuilder sb;

    public void Init(AIScript script)
    {
        sb = new StringBuilder();
        textRT = logText.GetComponent<RectTransform>();
        this.script = script;
        logText.text = "";
        logText.textComponent.color = script.color + new Color(0.5f,0.5f,0.5f);
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
        sb.AppendLine(text);
        if(sb.Length > 2000){
            sb.Remove(0,sb.Length - 2000);
        }
        logText.text = sb.ToString();
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
