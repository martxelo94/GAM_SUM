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
    public Button deckConfirmButton;


    private void Awake()
    {
        if(menu == null)
            menu = GetComponent<CampaignMenu>();

        Assert.IsTrue(menu != null);
        Assert.IsTrue(deckConfirmButton != null);

        FindNodes();

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

        // check that deck can be confirmed
        if (menu.deckManager.gameObject.activeSelf == true)
        {
            if (menu.deckManager.card_deck_count > 0)
                deckConfirmButton.interactable = true;
            else
                deckConfirmButton.interactable = false;
        }
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
        MapNode attackNode = nodes[GameSettings.INSTANCE.attack_idx];
        Deck attack_army = attackNode.army;
        Assert.IsTrue(attack_army != null);
        attack_army.SetDeck(GameSettings.INSTANCE.CopyAttackDeck());

        // enemy army is not changed
#if false
        Assert.IsTrue(GameSettings.INSTANCE.target_idx > -1);
        Deck target_army = nodes[GameSettings.INSTANCE.target_idx].army;
        target_army.SetDeck(GameSettings.INSTANCE.CopyTargetDeck());
#endif

        MapNode targetNode = nodes[GameSettings.INSTANCE.target_idx];
        Assert.IsTrue(targetNode.army != null);
        targetNode.army.SetDeck(GameSettings.INSTANCE.CopyTargetDeck());

        if (GameSettings.INSTANCE.last_battle_won)
        {
            // capture node & get reward
            CaptureNode(attackNode, targetNode);
        }
        // update editor
        menu.deckManager.SetUpDeck(attack_army);
        menu.deckManager.SaveDeck();

    }


    IEnumerator RewardAnimation(CardTypeCount[] reward)
    {
        RewardFlipCard rewardCardPrefab = menu.deckManager.rewardCardPrefab;
        Transform rewardPanel = menu.deckManager.rewardPanel;

        menu.addCardsButton.interactable = false;
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

            rewardFlipCards.Add(card);
        }

        int frames = (int)(0.5f / Time.deltaTime);
        for(int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(Vector3.zero, initScale, (float)(i + 1) / frames);
            yield return null;
        }

        // wait for every card to be flipped
        while(!menu.deckManager.IsEveryCardFliped(rewardFlipCards))
        {
            yield return new WaitForSecondsRealtime(1f);
        }
        // add reward & remove
        foreach (RewardFlipCard c in rewardFlipCards)
        {
            menu.deckManager.AddToPool(c.reward);
            Destroy(c.gameObject);
        }

        // reverse scale
        for (int i = 0; i < frames; ++i)
        {
            rewardPanel.parent.localScale = Vector3.Lerp(initScale, Vector3.zero, (float)(i + 1) / frames);
            yield return null;
        }
        rewardPanel.parent.localScale = initScale;
        rewardPanel.parent.gameObject.SetActive(false);

        SetActiveNodeTriggers(true);
        selected_node.reward_received = true;
        menu.infoPanel.reward_received_panel.SetActive(selected_node.reward_received);
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

    public MapNode GetDeadEndNode()
    {
        for (int i = nodes.Length - 1; i >= 0; --i)
            if (nodes[i].nextNodes.Length == 0)
                return nodes[i];
        return null;
    }

    public MapNode GetDeadEndNodeOfTeam(TeamType t)
    {
        for (int i = nodes.Length - 1; i >= 0; --i)
            if (nodes[i].nextNodes.Length == 0 && nodes[i].team == t)
                return nodes[i];
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

        // update infoPanel if is open
        if (menu.infoPanel.gameObject.activeSelf == true)
            menu.infoPanel.SetUp(selected_node);

        // SHOW ACTION PANEL
        if (selected_node.army != null)
        {
            
            if(selected_node.team == TeamType.Player)
            {
                //if (selected_node.army.cards_to_play_count == 0)
                {
                    menu.ShowArmyPanel(true);
                    menu.ShowMoveButton(false);
                    if (selected_node.reward_received)
                        menu.DisableButton(menu.addCardsButton);
                    else
                        menu.EnableButton(menu.addCardsButton);
                    //menu.ShowBattlePanel(false);
                    menu.ShowNodePanel(false);
                }
                
            }
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

    public void ReceiveNodeReward()
    {
        CardTypeCount[] deck = selected_node.deck_reward;

        menu.addCardsButton.interactable = false;
        SetActiveNodeTriggers(false);

        StartCoroutine(RewardAnimation(deck));
    }
    /*
    public void AddRandomAidPacket(int max_card_count)
    {
        Assert.IsTrue(selected_node != null && selected_node.army != null);
        CardType[] raw_deck = new CardType[max_card_count];
        for (int i = 0; i < max_card_count; ++i) {
            raw_deck[i] = (CardType)GameSettings.INSTANCE.randomizer.Next(0, (int)CardType.CardType_Count);
        }
        CardTypeCount[] deck = menu.deckManager.CollapseDeck(raw_deck.ToList());

        StartCoroutine(menu.deckManager.RewardAnimation(deck));
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
    */



    public void SetAttackerFromDeckEditor(DeckManager deckManager)
    {
        target_node.army.SetDeck(deckManager.CollapseDeck(deckManager.GetDeckUncollapsed()));
        target_node.army.UpdateText();
    }

    public void ToggleInfoPanel()
    {

        menu.infoPanel.SetUp(selected_node);

        menu.infoPanel.gameObject.SetActive(!menu.infoPanel.gameObject.activeSelf);
    }

    public void FindNodes()
    {
        nodes = FindObjectsOfType<MapNode>();
        System.Comparison<MapNode> comp = (a, b) => a.name.CompareTo(b.name);
        System.Array.Sort(nodes, comp);
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].map = this;
            nodes[i].node_idx = i;
        }
    }

    public void UpdateNodes()
    {
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].UpdateLinks();
            nodes[i].SetTeamColor();
        }
    }
}
