using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

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
    public bool is_attacking { get; private set; } = false;
    [HideInInspector]
    public bool is_moving { get; private set; } = false;
    MapNode target_node;
    public float movement_duration = 2.0f;
    private MapNode[] nodes;

    public GameObject[] armyPrefabs;

    public CampaignMenu menu;

    private void Awake()
    {
        if(menu == null)
            menu = GetComponent<CampaignMenu>();
        nodes = FindObjectsOfType<MapNode>();
        System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
        System.Array.Sort(nodes, comp);
        for (int i = 0; i < nodes.Length; ++i) {
            nodes[i].map = this;
            nodes[i].node_idx = i;
        }
        LoadFile();
        ConfirmBattleResult();
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveFile();

        // select node
        foreach (MapNode n in nodes)
        {
            if (n.army != null && n.army.team == TeamType.Player)
            {
                SelectNode(n);
                break;
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        //if (selected_node != null)
        //    FollowGUI();
        // if target node is dead end, then campaign finished

    }

    public List<Deck> GetArmiesOfTeam(TeamType team)
    {
        List<Deck> armies = new List<Deck>();
        foreach (MapNode n in nodes) {
            if (n.army != null && n.army.team == team)
                armies.Add(n.army);
        }
        return armies;
    }

    void ConfirmBattleResult()
    {
        // load scene
        if (GameSettings.INSTANCE.IsBattle())
        {
            // update decks
            Deck attack_army = nodes[GameSettings.INSTANCE.attack_idx].army;
            Assert.IsTrue(attack_army != null);
            attack_army.SetDeck(GameSettings.INSTANCE.attack_deck);

            Assert.IsTrue(GameSettings.INSTANCE.target_idx > -1);
            Deck target_army = nodes[GameSettings.INSTANCE.target_idx].army;
            Assert.IsTrue(GameSettings.INSTANCE.target_deck != null);
            target_army.SetDeck(GameSettings.INSTANCE.target_deck);

            // win reward
            if (GameSettings.INSTANCE.last_battle_won)
            {
                // capture node
                CaptureNode(nodes[GameSettings.INSTANCE.attack_idx], nodes[GameSettings.INSTANCE.target_idx]);

            }
            else
            {
                // update nodes

                // get reward for opponent
                MapNode targetNode = nodes[GameSettings.INSTANCE.target_idx];
                Assert.IsTrue(targetNode.army != null);
                targetNode.army.CombineCards(targetNode.deck_reward);

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
        Assert.IsTrue(attaker.army_model_idx != -1);
        if (attaker.army != null)
        {
            if (target.army != null)
            {
                // combine decks
                attaker.army.CombineCards(target.army.GetDeck());
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

        // get reward
        Assert.IsTrue(target.army != null);
        target.army.CombineCards(target.deck_reward);

    }

    public void UnselectNode()
    {
        target_node = null;
        if(selected_node != null)
            selected_node.Unselect();
        menu.ShowArmyPanel(false);
        menu.ShowNodePanel(false);
        menu.ShowBattlePanel(false);
    }
    public void SelectNode(MapNode node)
    {
        if (is_attacking)
        {
            Assert.IsTrue(selected_node != null);
            // select target node
            if (node.army != null)
            {
                if (node.army.team == TeamType.Player)
                {
                    // check that selected is in children
                    for (int i = 0; i < node.nextNodes.Length; ++i)
                    {
                        if (node.nextNodes[i] == selected_node)
                        {
                            target_node = node;
                            ShowBattlePanel(true);
                            return;
                        }
                    }
                }
            }
        }
        UnselectNode();
        selected_node = node;
        selected_node.Select();


        if (selected_node != null && selected_node.nextNodes.Length == 0 && selected_node.team == TeamType.Player)
        {
            menu.ShowEndPanel(true);
        }
        else
            menu.ShowEndPanel(false);

        menu.ShowNodePanel(true);

        if (selected_node.army != null)
        {
            
            if(selected_node.team == TeamType.Player)
            {
                //if (selected_node.army.cards_to_play_count == 0)
                {
                    menu.ShowArmyPanel(true);
                    menu.ShowMoveButton(false);

                    menu.ShowBattlePanel(false);
                    menu.ShowNodePanel(false);
                }
                
            }
            else
            {
                foreach (MapNode n in selected_node.parentNodes)
                {
                    if (n.army != null && n.army.team == TeamType.Player && n.army.cards_to_play_count > 0)
                    {
                        // show move button
                        menu.ShowArmyPanel(true);
                        menu.ShowMoveButton(true);
                        menu.ShowNodePanel(false);

                        break;
                    }
                }
            }

        }
            // if node has enemy army adyacent to player army
        else
        {
            foreach (MapNode n in selected_node.parentNodes)
            {
                if (n.army != null && n.army.team == TeamType.Player)
                {
                    // show move button
                    menu.ShowArmyPanel(true);
                    menu.ShowMoveButton(true);
                    menu.ShowNodePanel(false);
                    break;
                }
            }
        }

    }

    #region ATTACK PHASE
    public void CheckAttack()
    {
        Assert.IsTrue(selected_node != null);
        // check if only one army to attack
        int army_count = 0;
        MapNode armyNode = null;
        foreach (MapNode n in selected_node.parentNodes)
        {
            if (n.army != null && n.army.team == TeamType.Player) {
                army_count++;
                armyNode = n;
            }
        }
        // show directly confirm button or start attaking phase (choose army)
        Assert.IsTrue(army_count > 0);
        if (army_count > 1)
        {
            is_attacking = true;
        }
        else {
            // show confirm panel
            target_node = armyNode;
            menu.ShowBattlePanel(true);
            menu.ShowArmyPanel(false);
        }
    }

    public void BegineBattle(MapNode attacker, MapNode target)
    {
        GameSettings.INSTANCE.SetBattle(attacker, target);
        SaveFile();
        // load battle map
        GameSettings.INSTANCE.nextSceneIdx = battleLevelBuildIdx;
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }
    public bool ShowBattlePanel(bool show)
    {
        // check if adyacent
        Assert.IsTrue(selected_node != null);

        menu.battlePanel.SetActive(show);

        return true;
    }

    public void MoveArmy()
    {
        Assert.IsTrue(selected_node != null);
        Assert.IsTrue(target_node != null);

        // move army
        StartCoroutine(MoveArmyAnimaton(movement_duration));

        menu.ShowBattlePanel(false);
        menu.ShowArmyPanel(false);
    }

    IEnumerator MoveArmyAnimaton(float duration)
    {
        is_moving = true;
        // calculate frame duration
        int frames = (int)(duration * (1.0f / Time.deltaTime));
        Vector3 step = (selected_node.transform.position - target_node.transform.position) / frames;
        for (int i = 0; i < frames; ++i)
        {
            target_node.army.transform.position += step;
            yield return null;
        }

        if (selected_node.army != null)
        {
            BegineBattle(target_node, selected_node);
        }
        else
        {
            // capture node
            CaptureNode(target_node, selected_node);
            SaveFile();
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

    public void SetActiveNodes(bool active)
    {
        for (int i = 0; i < nodes.Length; ++i)
            nodes[i].enabled = active;
    }
}
