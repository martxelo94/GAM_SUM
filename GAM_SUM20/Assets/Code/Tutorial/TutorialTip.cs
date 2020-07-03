using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialTip : MonoBehaviour
{
    public Transform[] highlighted_objects;
    [HideInInspector]
    public List<Vector3> original_highlighted_scales;

    public Collider2D[] inactive_colliders;

    public void Enter()
    {
        if(inactive_colliders != null)
            foreach (Collider2D c in inactive_colliders)
            {
                if (c == null)
                    continue;
                c.enabled = false;
            }
    }

    public void Exit()
    {
        if (inactive_colliders != null)
            foreach (Collider2D c in inactive_colliders)
            {
                if (c == null)
                    continue;
                c.enabled = true;
            }
    }
}
