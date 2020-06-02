using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitStats))]
public class UnitStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnitStats _target = (UnitStats)target;
        EditorUtility.SetDirty(_target);

        //EditorGUILayout.LabelField("Prefered Target Types");

        // display preferedTargetUnit BitArray
        for (int i = 0; i < (int)CardType.CardType_Count; ++i)
        {
            ulong mask = (ulong)1 << i;
            bool val = mask == (_target.preferedTargetUnit & mask);
            bool new_val = EditorGUILayout.Toggle(((CardType)i).ToString(), val);
            if (new_val != val) {
                if (new_val) // set
                    _target.preferedTargetUnit |= mask;
                else // unset
                    _target.preferedTargetUnit &= ~mask;
                Debug.Log("Changed flag");
            }
        }
    }
}
