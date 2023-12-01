using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using MoreMountains.TopDownEngine;
public class Stage : MonoBehaviour
{
    public List<StageDoor> Doors;
    public PlayableDirector EnterStageDirector;
    public int PlayerPositionAnimatorTrackIndex;
    public int PlayerAnimationAnimatorTrackIndex;
    public Transform EnterStageStartPoint;
    public Transform EnterStageEndPoint;
    public Transform RewardSpawnPoint;
    public List<StageObject> StageObjects;

    public static Stage FindStage()
    {
        return FindObjectOfType<Stage>();
    }

    public Coroutine PlayEnterStageAnimation(GameObject player)
    {
        return StartCoroutine(_PlayEnterStageAnimationCoroutine(player));
    }

    private IEnumerator _PlayEnterStageAnimationCoroutine(GameObject player)
    {

        TimelineAsset timeline = (TimelineAsset)EnterStageDirector.playableAsset;
        AnimationTrack playerPositionTrack = (AnimationTrack)timeline.GetOutputTrack(PlayerPositionAnimatorTrackIndex);
        AnimationTrack playerAnimationTrack = (AnimationTrack)timeline.GetOutputTrack(PlayerAnimationAnimatorTrackIndex);
        EnterStageDirector.SetGenericBinding(playerPositionTrack, player.GetComponent<Animator>());
        EnterStageDirector.SetGenericBinding(playerAnimationTrack, player.GetComponent<Character>().CharacterAnimator);

        player.transform.position = EnterStageStartPoint.position;
        EnterStageDirector.Play();

        yield return new WaitForSeconds((float)timeline.duration);

        player.transform.position = EnterStageEndPoint.transform.position;
    }

}

[System.Serializable]
public class StageObject
{
    public StageType StageType;
    public List<GameObject> ObjectGroups;

    public void ActivateRandomObjectGroup()
    {
        ObjectGroups.PickRandom<GameObject>().SetActive(true);
    }
}
