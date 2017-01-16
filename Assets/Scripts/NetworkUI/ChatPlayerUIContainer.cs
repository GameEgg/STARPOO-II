using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatPlayerUIContainer : MonoBehaviour {

    public float maxHeight;
    float preferedHeight;

    Dictionary<byte, ChatPlayerUI> childUIs = new Dictionary<byte, ChatPlayerUI>();

    int childCount;
    RectTransform rt;

    bool isDirty;

    void Start()
    {
        isDirty = false;
        rt = GetComponent<RectTransform>();
        NetworkEvents.onPlayerOut.AddListener(OnPlayerOut);
    }

    void LateUpdate()
    {
        float totalH = 0;
        foreach (RectTransform child in rt)
        {
            totalH += child.rect.height;
        }
        float currentY = totalH/2;
        foreach (RectTransform child in rt)
        {
            float h = child.rect.height;
            child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY - h / 2);
            currentY -= h;
        }
    }

    void OnPlayerOut(byte playerid)
    {
        if (childUIs.ContainsKey(playerid))
        {
            childUIs.Remove(playerid);
            isDirty = true;
        }
    }

    void UpdateLayout()
    {
        var height = Mathf.Min(maxHeight, rt.rect.height / (childUIs.Count));

        foreach(var ui in childUIs.Values)
        {
            ui.ExpendHeight(height);
        }
    }

    void Update(){
        if(isDirty){
            UpdateLayout();
            isDirty = false;
        }
    }

    public void Add(ChatPlayerUI ui)
    {
        ui.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 0);
        childUIs.Add(ui.player.networkId,ui);
        UpdateLayout();
    }

    public void Clear()
    {
        childUIs.Clear();
    }
}
