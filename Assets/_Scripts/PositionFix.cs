using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFix : MonoBehaviour
{
    public Vector3 Position;

    void LateUpdate()
    {
        transform.localPosition = Position;
    }
}
