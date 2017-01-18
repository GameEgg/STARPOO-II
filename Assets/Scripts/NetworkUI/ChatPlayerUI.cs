using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatPlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform nameFieldParent;
    [SerializeField]
    RectTransform chatFieldParent;
    [SerializeField]
    Text nameField;
    [SerializeField]
    Text chatField;
    [SerializeField]
    Image nameTeduriImage;
    [SerializeField]
    Image chatFieldTeduriImage;
    [SerializeField]
    RectTransform chatFieldTeduri;
    [SerializeField]
    Animator animator;
    [SerializeField]
    Animator messageAnim;
    [SerializeField]
    AudioClip chatAudio;
    [SerializeField]
    AudioClip joinAudio;

    public Player player;
    const float chatLife = 2.4f;
    float chatRestLife = 0;
    const float expendingDuration = 1f;
    bool isDead = false;
    Color originTeduriColor;

    int myScriptCount;

    public void Init(Player player, bool isRightPanel)
    {
        this.player = player;
        nameField.text = player.name;
        if (isRightPanel)
            SettingRightPanelMode();

        myScriptCount = 0;
        originTeduriColor = nameTeduriImage.color;
        chatFieldTeduriImage.color = nameTeduriImage.color;
        chatField.text = "asdfasdfsadfsdaf";
        chatFieldParent.gameObject.SetActive(false);
        NetworkEvents.onChat.AddListener(OnChat);
        RoomEvents.onScriptSelected.AddListener(OnScriptIn);
        RoomEvents.onScriptCanceled.AddListener(OnScriptOut);
        NetworkEvents.onGameEnd.AddListener(OnGameEnd);

        SFXManager.instance.Play(joinAudio);
    }

    void OnGameEnd()
    {
        myScriptCount = 0;
        chatFieldTeduriImage.color = nameTeduriImage.color = originTeduriColor;
    }

    void OnScriptIn(UIScriptListItem item){
        if(player.networkId == item.script.author.networkId)
        {
            if(myScriptCount == 0)
            {
                nameTeduriImage.color = item.color + new Color(0.2f,0.2f,0.2f,0);
            }
            else if(myScriptCount == 1)
            {
                nameTeduriImage.color = nameTeduriImage.color + item.color;
            }
            chatFieldTeduriImage.color = nameTeduriImage.color;
            myScriptCount++;
        }
    }

    void OnScriptOut(UIScriptListItem item)
    {
        if (player.networkId == item.script.author.networkId)
        {
            if (myScriptCount == 1)
            {
                nameTeduriImage.color = originTeduriColor;
            }
            else if (myScriptCount == 2)
            {
                nameTeduriImage.color = nameTeduriImage.color - item.color;
            }
            chatFieldTeduriImage.color = nameTeduriImage.color;
            myScriptCount--;
        }
    }

    void SettingRightPanelMode()
    {
        GetComponent<RectTransform>().pivot = new Vector2(1, 0.5f);
        GetComponent<RectTransform>().localPosition = new Vector2(0, 0f);
        nameFieldParent.SetSiblingIndex(1);
        chatField.alignment = TextAnchor.MiddleRight;
        ReverseAnchoredX(nameFieldParent);
        ReverseAnchoredX(chatFieldParent);
        chatFieldTeduri.localScale = new Vector2(-1, 1);
        chatFieldTeduri.anchoredPosition = new Vector2(-chatFieldTeduri.anchoredPosition.x, chatFieldTeduri.anchoredPosition.y);
    }
    void ReverseAnchoredX(RectTransform rt)
    {
        var ap = rt.anchoredPosition;
        ap.x = -ap.x;
        rt.anchorMin = new Vector2(1, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.anchoredPosition = ap;
        rt.pivot = new Vector2(1, 0.5f);
        //rt.pivot = new Vector2(-rt.pivot.x, rt.pivot.y);
    }

    public void ExpendHeight(float height)
    {
        if (GetComponent<RectTransform>().sizeDelta.y != height && !isDead)
        {
            StartCoroutine(Expending(height));
        }
    }

    IEnumerator Expending(float height)
    {
        var rt = GetComponent<RectTransform>();
        var start = rt.sizeDelta.y;
        var end = height;
        var start_to_end = end - start;
        var startT = Time.time;
        var endT = startT + expendingDuration;
        while(Time.time <= endT)
        {
            var currentDur = Mathf.Min(Time.time-startT, expendingDuration);
            var applyHeight = start + (start_to_end) * (1 - Mathf.Pow(2, - 10 * currentDur/ expendingDuration));
            rt.sizeDelta = new Vector2(600, applyHeight);
            yield return null;
        }
    }



    void OnDisable()
    {
        //이벤트 해제
        NetworkEvents.onChat.RemoveListener(OnChat);
        RoomEvents.onScriptSelected.RemoveListener(OnScriptIn);
        RoomEvents.onScriptCanceled.RemoveListener(OnScriptOut);
        NetworkEvents.onGameEnd.RemoveListener(OnGameEnd);
    }


    void OnChat(ChatMessage chatMessage)
    {
        if(chatMessage.playerId == player.networkId)
        {
            ShowChat(chatMessage.message);
        }
    }

    void ShowChat(string message)
    {
        if (!chatFieldParent.gameObject.activeSelf)
            chatFieldParent.gameObject.SetActive(true);
        messageAnim.SetTrigger("Show");
        chatField.text = message;
        chatRestLife = chatLife;
        SFXManager.instance.Play(chatAudio);
    }

    void Update()
    {
        if (chatRestLife > 0)
        {
            chatRestLife -= Time.deltaTime;
            if (chatRestLife <= 0)
            {
                messageAnim.SetTrigger("Hide");
            }
        }
    }

    public void Suicide()
    {
        StartCoroutine(SuicideCoroutine());
        animator.Play("ChatOff");
    }

    IEnumerator SuicideCoroutine()
    {
        isDead = true;
        yield return Expending(0);
        Destroy(gameObject);
    }
}
