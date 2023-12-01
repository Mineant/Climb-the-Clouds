using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class StageDoor : ButtonActivated
{
    public int ID;
    public bool Initialized = false;
    public Action<int> OnSelect;
    public SceneTransitionModes SceneTransitionMode;
    public TMP_Text ScentText;
    public GameObject ActiveObject;
    public PlayableDirector ExitDirector;
    public int PlayerPositionAnimatorTrackIndex;
    public int PlayerAnimationAnimatorTrackIndex;
    // protected Collider _collider;

    public void Initialization(int id, StageInfo doorInfo)
    {
        Initialized = true;

        ID = id;
        OnSelect = null;

        ScentText.text = "";
        if (doorInfo.Type == StageType.Normal)
        {
            if (doorInfo.Reward.RewardType == StageRewardType.SkillAndSkillBooks)
            {
                if (doorInfo.Reward.RewardSkillTypes.Count == 0) ScentText.text = "Hmmm...";
                else ScentText.text = String.Join("\n", doorInfo.Reward.RewardSkillTypes.PickRandom<SkillType>(3));
            }
            else if (doorInfo.Reward.RewardType == StageRewardType.RandomSkillAndSkillBooks)
            {
                ScentText.text = "???";
            }
        }
        else if (doorInfo.Type == StageType.Boss) ScentText.text = "Dangerous Presence...";
        else if (doorInfo.Type == StageType.Shop) ScentText.text = "Shop";
        else if (doorInfo.Type == StageType.TopOfMountain) ScentText.text = "Scent of flowers...";
        else if (doorInfo.Type == StageType.MiniBoss) ScentText.text = "Dangerous Presence...";
        Deactivate();
    }

    public void Activate()
    {
        Activable = true;
        ActiveObject.SetActive(true);
        _collider.enabled = true;
    }

    public void Deactivate()
    {
        Activable = false;
        ActiveObject.SetActive(false);
        _collider.enabled = false;

    }

    protected override void ActivateZone()
    {
        base.ActivateZone();

        OnSelect.Invoke(ID);
    }

    public Coroutine PlayExitStageCutscene(GameObject player)
    {
        return StartCoroutine(_PlayExitStageCoroutine(player));
    }

    private IEnumerator _PlayExitStageCoroutine(GameObject player)
    {
        TimelineAsset timeline = (TimelineAsset)ExitDirector.playableAsset;
        AnimationTrack playerPositionTrack = (AnimationTrack)timeline.GetOutputTrack(PlayerPositionAnimatorTrackIndex);
        AnimationTrack playerAnimationTrack = (AnimationTrack)timeline.GetOutputTrack(PlayerAnimationAnimatorTrackIndex);
        ExitDirector.SetGenericBinding(playerPositionTrack, player.GetComponent<Animator>());
        ExitDirector.SetGenericBinding(playerAnimationTrack, player.GetComponent<Character>().CharacterAnimator);

        ExitDirector.Play();

        yield return new WaitForSeconds((float)timeline.duration);
    }

}
