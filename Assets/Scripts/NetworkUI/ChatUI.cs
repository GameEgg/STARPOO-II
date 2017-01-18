using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour {

    protected static ChatUI _instance;
    public static ChatUI instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    ChatPlayerUI playerUIPrefab;
    [SerializeField]
    ChatPlayerUIContainer leftUIContainer;
    [SerializeField]
    ChatPlayerUIContainer rightUIContainer;
    [SerializeField]
    InputField chatInputField;

    Dictionary<byte, ChatPlayerUI> chatPlayerUIs = new Dictionary<byte, ChatPlayerUI>();
    List<byte> deathNode = new List<byte>();
    
	void Start () {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        playerUIPrefab.gameObject.SetActive(false);
        chatInputField.gameObject.SetActive(false);

        deathNode.Clear();
        chatPlayerUIs.Clear();

        NetworkEvents.onPlayerJoin.AddListener(OnPlayerJoin);
        NetworkEvents.onPlayerOut.AddListener(OnPlayerOut);
	}

    void OnPlayerJoin(Player player)
    {
        var chatPlayerUI = Instantiate(playerUIPrefab);
        chatPlayerUIs.Add(player.networkId, chatPlayerUI);
        var isRightPanel = leftUIContainer.transform.childCount > rightUIContainer.transform.childCount;

        var parent = (isRightPanel) ? rightUIContainer : leftUIContainer;
        chatPlayerUI.transform.SetParent(parent.transform);
        chatPlayerUI.transform.localScale = Vector3.one;
        chatPlayerUI.gameObject.SetActive(true);
        chatPlayerUI.Init(player, isRightPanel);
        parent.Add(chatPlayerUI);
        Debug.Log("network player join : "+player.networkId);
    }

    void OnPlayerOut(byte id)
    {
        deathNode.Add(id);
        Debug.Log("network player out : "+id);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && NetworkVariables.isNetwork)
        {
            if (chatInputField.gameObject.activeSelf)
            {
                chatInputField.gameObject.SetActive(false);
                if(chatInputField.text.Length > 0)
                {
                    NetworkManager.instance.Chat(chatInputField.text);
                }
            }
            else {
                chatInputField.text = "";
                chatInputField.gameObject.SetActive(true);
                chatInputField.ActivateInputField();
                chatInputField.Select();
            }
        }

        if (chatPlayerUIs.Count > 0 && !NetworkVariables.isNetwork)
        {
            foreach (var playerUI in chatPlayerUIs.Values)
            {
                playerUI.Suicide();
            }
            chatPlayerUIs.Clear();
            leftUIContainer.Clear();
            rightUIContainer.Clear();
        }

        if (deathNode.Count > 0)
        {
            foreach(var deadId in deathNode)
            {
                chatPlayerUIs[deadId].Suicide();
                chatPlayerUIs.Remove(deadId);
            }
            deathNode.Clear();
        }
    }
}
