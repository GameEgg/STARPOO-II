using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptListItem : MonoBehaviour {
    public AIScriptEvent onScriptClicked = new AIScriptEvent();

    [SerializeField]
    Text nameText;
    [SerializeField]
    Text authorText;
    [SerializeField]
    Animator stateAnim;
    [SerializeField]
    AudioClip showSound;
    [SerializeField]
    AudioClip hideSound;

    public Button       btn;
    public AIScript    script = null;
    public bool interactable = true;
    public Color color;

    /// <summary>
    /// 스크립트를 보여줍니다.
    /// </summary>
    /// <param name="script"></param>
    public void ShowScript(AIScript script)
    {
        this.script = script;
        if (script == null)
        {
            if (stateAnim == null)
                gameObject.SetActive(false);
            else
                stateAnim.SetBool("Selected", false);
            if (btn != null)
                btn.interactable = false;
            SFXManager.instance.Play(hideSound);
            return;
        }

        script.color = color;
        if (stateAnim == null)
            gameObject.SetActive(true);
        else
            stateAnim.SetBool("Selected", true);
        if (nameText != null)
            nameText.text = script.name;
        if (authorText != null)
            authorText.text = script.author.name;
        if (btn != null)
        {
            btn.enabled = false;
            btn.enabled = true;
            btn.interactable = interactable;
        }
        SFXManager.instance.Play(showSound);
    }

    void Awake()
    {
        if(btn != null)
            btn.onClick.AddListener(Clicked);
        if (nameText != null)
            nameText.text = "";
        if (authorText != null)
            authorText.text = "";
        if(stateAnim != null)
            stateAnim.SetBool("Selected", false);
        if (btn != null)
            btn.interactable = false;
    }

    void Clicked()
    {
        onScriptClicked.Invoke(script);
    }
}
