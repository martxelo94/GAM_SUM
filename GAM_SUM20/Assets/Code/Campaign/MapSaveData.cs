using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public struct CardTypeCount
{
    public CardType type;
    public int count;

    public static CardTypeCount[] Initialize()
    {
        CardTypeCount[] deck = new CardTypeCount[(int)CardType.CardType_Count];
        for (int i = 0; i < (int)CardType.CardType_Count; ++i) {
            deck[i].type = (CardType)i;
            deck[i].count = 0;
        }
        return deck;
    }
}

[System.Serializable]
public struct NodeSaveData
{
    public string name;
    public CardTypeCount[] deck;
    public TeamType team;
    public int model_idx;

    public NodeSaveData(MapNode node)
    {
        name = node.gameObject.name;
        model_idx = node.army_model_idx;
        if (node.army != null)
            deck = node.army.GetDeck();
        else deck = null;
        team = node.team;
    }
}

[System.Serializable]
public class MapSaveData
{
    public int attack_idx;
    public int target_idx;
    public NodeSaveData[] nodes;

    public MapSaveData(MapNode[] _nodes, int attack, int target)
    {
        attack_idx = attack;
        target_idx = target;
        nodes = new NodeSaveData[_nodes.Length];
        for (int i = 0; i < _nodes.Length; ++i)
            nodes[i] = new NodeSaveData(_nodes[i]);
        //System.Comparison<NodeSaveData> comp = (a, b) => a.name.CompareTo(b.name);
        //System.Array.Sort(nodes, comp);
    }
    public void UpdateBattleDecks(CardTypeCount[] attacker, CardTypeCount[] defender)
    {
        Assert.IsTrue(attack_idx != -1 && target_idx != -1);
        nodes[attack_idx].deck = attacker;
        nodes[target_idx].deck = defender;
    }
}

