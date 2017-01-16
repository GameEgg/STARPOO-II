using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : GameSyncObject
{

    [SerializeField]
    Text gameOverText;

    [SerializeField]
    AudioSource groundSizeAlarm;

    [SerializeField]
    Text timerText;
    [SerializeField]
    Animator timerTextAnim;

    [SerializeField]
    UIGamePlayer[] players;

    float prevTimer;
    
    void Reset()
    {
        players = gameObject.GetComponentsInChildren<UIGamePlayer>();
    }

    public override void GameInit()
    {
        for (int i = 0; i < FleetContainer.fleets.Count; ++i)
        {
            var fleet = FleetContainer.fleets[i];
            players[i].InitWithFleet(fleet);
        }
        gameOverText.gameObject.SetActive(false);

        prevTimer = GameConsts.basicGroundTimer;
        GameVariables.groundChangeTimer.AddListener(TimerChanged);
    }

    public IEnumerator PlayGameOverUI()
    {
        foreach (var fleet in FleetContainer.fleets)
        {
            if(fleet.myShips.Count > 0)
            {
                gameOverText.gameObject.SetActive(true);
                gameOverText.text = fleet.name + " Win!";
                break;
            }
        }
        yield return new WaitForSeconds(3);

    }

    void TimerChanged(float time)
    {
        if (time > prevTimer)
        {
            timerTextAnim.SetTrigger("Go");
            timerText.color = Color.white;
        }
        var alarmTime = 3f;
        if (time < 3 && Mathf.Floor(prevTimer) > Mathf.Floor(time))
        {
            groundSizeAlarm.Play();
            timerText.color = Color.red;
        }
        prevTimer = time;
        timerText.text = time.ToString("0.00");
    }

    void OnDestroy()
    {
        GameVariables.groundChangeTimer.RemoveListener(TimerChanged);
    }
}
