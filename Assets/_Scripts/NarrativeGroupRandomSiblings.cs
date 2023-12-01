using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NarrativeGroupRandomSiblings : MonoBehaviour
{
    public List<GameObject> RandomizableNarrativeEvents;
    private Transform _parent;

    void Awake()
    {
        _parent = RandomizableNarrativeEvents[0].transform;
    }

    void OnEnable()
    {
        // Count the difference
        int diff = _parent.childCount - RandomizableNarrativeEvents.Count;  // 3, 2,
        RandomizableNarrativeEvents = RandomizableNarrativeEvents.Shuffle().ToList();
        for (int i = 0; i < RandomizableNarrativeEvents.Count; i++)
        {
            RandomizableNarrativeEvents[i].transform.SetSiblingIndex(diff + i);
        }
    }
}
