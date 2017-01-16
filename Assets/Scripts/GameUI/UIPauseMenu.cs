using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject panel;
    [SerializeField]
    Button resume;
    [SerializeField]
    Button exit;

    // Use this for initialization
    void Start () {
        resume.onClick.AddListener(ClickResume);
        exit.onClick.AddListener(ClickExit);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    void ClickResume()
    {
        panel.SetActive(!panel.activeSelf);
    }
    void ClickExit()
    {
        SceneManager.LoadScene("Main");
    }
}
