using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public abstract class Tooltip : MonoBehaviour
{
    public abstract bool Show<T>(T tooltipProvider) where T : ITooltipInfoProvider;
    public abstract bool Hide<T>(T tooltipProvider) where T : ITooltipInfoProvider;
}

public abstract class Tooltip<TInfo> : Tooltip where TInfo : TooltipInfo
{
    protected Sequence _moveSequence;
    protected Tween _showTween;
    protected RectTransform _rectTransform;
    protected CanvasGroup _canvasGroup;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Update()
    {
        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        float finalPivotX = 0f;
        float finalPivotY = 0f;

        //If mouse on left of screen move tooltip to right of cursor and vice vera
        if (pivotX < 0.5) finalPivotX = -0.1f;
        else finalPivotX = 1.01f;


        //If mouse on lower half of screen move tooltip above cursor and vice versa
        if (pivotY < 0.5) finalPivotY = 0;
        else finalPivotY = 1;

        Vector2 finalPivot = new Vector2(finalPivotX, finalPivotY);
        _rectTransform.pivot = finalPivot;

        // if (_rectTransform.pivot != finalPivot)
        // {
        //     _moveSequence.Kill();

        //     _moveSequence = DOTween.Sequence()
        //     .Join(DOTween.To(() => _rectTransform.pivot, x => _rectTransform.pivot = x, new Vector2(finalPivotX, finalPivotY), .5f))
        //     .Join(DOTween.To(() => _rectTransform.pivot, y => _rectTransform.pivot = y, new Vector2(finalPivotX, finalPivotY), 1f))
        //     .SetRelative(false)
        //     .SetUpdate(true)
        //     .Play();
        // }

        transform.position = position;
    }


    /// <summary>
    /// This will check if the infoprovider has the same info type as the tooltip's info type. If it is, it will show the info.
    /// </summary>
    public sealed override bool Show<T>(T infoProvider)
    {
        // See if the infoProvider has the info type that I want
        if (!(infoProvider is ITooltipInfoProvider<TInfo>)) return false;

        // So now that I know it is the provider I want, I can cast it into the desired provider, and get its info, and show it
        Show(((ITooltipInfoProvider<TInfo>)(infoProvider)).GetTooltipInfo());

        return true;
    }

    public sealed override bool Hide<T>(T infoProvider)
    {
        if (!(infoProvider is ITooltipInfoProvider<TInfo>)) return false;

        Hide();

        return true;
    }

    public virtual void Show(TInfo info)
    {
        this.gameObject.SetActive(true);

        _canvasGroup.alpha = 0f;
        _showTween = DOTween
        .To(() => _canvasGroup.alpha, a => _canvasGroup.alpha = a, 1f, 0.2f)
        .SetEase(Ease.InExpo)
        .SetUpdate(true)
        .Play();
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);

        _showTween.Kill();
    }
}
