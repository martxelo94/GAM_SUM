using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Deck))]
public class DeckEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Deck _target = (Deck)target;

        GUILayout.Label("Card Count = " + _target.cards_to_play_count.ToString());

        if(_target.GetDeck() == null ||_target.GetDeck().Length != (int)CardType.CardType_Count)
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
                CardTypeCount[] tmp = _target.GetDeck();
                for (int i = 0; i < (int)CardType.Riflemen; ++i)
                    tmp[i].count = 0;
                tmp[(int)CardType.Riflemen].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Riflemen + 1; i < tmp.Length; ++i)
                    tmp[i].count = 0;
                _target.SetDeck(tmp);
            }
            if (GUILayout.Button("Cavalry Only"))
            {
                CardTypeCount[] tmp = _target.GetDeck();
                for (int i = 0; i < (int)CardType.Cavalry; ++i)
                    tmp[i].count = 0;
                tmp[(int)CardType.Cavalry].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Cavalry + 1; i < tmp.Length; ++i)
                    tmp[i].count = 0;
                _target.SetDeck(tmp);
            }
            if (GUILayout.Button("Tank Only"))
            {
                CardTypeCount[] tmp = _target.GetDeck();
                for (int i = 0; i < (int)CardType.Tank; ++i)
                    tmp[i].count = 0;
                tmp[(int)CardType.Tank].count = _target.cards_to_play_count;
                for (int i = (int)CardType.Tank + 1; i < tmp.Length; ++i)
                    tmp[i].count = 0;
                _target.SetDeck(tmp);
            }

        }

    }
}
