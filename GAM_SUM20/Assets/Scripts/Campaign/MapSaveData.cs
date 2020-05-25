using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NodeSaveData
{
    public string name;
    public CardType[] deck = null;
    public TeamType team = TeamType.None;
    public int model_idx = -1;

    public NodeSaveData(MapNode node)
    {
        name = node.gameObject.name;
        model_idx = node.army_model_idx;
        if (node.army != null)
            deck = node.army.deck_types;
        team = node.team;
    }
}

[System.Serializable]
public class MapSaveData
{
    public NodeSaveData[] nodes;

    public MapSaveData(MapNode[] _nodes)
    {
        nodes = new NodeSaveData[_nodes.Length];
        for (int i = 0; i < _nodes.Length; ++i)
            nodes[i] = new NodeSaveData(_nodes[i]);
        //System.Comparison<NodeSaveData> comp = (a, b) => a.name.CompareTo(b.name);
        //System.Array.Sort(nodes, comp);
    }
}

