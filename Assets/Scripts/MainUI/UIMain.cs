using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMain : MonoBehaviour {
    static bool firstSpawn = true;
    // Layouts in Main Panel
    [SerializeField]
    GameObject indexLayout;
    [SerializeField]
    GameObject multiplayerLayout;
    [SerializeField]
    GameObject renameLayout;
    [SerializeField]
    GameObject joinLayout;

    [SerializeField]
    Text nickName;

    // Buttons in Index Layout
    [SerializeField]
    Button btnSinglePlayer;
    [SerializeField]
    Button btnMultiPlayer;
    [SerializeField]
    Button btnExit;

    // Buttons in Multiplayer Layout
    [SerializeField]
    Button btnHost;
    [SerializeField]
    Button btnJoin;
    [SerializeField]
    Button btnRename;
    [SerializeField]
    Button btnBackToIndex;

    // Buttons in Rename Layout
    [SerializeField]
    Button btnRenameApply;
    [SerializeField]
    InputField inputFieldNickname;

    // Join Layout
    [SerializeField]
    Button btnJoinToAddress;
    [SerializeField]
    InputField inputFieldNetworkAddress;

    [SerializeField]
    AudioClip bgm;

    private GameObject currentLayout;

    // Use this for initialization
    void Start ()
    {
        btnSinglePlayer.onClick.AddListener(ClickSinglePlayerBtn);
        btnMultiPlayer.onClick.AddListener(ClickMultiPlayerBtn);
        btnExit.onClick.AddListener(ClickExitBtn);
        btnHost.onClick.AddListener(ClickHostBtn);
        btnJoin.onClick.AddListener(ClickJoinBtn);
        btnRename.onClick.AddListener(ClickRenameBtn);
        btnBackToIndex.onClick.AddListener(ClickBackToIndexBtn);
        btnRenameApply.onClick.AddListener(ClickRenameApplyBtn);
        btnJoinToAddress.onClick.AddListener(ClickJoinToAddressBtn);

        indexLayout.SetActive(true);
        multiplayerLayout.SetActive(false);
        renameLayout.SetActive(false);
        joinLayout.SetActive(false);
        currentLayout = indexLayout;

        LoadPlayerPrefData();
        BGMManager.instance.Play(bgm);
        NetworkManager.instance.StopAll();

        if (firstSpawn)
        {
            transform.localScale = Vector3.zero;
            firstSpawn = false;
            StartCoroutine(FirstSpawn());
        }
    }

    IEnumerator FirstSpawn()
    {
        while(gameObject.transform.localScale.x < 0.995f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.03f);
            yield return null;
        }
    }
    
    void Update()
    {
        if (currentLayout == renameLayout && inputFieldNickname.text != string.Empty)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClickRenameApplyBtn();
            }
        }
        else if (currentLayout == joinLayout)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClickJoinToAddressBtn();
            }
        }
    }

    void ClickSinglePlayerBtn ()
    {
        SceneManager.LoadScene("Room");
    }

    void ClickMultiPlayerBtn ()
    {
        if (GameSettings.nickName == string.Empty || GameSettings.nickName == null)
        {
            ChangeLayout(renameLayout);
        }
        else
        {
            ChangeLayout(multiplayerLayout);
        }
    }

    void ClickExitBtn ()
    {
        Application.Quit();
    }

    void ClickHostBtn ()
    {
        NetworkManager.instance.StartServer();
    }

    void ClickJoinBtn ()
    {
        ChangeLayout(joinLayout);

        // Focus on input field of IP Address
        EventSystem.current.SetSelectedGameObject(inputFieldNetworkAddress.gameObject, null);
        inputFieldNetworkAddress.OnPointerClick(new PointerEventData(EventSystem.current));
    }

    void ClickJoinToAddressBtn()
    {
        if (inputFieldNetworkAddress.text != string.Empty && inputFieldNetworkAddress.text != null)
        {
            PlayerPrefs.SetString("ip",inputFieldNetworkAddress.text);
            NetworkManager.instance.Connect(inputFieldNetworkAddress.text);
        }
        else
        {
            // Join to localhost
            NetworkManager.instance.Connect();
        }
    }
    
    void ClickRenameBtn ()
    {
        ChangeLayout(renameLayout);

        // Focus on input field of NickName
        EventSystem.current.SetSelectedGameObject(inputFieldNickname.gameObject, null);
        inputFieldNickname.OnPointerClick(new PointerEventData(EventSystem.current));
    }
    
    void ClickBackToIndexBtn ()
    {
        ChangeLayout(indexLayout);
    }

    void ClickRenameApplyBtn ()
    {
        if (inputFieldNickname.text != string.Empty && inputFieldNickname.text != null)
        {
            GameSettings.nickName = inputFieldNickname.text;
            PlayerPrefs.SetString("nickName", GameSettings.nickName);
            PlayerPrefs.Save();
            nickName.text = "Player : " + GameSettings.nickName;

            if (GameVariables.me == null)
                GameVariables.me = new Player();
            GameVariables.me.name = GameSettings.nickName;

            ChangeLayout(multiplayerLayout);
        }
    }

    void ChangeLayout (GameObject layout)
    {
        currentLayout.SetActive(false);
        currentLayout = layout;
        currentLayout.SetActive(true);
    }

    void LoadPlayerPrefData ()
    {
        if(GameVariables.me == null)
            GameVariables.me = new Player();
        GameSettings.nickName = PlayerPrefs.GetString("nickName", string.Empty);
        nickName.text = "Player : " + GameSettings.nickName;
        GameVariables.me.name = GameSettings.nickName;

        inputFieldNetworkAddress.text = PlayerPrefs.GetString("ip");
    } 
}
