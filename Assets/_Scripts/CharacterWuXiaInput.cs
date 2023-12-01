using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class CharacterWuXiaInput : CharacterAbility, MMEventListener<InventoryDragAndDropEvent>
{
    bool _isPointerOverUIElement;
    bool _isBuffering { get { return _bufferEndTime >= _currentTime; } }
    float _bufferEndTime = Mathf.NegativeInfinity;
    float _bufferTime = 0.5f;
    float _currentTime = 0f;
    KeyCode _bufferKey;
    bool _dragging;

    public override void EarlyProcessAbility()
    {
        _isPointerOverUIElement = Helpers.IsPointerOverUIElement();
        _currentTime = Time.time;

        if (!_isPointerOverUIElement && !_dragging)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _bufferKey = KeyCode.Mouse0;
                UpdateBufferEndTime(_currentTime + _bufferTime);
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                _bufferKey = KeyCode.Mouse0;
                UpdateBufferEndTime(_currentTime);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _bufferKey = KeyCode.Mouse1;
                UpdateBufferEndTime(_currentTime + _bufferTime);
            }
            else if (Input.GetKey(KeyCode.Mouse1))
            {
                _bufferKey = KeyCode.Mouse1;
                UpdateBufferEndTime(_currentTime);
            }
        }


        base.EarlyProcessAbility();

        void UpdateBufferEndTime(float time)
        {
            if (time >= _bufferEndTime) _bufferEndTime = time;
        }
    }

    public bool GetNormalAttackButtonDown()
    {
        if (InputManager.Instance.InputDetectionActive && _isBuffering && _bufferKey == KeyCode.Mouse0)
        {
            return true;
        }

        return false;
    }

    public bool GetAuraAttackButtonDown()
    {
        if (InputManager.Instance.InputDetectionActive && _isBuffering && _bufferKey == KeyCode.Mouse1)
        {
            return true;
        }

        return false;
    }

    internal void ResetBuffer()
    {
        _bufferEndTime = Time.time;
    }

    public void OnMMEvent(InventoryDragAndDropEvent eventType)
    {
        switch (eventType.EventType)
        {
            case (InventoryDragAndDropEventType.BeginDrag):
                _dragging = true;
                break;
            case (InventoryDragAndDropEventType.EndDrag):
                _dragging = false;
                break;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<InventoryDragAndDropEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<InventoryDragAndDropEvent>();
    }
}

// public class InputThing
// {
//     public KeyCode Key;
//     public bool IsBuffering;
//     public float BufferEndTime;
//     public float BufferTime;

//     public bool UseInput()
//     {
//         if (!IsBuffering)
//         {
//             if (Input.GetKeyDown(Key))
//             {

//             }
//         }
//     }

//     public bool ExtendBuffer(bool requireKeyDown)
//     {
//         if (requireKeyDown && !Input.GetKeyDown(Key)) return false;

//         IsBuffering = true;
//         BufferEndTime = Time.time + BufferTime;

//         return true;
//     }
// }