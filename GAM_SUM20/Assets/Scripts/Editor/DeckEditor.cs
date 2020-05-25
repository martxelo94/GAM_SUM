using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Deck))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Deck _target = (Deck)target;
        if (GUILayout.Button("Randomize"))
        {
            _target.Randomize();
        }
    }
}
