using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using DialogueQuests;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public enum SceneTransitionModes
{
    UtoM = 0, DtoM, LtoM, RtoM, MtoD = 10, MtoU,

}

public class StageManager : MMPersistentSingleton<StageManager>, MMEventListener<GameEvent>, MMEventListener<MMLifeCycleEvent>
{

    public enum StageStates { }
    public List<EnemyData> Enemies;
    public Character PlayerPrefab;
    public string StageScene = "Stage";
    public string MainMenuScene = "MainMenu";
    public Animator SceneTransitionAnimator;

    [Header("Stage")]
    public int CurrentStageIndex;
    public StageInfo FirstStageInfo;
    public StageInfo CurrentStageInfo;
    public StageInfo NextStageInfo;
    public List<StageType> StageProgression;
    public Character MiniBossEnemy;
    public Character BossEnemy;
    public List<BaseInventoryItem> BossDrops;

    [Header("Wave Parameters")]
    [Tooltip("Will randomly pick the last X enemies of the highest cost.")]
    public int WaveEnemyRange = 5;
    public BaseMultiplierVariableStat NumberOfWavesStat;
    public BaseMultiplierVariableStat MoneyPerWaveStat;
    public BaseMultiplierVariableStat HighestCostEnemyPerWaveStat;
    [Header("Reward Parameters")]
    public BaseMultiplierVariableStat CommonRewardStat;
    public BaseMultiplierVariableStat UncommonRewardStat;
    public BaseMultiplierVariableStat RareRewardStat;
    public BaseMultiplierVariableStat LegendaryRewardStat;

    [Header("Player Dead Cutscene")]
    public PlayableDirector PlayerDeadDirector;
    public GameObject Rescuer;
    public Transform RescuerOffset;

    public Character Player { get; protected set; }
    public Stage CurrentStage { get; protected set; }
    protected bool _bossDialogging;
    protected List<StageInfo> _doorStageInfos;
    protected bool _isPlaying;

    void Start()
    {
        // NarrativeManager.Get().onPauseGameplay += OnPauseGamePlay;
        // NarrativeManager.Get().onUnpauseGameplay += OnUnpauseGameplay;
    }

    private void OnUnpauseGameplay()
    {
        EnablePlayerProperties(true, true, true, true);
    }


    private void OnPauseGamePlay()
    {
        EnablePlayerProperties(false, false, false, false);
    }


    public void EnablePlayerProperties(bool damageEnabled, bool inputEnabled, bool physicsEnabled, bool uiEnabled)
    {
        if (damageEnabled) Player.CharacterHealth.DamageEnabled();
        else Player.CharacterHealth.DamageDisabled();

        Player.GetComponent<Collider>().enabled = physicsEnabled;
        Player.GetComponent<TopDownController>().enabled = physicsEnabled;

        Player.GetComponent<CharacterOrientation3D>().enabled = inputEnabled;
        InputManager.Instance.InputDetectionActive = inputEnabled;
        if (UIManager.HasInstance)
            if (uiEnabled) UIManager.Instance.ShowHUD();
            else UIManager.Instance.HideHUD();
    }


    public void EnterFirstStage()
    {
        StartCoroutine(_EnterFirstStageCoroutine());
    }

    private IEnumerator _EnterFirstStageCoroutine()
    {

        CurrentStageIndex = 0;
        CurrentStageInfo = null;
        NextStageInfo = null;
        _isPlaying = true;
        GameManager.Instance.SetPoints(0);

        Player = Instantiate(PlayerPrefab);
        EnablePlayerProperties(false, false, false, false);
        DontDestroyOnLoad(Player.gameObject);

        NextStageInfo = GetRandomStageInfo(0, 0);

        PlaySceneTransition(SceneTransitionModes.UtoM);
        yield return new WaitForSeconds(0.5f);

        // Trigger the conversation here.

        EnterNextStage(CurrentStageIndex);
    }


    public void EnterNextStage(int stageNo)
    {
        StartCoroutine(_EnterNextStageCoroutine(stageNo));
    }

    private IEnumerator _EnterNextStageCoroutine(int stageNo)
    {
        CurrentStageIndex = stageNo;

        EnablePlayerProperties(false, false, false, false);

        yield return SceneManager.LoadSceneAsync(StageScene);

        EnablePlayerProperties(false, false, false, false);

        LevelManager.Instance.SceneCharacters.Add(Player);
        Player.FindAbility<CharacterWuXiaHandleSkillBook>().UpdateUI();

        CurrentStage = Stage.FindStage();
        CurrentStageInfo = NextStageInfo;

        // Initialize the Doors
        _doorStageInfos = new();
        if (CurrentStageInfo.Type == StageType.TopOfMountain)
        {
            // I really want to have some type of story that happens when the character reached the top of the mauntain.
            foreach (var door in CurrentStage.Doors)
            {
                door.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < CurrentStage.Doors.Count; i++)
            {
                var door = CurrentStage.Doors[i];
                var doorInfo = GetRandomStageInfo(CurrentStageIndex + 1, i);
                door.Initialization(i, doorInfo);
                door.OnSelect += OnDoorSelected;
                door.Deactivate();
                _doorStageInfos.Add(doorInfo);
            }
        }

        // Enable the stage environment
        CurrentStage.StageObjects.Find(s => s.StageType == CurrentStageInfo.Type)?.ActivateRandomObjectGroup();

        if (CurrentStageInfo.Type == StageType.Normal)
        {
            // Calculate the Wave Info
            WaveData wave = new();
            wave.TotalMoney = (int)MoneyPerWaveStat.GetStat(CurrentStageIndex);
            wave.HighestCostUnit = (int)HighestCostEnemyPerWaveStat.GetStat(CurrentStageIndex);
            wave.NumberOfWaves = (int)NumberOfWavesStat.GetStat(CurrentStageIndex);
            wave.Enemies = Enemies.Where(e => e.Cost <= wave.HighestCostUnit).OrderByDescending(e => e.Cost).Take(WaveEnemyRange).PickRandom<EnemyData>(3).ToList();

            // Spawn Them Monsters.
            WaveManager.Instance.StartWave(wave);
        }
        else if (CurrentStageInfo.Type == StageType.Boss)
        {
            _bossDialogging = true;
            WaveManager.Instance.StartBossWave(BossEnemy);
        }
        else if (CurrentStageInfo.Type == StageType.MiniBoss)
        {
            _bossDialogging = true;
            WaveManager.Instance.StartBossWave(MiniBossEnemy);
        }
        else if (CurrentStageInfo.Type == StageType.Shop)
        {
            Shop shop = FindObjectOfType<Shop>();
            RarityLootTable rewardsLootTable = GetRewardRarityLootTable(CurrentStageIndex);
            shop.Initialization(rewardsLootTable);
        }


        PlaySceneTransition(SceneTransitionModes.MtoD);
        yield return CurrentStage.PlayEnterStageAnimation(Player.gameObject);

        yield return new WaitForSeconds(0.3f);

        if (CurrentStageInfo.Type == StageType.Boss || CurrentStageInfo.Type == StageType.MiniBoss)
        {
            GameEvent.Trigger(GameEventType.BossDialogStart);
            while (_bossDialogging) yield return true;
        }
        else if (CurrentStageInfo.Type == StageType.Shop)
        {
            OpenDoors();
        }

        Player.FindAbility<CharacterWuXiaCombat>().UpdateUI();
        EnablePlayerProperties(true, true, true, true);
    }


    private void EnemyWaveCleared()
    {
        if (!_isPlaying) return;

        StartCoroutine(_EnemyWaveClearedCoroutine());
    }

    private IEnumerator _EnemyWaveClearedCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if (CurrentStageInfo.Reward.RewardType == StageRewardType.SkillAndSkillBooks || CurrentStageInfo.Reward.RewardType == StageRewardType.RandomSkillAndSkillBooks)
        {
            foreach (var item in CurrentStageInfo.Reward.RewardItems)
            {
                // Spawn the item.
                item.SpawnPrefab(CurrentStage.RewardSpawnPoint.position);
            }
        }

        if (CurrentStageInfo.Type == StageType.Boss || CurrentStageInfo.Type == StageType.MiniBoss)
        {
            foreach (var item in BossDrops)
            {
                item.SpawnPrefab(CurrentStage.RewardSpawnPoint.position);
            }
        }

        yield return new WaitForSeconds(2f);

        OpenDoors();
    }

    private void OpenDoors()
    {
        if (!_isPlaying) return;

        foreach (var door in CurrentStage.Doors)
        {
            door.Activate();
        }
    }


    private void OnDoorSelected(int doorID)
    {
        StartCoroutine(_ExitStageCoroutine(doorID));
    }

    protected IEnumerator _ExitStageCoroutine(int doorID)
    {
        StageDoor door = CurrentStage.Doors[doorID];
        StageInfo selectedStageInfo = _doorStageInfos[doorID];
        NextStageInfo = selectedStageInfo;

        EnablePlayerProperties(false, false, false, false);
        door.PlayExitStageCutscene(Player.gameObject);
        yield return new WaitForSeconds(1.3f);


        PlaySceneTransition(door.SceneTransitionMode);
        yield return new WaitForSeconds(0.5f);
        EnterNextStage(CurrentStageIndex + 1);
    }


    protected StageInfo GetRandomStageInfo(int stageNo, int doorNo)
    {
        StageInfo stageInfo = new();
        stageInfo.Type = StageProgression[stageNo];

        if (stageInfo.Type == StageType.NormalOrShop)
        {
            if (doorNo % 2 == 0) stageInfo.Type = StageType.Normal;
            else stageInfo.Type = StageType.Shop;
        }

        if (stageInfo.Type == StageType.Normal || stageInfo.Type == StageType.Boss || stageInfo.Type == StageType.MiniBoss)
        {
            int rewardLevel = CurrentStageIndex;
            if (stageInfo.Type == StageType.Boss || stageInfo.Type == StageType.MiniBoss) rewardLevel += 5;

            // Construct the rarity loot table
            RarityLootTable rarityLootTable = GetRewardRarityLootTable(rewardLevel);

            var stageRewardItems = new List<BaseInventoryItem>();

            for (int i = 0; i < 3; i++)
            {
                // Chose a rarity
                Rarity rarity = rarityLootTable.GetLoot().Loot;
                List<BaseInventoryItem> rewards = ResourceSystem.Instance.GetSkillAndSkillBooksCustomChance(rarity);

                /// IMPORTANT, HERE, WE MAKE THE SKILL BOOK INSTANCES. THIS SHOULD BE THE ONLY FEW PLACES TO MAKE THEM ///
                var reward = rewards.PickRandom<BaseInventoryItem>();
                if (reward is SkillBook skillBook)
                    reward = skillBook.GetInstance();
                ///// END //////

                stageRewardItems.Add(reward);
            }

            stageInfo.Reward = new();
            stageInfo.Reward.RewardType = StageRewardType.SkillAndSkillBooks;
            stageInfo.Reward.RewardItems = stageRewardItems;
        }

        return stageInfo;
    }

    public RarityLootTable GetRewardRarityLootTable(int rewardLevel)
    {
        RarityLootTable rarityLootTable = new();
        rarityLootTable.AddLoot(new RarityLoot(Rarity.Common, CommonRewardStat.GetStat(rewardLevel)));
        rarityLootTable.AddLoot(new RarityLoot(Rarity.Uncommon, UncommonRewardStat.GetStat(rewardLevel)));
        rarityLootTable.AddLoot(new RarityLoot(Rarity.Rare, RareRewardStat.GetStat(rewardLevel)));
        rarityLootTable.AddLoot(new RarityLoot(Rarity.Legendary, LegendaryRewardStat.GetStat(rewardLevel)));
        return rarityLootTable;
    }

    public void PlayerIsDead()
    {
        StartCoroutine(_PlayerIsDeadCoroutine());
    }

    public void PlaySceneTransition(SceneTransitionModes mode)
    {
        SceneTransitionAnimator.SetTrigger(mode.ToString());
    }

    private IEnumerator _PlayerIsDeadCoroutine()
    {
        // Disable Player
        EnablePlayerProperties(false, false, false, false);

        // Wait for player death animation to finish
        yield return new WaitForSeconds(1f);

        // Play a sequence of someonoe carrying the player away
        TimelineAsset timeline = (TimelineAsset)PlayerDeadDirector.playableAsset;
        AnimationTrack playerPositionTrack = (AnimationTrack)timeline.GetOutputTrack(0);
        AnimationTrack playerAnimationTrack = (AnimationTrack)timeline.GetOutputTrack(1);
        PlayerDeadDirector.SetGenericBinding(playerPositionTrack, Player.GetComponent<Animator>());
        PlayerDeadDirector.SetGenericBinding(playerAnimationTrack, Player.GetComponent<Character>().CharacterAnimator);

        Rescuer.SetActive(true);
        Rescuer.transform.position = Player.transform.position + RescuerOffset.position;

        PlayerDeadDirector.Play();

        // wait for sequence to finish
        yield return new WaitForSeconds((float)timeline.duration - 0.5f);

        PlaySceneTransition(SceneTransitionModes.DtoM);
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(_EndGame());
    }

    private void WinGame()
    {
        UnityEngine.Debug.Log("Winned game");
        StartCoroutine(_WinGameCoroutine());
    }

    private IEnumerator _WinGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(_EndGameTransition());
    }


    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quit Game");
        StartCoroutine(_EndGameTransition());
    }

    private IEnumerator _EndGameTransition()
    {
        PlaySceneTransition(SceneTransitionModes.DtoM);
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(_EndGame());
    }

    private IEnumerator _EndGame()
    {
        _isPlaying = false;

        GameManager.Instance.UnPause();

        Rescuer.SetActive(false);
        Destroy(Player.gameObject);
        Player = null;

        yield return SceneManager.LoadSceneAsync(MainMenuScene);

        PlaySceneTransition(SceneTransitionModes.MtoU);
    }


    public void OnMMEvent(GameEvent eventType)
    {
        switch (eventType.GameEventType)
        {
            case (GameEventType.EnemyWaveCleared):
                EnemyWaveCleared();
                break;
            case (GameEventType.GameWin):
                WinGame();
                break;
            case (GameEventType.PauseGameplay):
                OnPauseGamePlay();
                break;
            case (GameEventType.UnpauseGameplay):
                OnUnpauseGameplay();
                break;
            case (GameEventType.NarrativeEventStart):
                if (UIManager.HasInstance) UIManager.Instance.HideHUD();
                break;
            case (GameEventType.NarrativeEventEnd):
                if (UIManager.HasInstance) UIManager.Instance.ShowHUD();
                break;
            case (GameEventType.BossDialogEnded):
                _bossDialogging = false;
                break;
        }
    }




    public void OnMMEvent(MMLifeCycleEvent eventType)
    {
        if (Player != null && eventType.AffectedHealth == Player.GetComponent<Health>() && eventType.MMLifeCycleEventType == MMLifeCycleEventTypes.Death)
        {
            PlayerIsDead();
        }
    }

    void OnEnable()
    {
        this.MMEventStartListening<GameEvent>();
        this.MMEventStartListening<MMLifeCycleEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<GameEvent>();
        this.MMEventStopListening<MMLifeCycleEvent>();
    }



}

