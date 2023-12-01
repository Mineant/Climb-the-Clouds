using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDrawer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        if (TryGetComponent<SphereCollider>(out var sphereCollider))
        {
            Gizmos.DrawWireSphere(transform.position + sphereCollider.center, sphereCollider.radius);
        }
        else if (TryGetComponent<BoxCollider>(out var boxCollider))
        {
            Gizmos.DrawWireCube(transform.position + boxCollider.center, boxCollider.size);

        }
    }
}
