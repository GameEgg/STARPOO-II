using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipCountChanger : MonoBehaviour {
    [SerializeField]
    Slider slider;
    [SerializeField]
    Text textShipCount;
    [SerializeField]
    AudioClip tickSound;
    
	void Start ()
    {
        slider.wholeNumbers = true;
        slider.minValue = GameConsts.minShipCount;
        slider.maxValue = GameConsts.maxShipCount;
        slider.onValueChanged.AddListener(ChangeShipCount);

        slider.value = GameSettings.shipCount;

        if (NetworkVariables.isClient)
        {
            slider.gameObject.SetActive(false);
            NetworkEvents.onShipAmountChanged.AddListener(ChangeShipCount);
        }
    }

    void OnDestroy()
    {
        if (NetworkVariables.isClient)
        {
            NetworkEvents.onShipAmountChanged.RemoveListener(ChangeShipCount);
        }
    }

    void ChangeShipCount(byte count)
    {
        ChangeShipCount((float)count);
    }

    void ChangeShipCount(float count)
    {
        GameSettings.shipCount = (int)count;
        textShipCount.text = count.ToString();

        if (NetworkVariables.isServer)
            NetworkManager.instance.ChangeShipAmount((byte)GameSettings.shipCount);

        SFXManager.instance.Play(tickSound);
    }
}
