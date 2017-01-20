using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIRoom : MonoBehaviour {
    [SerializeField]
    Text textRoomTitle;
    [SerializeField]
    UIScriptList scriptList;
    [SerializeField]
    UIScriptListItem[] selectedItems;
    [SerializeField]
    Text textVS;

    [SerializeField]
    UIShipCountChanger shipCountChanger; // 추후 멀티플레이시 방장이 아니라면 이걸 숨겨야함
    [SerializeField]
    Button btnBack;
    [SerializeField]
    Button btnPlay; // 추후 멀티플레이시 방장이 아니라면 이걸 숨겨야함

    [SerializeField]
    AudioClip bgm;

    Coroutine updateVSColorCoroutine;
    int selectedCount;

    void Start ()
    {
        ClearHistory();
        var isRoommaker = !NetworkVariables.isNetwork || NetworkVariables.isServer;

        btnPlay.onClick.AddListener(ClickPlayBtn);
        btnPlay.interactable = false;
        if (!isRoommaker) btnPlay.gameObject.SetActive(false);
        btnBack.onClick.AddListener(ClickBackBtn);
        if (NetworkVariables.isNetwork)
        {
            scriptList.onScriptClicked.AddListener(SelectScriptAndSend);
        }
        else
        {
            scriptList.onScriptClicked.AddListener(SelectScript);
        }

        UpdateVSColor(0, true);
        selectedCount = 0;

        byte itemIndex = 0;
        foreach (var item in selectedItems)
        {
            byte fixedItemIndex = itemIndex++;
            if (isRoommaker)
            {
                item.btn.onClick.AddListener(() => { CancelScript(fixedItemIndex); });
            }
            else
            {
                item.interactable = false;
            }
        }
        selectedItems[0].color = new Color(1,0.1f,0,1);
        selectedItems[1].color = new Color(0,0.3f,1,1);

        NetworkEvents.onPlayGame.AddListener(ClickPlayBtn);
        NetworkEvents.onScriptIn.AddListener(SelectScript);
        if(!isRoommaker)
            NetworkEvents.onScriptOut.AddListener(CancelScript);

        if (NetworkVariables.isNetwork)
        {
            if (NetworkVariables.isServer)
            {
                SetRoommaker(GameVariables.me);
                TransferTCP.instance.onNewClientJoin.AddListener(ClientJoin);
            }
            if (NetworkVariables.isClient)
            {
                StartCoroutine(SetRoomTitleAsClient());
            }
        }

        BGMManager.instance.Play(bgm);
    }

    void ClearHistory(){

        foreach (FleetHistory fleetHistory in BattleHistory.fleetHistorys)
        {
            fleetHistory.shipHistorys.Clear();
        }
        BattleHistory.fleetHistorys.Clear();
    }

    void OnDestroy()
    {
        var isRoommaker = !NetworkVariables.isNetwork || NetworkVariables.isServer;
        NetworkEvents.onPlayGame.RemoveListener(ClickPlayBtn);
        NetworkEvents.onScriptIn.RemoveListener(SelectScript);
        if (!isRoommaker)
            NetworkEvents.onScriptOut.RemoveListener(CancelScript);
        if (NetworkVariables.isNetwork && NetworkVariables.isServer)
        {
            TransferTCP.instance.onNewClientJoin.RemoveListener(ClientJoin);
        }
    }

    /// <summary>
    /// 중간에 새 클라이언트가 접속하면, 기존 선택된 스크립트에 대한 정보를 전송해준다.
    /// </summary>
    /// <param name="client"></param>
    void ClientJoin(NetworkClient client)
    {
        byte scriptIndex = 0;
        foreach (var item in selectedItems) {
            if (item.script != null)
            {
                var packet = new pScript();
                packet.code = item.script.code;
                packet.index = scriptIndex;
                packet.name = item.script.name;
                packet.networkId = item.script.author.networkId;
                client.sendQueue.Enqueue(packet.Serialize(PacketType.pSendScript));
            }
            scriptIndex++;
        }
    }

    IEnumerator SetRoomTitleAsClient()
    {
        textRoomTitle.text = "";
        while (NetworkManager.instance.playerContainer.players.Count == 0)
        {
            yield return null;
        }
        SetRoommaker(NetworkManager.instance.playerContainer.players[0]);
    }

    void SetRoommaker(Player player)
    {
        textRoomTitle.text = player.name + "'s Room";
    }

    void UpdateVSColor(float a, bool immediatly = false)
    {
        if (updateVSColorCoroutine != null)
            StopCoroutine(updateVSColorCoroutine);
        updateVSColorCoroutine = StartCoroutine(UpdateVSColorCoroutine(a, immediatly));
    }
    IEnumerator UpdateVSColorCoroutine(float a, bool immediatly)
    {
        var t = 0f;
        var c = textVS.color;
        if (immediatly)
        {
            c.a = a;
            textVS.color = c;
        }
        while (c.a != a) {
            c.a = Mathf.Lerp(c.a, a, t);
            t += Time.deltaTime;
            textVS.color = c;
            yield return null;
        }
    }

    /// <summary>
    /// 선택된 스크립트를 눌렀을 때(x버튼) 호출됩니다.
    /// </summary>
    /// <param name="clickedItem"></param>
    void CancelScript(byte index)
    {
        var item = selectedItems[index];
        RoomEvents.onScriptCanceled.Invoke(item);
        item.ShowScript(null);
        selectedCount--;

        UpdateVSColor((float)selectedCount / selectedItems.Length);
        if (selectedCount != selectedItems.Length)
            btnPlay.interactable = false;

        if (NetworkVariables.isServer)
        {
            NetworkManager.instance.CancelScript(index);
        }
    }

    void SelectScriptAndSend(AIScript script)
    {
        SelectScript(script);
        NetworkManager.instance.SendScript(script);
    }

    /// <summary>
    /// 스크립트 리스트의 스크립트를 클릭하면 호출됩니다.
    /// </summary>
    /// <param name="script"></param>
    void SelectScript(AIScript script)
    {
        bool somethingSelected = false;
        UIScriptListItem selectedItem = null;
        foreach (var item in selectedItems)
        {
            if (item.script == null)
            {
                item.ShowScript(script);
                selectedCount++;
                somethingSelected = true;
                selectedItem = item;
                break;
            }
        }
        if (somethingSelected)
        {
            RoomEvents.onScriptSelected.Invoke(selectedItem);
            UpdateVSColor((float)selectedCount / selectedItems.Length);
            if (selectedCount == selectedItems.Length)
                btnPlay.interactable = true;
        }
    }

    /// <summary>
    /// 플레이 버튼을 눌렀을 때 호출됩니다.
    /// </summary>
    /// <returns></returns>
    void ClickPlayBtn()
    {
        if(NetworkVariables.isServer)
            NetworkManager.instance.PlayGame();

        GameSettings.selectedScripts.Clear();
        foreach (var item in selectedItems)
        {
            var script = new AIScript(item.script);
            script.color = item.color;
            GameSettings.selectedScripts.Add(script);
        }

        FleetContainer.InitFleetsWithGameSettings();
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// 뒤로가기 버튼을 누르면 호출됩니다.
    /// </summary>
    void ClickBackBtn()
    {
        GameSettings.selectedScripts.Clear();
        SceneManager.LoadScene("Main");
    }
}
