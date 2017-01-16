using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    static NoticeUI _instance;
    public static NoticeUI instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    GameObject popup;
    [SerializeField]
    Text text;
    [SerializeField]
    Button touchArea;
    [SerializeField]
    Animator animator;

    string message;
    int messageCount = 0;

    void Start()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        popup.SetActive(false);
        touchArea.onClick.AddListener(OnTouched);
    }

    void OnTouched()
    {
        this.message = "";
        touchArea.enabled = (false);
        animator.SetBool("Active", false);
    }

    public void ShowMessage(string message)
    {
        this.message = message;
        messageCount++;
    }

    void Update()
    {
        if (this.message != "" && messageCount > 0)
        {
            popup.SetActive(false);
            popup.SetActive(true);
            touchArea.enabled = (true);
            text.text = message;
            animator.SetBool("Active", true);
            --messageCount;
        }
    }
}
