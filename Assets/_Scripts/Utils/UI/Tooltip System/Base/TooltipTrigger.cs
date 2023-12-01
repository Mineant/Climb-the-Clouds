using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject CustomPointerObject;
    protected ITooltipInfoProvider _tooltipProvider;
    // protected Sequence _showSequence;
    protected Tween _tween;
    const float SHOW_DELAY = 0.5f;
    protected bool _showingTootlip;

    void Awake()
    {
        _tooltipProvider = GetComponent<ITooltipInfoProvider>();

        if (_tooltipProvider == null) Debug.LogWarning($"No tooltip providers on {this.gameObject.name}, won't have tooltips for the object.");

        if (CustomPointerObject != null)
        {
            EventTrigger otherEventTrigger = CustomPointerObject.GetOrAddComponent<EventTrigger>();

            // Add a listener for the OnPointerEnter event
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { OnPointerEnterDelegate((PointerEventData)data); });
            otherEventTrigger.triggers.Add(enterEntry);

            // Add a listener for the OnPointerExit event
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { OnPointerExitDelegate((PointerEventData)data); });
            otherEventTrigger.triggers.Add(exitEntry);
        }
    }

    private void ShowTooltip()
    {
        if (_tooltipProvider == null) return;

        _showingTootlip = true;
        _tween = DOVirtual
        .DelayedCall(SHOW_DELAY, () => TooltipSystem.Show(_tooltipProvider))
        .Play();
    }

    private void HideTooltip()
    {
        if (_tooltipProvider == null) return;

        _showingTootlip = false;
        _tween.Kill();
        TooltipSystem.Hide(_tooltipProvider);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CustomPointerObject != null) return;

        ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CustomPointerObject != null) return;

        HideTooltip();
    }

    void OnPointerEnterDelegate(PointerEventData data)
    {
        ShowTooltip();
    }

    void OnPointerExitDelegate(PointerEventData data)
    {
        HideTooltip();
    }

    void OnDisable()
    {
        if (_showingTootlip) HideTooltip();
    }
}
