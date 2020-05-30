using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;

public class MapCampaign : MonoBehaviour
{

    public int battleLevelBuildIdx = 2;

    [HideInInspector]
    public MapNode selected_node;
    [HideInInspector]
    public bool is_attaking { get; private set; } = false;
    [HideInInspector]
    public bool is_moving { get; private set; } = false;
    MapNode target_node;
    public float movement_duration = 2.0f;
    private MapNode[] nodes;

    public CampaignMenu menu;

    private void Awake()
    {
        if(menu == null)
            menu = GetComponent<CampaignMenu>();
        nodes = FindObjectsOfType<MapNode>();
        System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
        System.Array.Sort(nodes, comp);
        for (int i = 0; i < nodes.Length; ++i)
            nodes[i].node_idx = i;
        LoadFile();
        ConfirmBattleResult();
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveFile();
    }

    private void OnDestroy()
    {
        //SaveFile();
    }

    // Update is called once per frame
    void Update()
    {
        //if (selected_node != null)
        //    FollowGUI();
    }

    void ConfirmBattleResult()
    {
        // load scene
        if (GameSettings.INSTANCE.IsBattle())
        {
            // update decks
            Deck attack_army = nodes[GameSettings.INSTANCE.attack_idx].army;
            Assert.IsTrue(attack_army != null);
            attack_army.deck_types = GameSettings.INSTANCE.attack_deck;

            Assert.IsTrue(GameSettings.INSTANCE.target_idx > -1);
            Deck target_army = nodes[GameSettings.INSTANCE.target_idx].army;
            Assert.IsTrue(GameSettings.INSTANCE.target_deck != null);
            target_army.deck_types = GameSettings.INSTANCE.target_deck;

            // win reward
            if (GameSettings.INSTANCE.last_battle_won)
            {
                // capture node
                CaptureNode(nodes[GameSettings.INSTANCE.attack_idx], nodes[GameSettings.INSTANCE.target_idx]);
            }
            else
            {
                // update nodes

            }
        }
    }

    // update with battle result
    public static bool UpdateFile()
    {
        string path = Application.persistentDataPath + "/" + GameSettings.INSTANCE.campaign_battle_name;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MapSaveData data = formatter.Deserialize(stream) as MapSaveData;

            Assert.IsTrue(GameSettings.INSTANCE.IsBattle());
            // update with battle result
            //if (GameSettings.INSTANCE.IsBattle())
            {
                data.nodes[GameSettings.INSTANCE.attack_idx].deck = GameSettings.INSTANCE.attack_deck;
                data.nodes[GameSettings.INSTANCE.target_idx].deck = GameSettings.INSTANCE.target_deck;

                formatter.Serialize(stream, data);

                return true;
            }

        }

        return false;

    }

    public static void DeleteFile()
    {
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name;
        File.Delete(path);
    }

    public void SaveFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name;
        FileStream stream = new FileStream(path, FileMode.Create);

        if (nodes == null) {
            nodes = FindObjectsOfType<MapNode>();
            System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
            System.Array.Sort(nodes, comp);
            for (int i = 0; i < nodes.Length; ++i)
                nodes[i].node_idx = i;
        }

        MapSaveData data = new MapSaveData(nodes);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public bool LoadFile()
    {
        string path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MapSaveData data = formatter.Deserialize(stream) as MapSaveData;

            // set saved data
            if (nodes.Length != data.nodes.Length)
                return false;
            Assert.IsTrue(nodes.Length == data.nodes.Length);   // redundant

            // shorted at Awake
            //System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
            //System.Array.Sort(nodes, comp);
            
            // set data from same index
            for (int i = 0; i < nodes.Length; ++i) {
                nodes[i].Set(data.nodes[i]);
            }

            return true;
        }
        else {
            return false;
        }
    }


    void CaptureNode(MapNode attaker, MapNode target)
    {
        if (attaker.army != null) {
            if (target.army != null) {
                // combine decks
                attaker.army.CombineCards(target.army.deck_types);
                Destroy(target.army.gameObject);
            }
            // replace
            target.army = attaker.army;
            attaker.army = null;
            //Destroy(attaker.army.gameObject);
            target.army.transform.parent = target.transform;
            target.army.transform.position = target.transform.position;
            target.army_model_idx = attaker.army_model_idx;
            attaker.army_model_idx = -1;
        }
        // set team
        target.team = attaker.team;
        // update links if need
        attaker.UpdateLinks();
        target.UpdateLinks();
        attaker.SetTeamColor();
        target.SetTeamColor();    // done in Start of Node, but might update if captured empty node


        // if target node is dead end, then campaign finished
        if (target.nextNodes.Length == 0) {
            menu.ShowEndPanel(true);
        }
    }

    public void UnselectNode()
    {
        target_node = null;
        if(selected_node != null)
            selected_node.Unselect();
        menu.ShowArmyPanel(false);
        menu.ShowNodePanel(false);
        menu.LockBattlePanel(false);
        is_attaking = false;

    }
    public void SelectNode(MapNode node)
    {
        UnselectNode();
        selected_node = node;
        selected_node.Select();
        if (selected_node.army != null && selected_node.team == TeamType.Player)
        {
            menu.ShowArmyPanel(true);
            if (selected_node.army.cards_to_play_count == 0)
            {
                menu.ShowMoveButton(false);
            }
            else
                menu.ShowMoveButton(true);
            //selectedArmyPanel.transform.position = selected_node.transform.position + Vector3.back * 100;
        }
        else
        {
            menu.ShowNodePanel(true);
            //selectedNodePanel.transform.position = selected_node.transform.position + Vector3.back * 100;
        }
    }

    #region ATTACK PHASE
    public void BegineBattle(MapNode attacker, MapNode target)
    {
        GameSettings.INSTANCE.SetBattle(attacker, target);
        SaveFile();
        // load battle map
        GameSettings.INSTANCE.nextSceneIdx = battleLevelBuildIdx;
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }
    public void ToggleAttack() { is_attaking = !is_attaking; }
    public bool ShowBattlePanel(MapNode target)
    {
        // check if adyacent
        bool found = false;
        Assert.IsTrue(selected_node != null);

        foreach (MapNode n in selected_node.nextNodes)
            if (n == target)
            {
                found = true;
                break;
            }
        if (found == false)
        {
            UnselectNode();
            SelectNode(target);
            is_attaking = false;
            return false;
        }

        target_node = target;
        menu.LockBattlePanel(true);

        return true;
    }

    public void MoveArmy()
    {
        Assert.IsTrue(selected_node.army != null);

        // move army
        StartCoroutine(MoveArmyAnimaton(movement_duration));

        menu.ShowBattlePanel(false);
        menu.ShowArmyPanel(false);
        is_attaking = false;
    }

    IEnumerator MoveArmyAnimaton(float duration)
    {
        is_moving = true;
        // calculate frame duration
        int frames = (int)(duration * (1.0f / Time.deltaTime));
        Vector3 step = (target_node.transform.position - selected_node.transform.position) / frames;
        for (int i = 0; i < frames; ++i)
        {
            selected_node.army.transform.position += step;
            yield return null;
        }

        if (target_node.army != null)
        {
            BegineBattle(selected_node, target_node);
        }
        else
        {
            // capture node
            CaptureNode(selected_node, target_node);
        }
        is_moving = false;
    }

    public void FollowGUI()
    {
        menu.selectedArmyPanel.transform.position = selected_node.transform.position + Vector3.back * 100;
        menu.selectedNodePanel.transform.position = selected_node.transform.position + Vector3.back * 100;
    }
    #endregion

    public void AddRandomCardsToSelectedDeck(int count)
    {
        if (selected_node == null || selected_node.army == null)
            return;
        Deck deck = selected_node.army;

        for (int i = 0; i < count; ++i)
        {
            deck.AddToDeck((CardType)GameSettings.INSTANCE.randomizer.Next(0, (int)CardType.CardType_Count), 1);
        }
        deck.UpdateText();
    }
}
