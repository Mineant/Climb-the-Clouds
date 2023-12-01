using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

public class SkillBookDrop : MonoBehaviour, MMEventListener<SkillBookEvent>
{
    public GameObject DropMeObject;
    
    public void OnMMEvent(SkillBookEvent eventType)
    {

        switch (eventType.SkillBookEventType)
        {
            case (SkillBookEventType.StartDrag):
                if (DropMeObject != null) DropMeObject.SetActive(true);
                break;
            case (SkillBookEventType.EndDrag):
                if (DropMeObject != null) DropMeObject.SetActive(false);
                break;
        }
    }
    void OnEnable()
    {
        this.MMEventStartListening<SkillBookEvent>();
    }

    void OnDisable()
    {
        this.MMEventStopListening<SkillBookEvent>();
    }

}
