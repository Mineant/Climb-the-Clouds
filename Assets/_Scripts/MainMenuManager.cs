using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MMSingleton<MainMenuManager>
{
    public Button StartButton;

    protected override void Awake()
    {
        base.Awake();
        StartButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        MyGameManager.Instance.StartGame();
    }

}
