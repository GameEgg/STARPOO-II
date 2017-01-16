using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.IO;


public class UIScriptList : MonoBehaviour {
    public AIScriptEvent onScriptClicked = new AIScriptEvent();

    [SerializeField]
    Transform itemParent;
    [SerializeField]
    UIScriptListItem itemTemplete;
    [SerializeField]
    Button reloadBtn;

    
    void Start()
    {
        CreateScriptItems();
        reloadBtn.onClick.AddListener(Reload);
    }

    void CreateScriptItems()
    {
        itemTemplete.gameObject.SetActive(false);
        foreach (var script in ScriptLoader.LoadAIScripts())
        {
            var item = Instantiate(itemTemplete);
            item.gameObject.SetActive(true);
            item.transform.SetParent(itemParent);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.ShowScript(script);
            item.onScriptClicked.AddListener(onScriptClicked.Invoke);
        }
    }

    void Reload()
    {
        DeleteScriptItems();
        CreateScriptItems();
    }

    void DeleteScriptItems()
    {
        foreach (Transform child in itemParent)
        {
            if (child == itemTemplete.transform)
                continue;
            Destroy(child.gameObject);
        }
    }
}
