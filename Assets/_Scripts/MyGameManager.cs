using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class MyGameManager : MMPersistentSingleton<MyGameManager>
{

    void Start()
    {
        MMSoundManager.Instance.LoadSettings();
    }

    public void StartGame()
    {
        StageManager.Instance.EnterFirstStage();
    }

}

public enum GameEventType
{
    EnemyWaveStarted, EnemyWaveCleared, GameWin, GameWinned, GameLose, GameLost,
    PauseGameplay,
    UnpauseGameplay,
    NarrativeEventStart,
    NarrativeEventEnd,
    BossDialogStart,
    BossDialogStarted,
    BossDialogEnd,
    BossDialogEnded,
    MasterDialogStart,


}

public struct GameEvent
{
    public GameEventType GameEventType;

    public GameEvent(GameEventType gameEventType)
    {
        GameEventType = gameEventType;
    }

    static GameEvent e;

    public static void Trigger(GameEventType gameEventType)
    {
        e.GameEventType = gameEventType;

        MMEventManager.TriggerEvent(e);
    }

}