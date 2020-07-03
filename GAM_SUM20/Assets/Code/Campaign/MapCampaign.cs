﻿using System.Collections;
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
    private bool saved_campaign = false;
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
    public RewardFlipCard rewardCardPrefab;
    public Transform rewardPanel;


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
        if (GameSettings.INSTANCE.IsBattle())
            ConfirmBattleResult();
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveFile();
        saved_campaign = false;

#if false
        // select node
        foreach (MapNode n in nodes)
        {
            if (n.army != null && n.army.team == TeamType.Player)
            {
                SelectNode(n);
                break;
            }
        }
#endif
    }



    // Update is called once per frame
    void Update()
    {
        //if (selected_node != null)
        //    FollowGUI();
        // if target node is dead end, then campaign finished

    }


    private void OnApplicationQuit()
    {
        if (saved_campaign == false)
            SaveFile();
        //Debug.Log(this.ToString() + " has been disabled.");
    }

    public void SetActiveNodeTriggers(bool active)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].circleCollider2D.enabled = active;
        }
    }
    public void SetActiveNodes(bool active)
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].enabled = active;
        }
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
        // update decks
        Deck attack_army = nodes[GameSettings.INSTANCE.attack_idx].army;
        Assert.IsTrue(attack_army != null);
        attack_army.SetDeck(GameSettings.INSTANCE.CopyAttackDeck());

        Assert.IsTrue(GameSettings.INSTANCE.target_idx > -1);
        Deck target_army = nodes[GameSettings.INSTANCE.target_idx].army;
        target_army.SetDeck(GameSettings.INSTANCE.CopyTargetDeck());

        // update editor
        menu.deckManager.SetUpDeck(target_army);
        menu.deckManager.SaveDeck();

        MapNode targetNode = nodes[GameSettings.INSTANCE.target_idx];

        if (GameSettings.INSTANCE.last_battle_won)
        {
            // capture node
            CaptureNode(nodes[GameSettings.INSTANCE.attack_idx], targetNode);
        }

        // give reward to target node

        Assert.IsTrue(targetNode.army != null);

        //targetNode.army.CombineCards(targetNode.deck_reward); // no, reward is added to the pool

        StartCoroutine(RewardAnimation(targetNode.deck_reward));
        
    }

    bool IsEveryCardFliped(List<RewardFlipCard> rewardFlipCards)
    {
        int fliped_count = 0;
        foreach (RewardFlipCard c in rewardFlipCards) {
            if (c.HasFliped())
                fliped_count++;
        }

        return fliped_count == rewardFlipCards.Count;
    }

    IEnumerator RewardAnimation(CardTypeCount[] reward)
    {
        SetActiveNodeTriggers(false);
        yield return null;

        Vector3 initScale = rewardPanel.parent.localScale;
        rewardPanel.parent.localScale = Vector3.zero;
        rewardPanel.parent.gameObject.SetActive(true);

        List<RewardFlipCard> rewardFlipCards = new List<RewardFlipCard>();

        // add cards to reward
        for (int i = 0; i < reward.Length; ++i)
        {
            if (reward[i].count == 0)
                continue;
            RewardFlipCard card = Instantiate(rewardCardPrefab, rewardPanel) as RewardFlipCard;
            card.reward = reward[i];
            card.manager = menu.deckManager;

            rewardFlipCards.Add(card);
        }

        int frames = (int)(0.5f / Time.deltaTime);
        for(int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(Vector3.zero, initScale, (float)(i + 1) / frames);
            yield return null;
        }

        // wait for every card to be flipped
        while(!IsEveryCardFliped(rewardFlipCards))
        {
            yield return new WaitForSecondsRealtime(1f);
        }
        // Remove cards
        foreach (RewardFlipCard c in rewardFlipCards)
            Destroy(c.gameObject);

        // reverse scale
        for (int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(initScale, Vector3.zero, (float)(i + 1) / frames);
            yield return null;
        }
        rewardPanel.parent.localScale = initScale;
        rewardPanel.parent.gameObject.SetActive(false);

        SetActiveNodeTriggers(true);

    }

    // update with battle result
    public static bool UpdateDecksInFile(CardTypeCount[] attaker, CardTypeCount[] defender)
    {
        string path = Application.persistentDataPath + "/" + GameSettings.INSTANCE.campaign_battle_name;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MapSaveData data = formatter.Deserialize(stream) as MapSaveData;

            Assert.IsTrue(GameSettings.INSTANCE.IsBattle());
            data.UpdateBattleDecks(attaker, defender);
            formatter.Serialize(stream, data);
            return true;
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
        int attaker_idx = -1;
        int defender_idx = -1;
        if (target_node != null && selected_node != null)
        {
            for (int i = 0; i < nodes.Length; ++i)
            {
                if (nodes[i] == selected_node)
                    defender_idx = i;
                if (nodes[i] == target_node)
                    attaker_idx = i;
            }
        }

        MapSaveData data = new MapSaveData(nodes, attaker_idx, defender_idx);

        formatter.Serialize(stream, data);
        stream.Close();

        saved_campaign = true;
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
            if (nodes.Length != data.nodes.Length) {
                stream.Close();
                return false;
            }
            Assert.IsTrue(nodes.Length == data.nodes.Length);   // redundant

            // shorted at Awake
            //System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
            //System.Array.Sort(nodes, comp);
            
            // set data from same index
            for (int i = 0; i < nodes.Length; ++i) {
                nodes[i].Set(data.nodes[i]);
            }

            stream.Close();

            return true;
        }
        else {
            return false;
        }
    }

    public MapNode GetDeadEndNodeOfTeam(TeamType t)
    {
        foreach (MapNode n in nodes)
            if (n.nextNodes.Length == 0 && n.team == t)
                return n;
        return null;
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
    }

    public void UnselectNode()
    {
        target_node = null;
        if(selected_node != null)
            selected_node.Unselect();
        target_node = null;
        menu.ShowArmyPanel(false);
        menu.ShowNodePanel(false);
        //menu.ShowBattlePanel(false);
    }
    public void SelectNode(MapNode node)
    {
        if (is_attacking)
        {
            Assert.IsTrue(false); // this phase never happends!

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
                            //ShowBattlePanel(true);
                            return;
                        }
                    }
                }
            }
        }
        UnselectNode();
        selected_node = node;
        selected_node.Select();

        // CAMPAIGN  END CONDITION
        if (selected_node != null && selected_node.nextNodes.Length == 0 && selected_node.team == TeamType.Player)
        {
            menu.ShowEndPanel(true);
        }
        else
            menu.ShowEndPanel(false);

        // SHOW INFO PANEL
        menu.ShowNodePanel(true);

        // SHOW ACTION PANEL
        if (selected_node.army != null)
        {
            
            if(selected_node.team == TeamType.Player)
            {
                //if (selected_node.army.cards_to_play_count == 0)
                {
                    menu.ShowArmyPanel(true);
                    menu.ShowMoveButton(false);

                    //menu.ShowBattlePanel(false);
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

                        // army look at selected node
                        Vector2 direction = selected_node.transform.position - n.army.transform.position;
                        n.army.transform.up = direction;
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

                    // army look at selected node
                    Vector2 direction = selected_node.transform.position - n.army.transform.position;
                    n.army.transform.up = direction;

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
            Assert.IsTrue(false); // Allow only one army
        }
        else {
            // show confirm panel
            target_node = armyNode;
            //menu.ShowBattlePanel(true);
            menu.ShowArmyPanel(false);

            if (selected_node.army == null)
            {
                MoveArmy();
            }
            else {
                menu.ToggleActive(menu.deckManager.gameObject);
                menu.ShowConfirmPanel(true);
                SetActiveNodeTriggers(false);
            }
        }
    }

    public void BegineBattle(MapNode attacker, MapNode target)
    {
        GameSettings.INSTANCE.SetBattle(attacker, target);
        SaveFile();
        // load battle map
        GameSettings.INSTANCE.SetNextSceneIdx(battleLevelBuildIdx);
        // load loading scene
        SceneManager.LoadScene("Scenes/LoadingScreen");
    }

    public void MoveArmy()
    {
        Assert.IsTrue(selected_node != null);
        Assert.IsTrue(target_node != null);

        // move army
        StartCoroutine(MoveArmyAnimaton(movement_duration));

        //menu.ShowBattlePanel(false);
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

        if (selected_node.army != null && selected_node.army.cards_to_play_count > 0)
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

    public void AddRandomAidPacket(int max_card_count)
    {
        Assert.IsTrue(selected_node != null && selected_node.army != null);
        CardType[] raw_deck = new CardType[max_card_count];
        for (int i = 0; i < max_card_count; ++i) {
            raw_deck[i] = (CardType)GameSettings.INSTANCE.randomizer.Next(0, (int)CardType.CardType_Count);
        }
        CardTypeCount[] deck = menu.deckManager.CollapseDeck(raw_deck.ToList());

        StartCoroutine(RewardAnimation(deck));
    }

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



    public void SetAttackerFromDeckEditor(DeckManager deckManager)
    {
        target_node.army.SetDeck(deckManager.CollapseDeck(deckManager.GetDeckUncollapsed()));
        target_node.army.UpdateText();
    }
}
