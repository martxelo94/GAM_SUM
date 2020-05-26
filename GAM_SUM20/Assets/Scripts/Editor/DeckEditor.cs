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
        if (GUILayout.Button("Rifleman Only"))
        {
            for(int i = 0; i < _target.deck_types.Length; ++i)
                _target.deck_types[i] = CardType.Riflemen;
        }
        if (GUILayout.Button("Cavalry Only"))
        {
            for (int i = 0; i < _target.deck_types.Length; ++i)
                _target.deck_types[i] = CardType.Cavalry;
        }
        if (GUILayout.Button("Tank Only"))
        {
            for (int i = 0; i < _target.deck_types.Length; ++i)
                _target.deck_types[i] = CardType.Tank;
        }
    }
}
