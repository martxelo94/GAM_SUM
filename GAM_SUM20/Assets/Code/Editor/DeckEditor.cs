using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Deck))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Deck _target = (Deck)target;
        if(_target.deck_types == null ||_target.deck_types.Length != (int)CardType.CardType_Count)
        {
            if (GUILayout.Button("Initialize"))
            {
                _target.InitCardTypes();
            }
        }
        else
        {
            if (GUILayout.Button("Clear"))
            {
                _target.ClearCardTypes();
            }
            if (GUILayout.Button("Randomize"))
            {
                _target.UpdateCardCount();
                _target.Randomize();
            }
            if (GUILayout.Button("Rifleman Only"))
            {
                _target.UpdateCardCount();
                for (int i = 0; i < (int)CardType.Riflemen; ++i)
                    _target.deck_types[i].count = 0;
                _target.deck_types[(int)CardType.Riflemen].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Riflemen + 1; i < _target.deck_types.Length; ++i)
                    _target.deck_types[i].count = 0;

            }
            if (GUILayout.Button("Cavalry Only"))
            {
                _target.UpdateCardCount();
                for (int i = 0; i < (int)CardType.Cavalry; ++i)
                    _target.deck_types[i].count = 0;
                _target.deck_types[(int)CardType.Cavalry].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Cavalry + 1; i < _target.deck_types.Length; ++i)
                    _target.deck_types[i].count = 0;
            }
            if (GUILayout.Button("Tank Only"))
            {
                _target.UpdateCardCount();
                for (int i = 0; i < (int)CardType.Tank; ++i)
                    _target.deck_types[i].count = 0;
                _target.deck_types[(int)CardType.Tank].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Tank + 1; i < _target.deck_types.Length; ++i)
                    _target.deck_types[i].count = 0;
            }

        }

    }
}
