﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    // SINGLETON
    private static GameSettings _instance;
    public static GameSettings INSTANCE {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameSettings>();
            }
            return _instance;
        }
        private set { _instance = value; }
    }


    public int nextSceneIdx = -1;

    public CardType[] attack_deck { get; private set; } = null;
    public int attack_idx { get; private set; } = -1;
    public CardType[] target_deck { get; private set; } = null;
    public int target_idx { get; private set; } = -1;
    public bool last_battle_won = false;
    public string campaign_battle_name;

    public void SetAttackDeck(CardType[] deck)
    {
        attack_deck = deck;
    }
    public void SetTargetDeck(CardType[] deck)
    {
        target_deck = deck;
    }
    public void SetBattle(MapNode attaker, MapNode target)
    {
        Assert.IsTrue(attaker.army != null && target.army != null);
        attack_deck = attaker.army.deck_types;
        attack_idx = attaker.node_idx;

        target_deck = target.army.deck_types;
        target_idx = target.node_idx;

        campaign_battle_name = SceneManager.GetActiveScene().name;
    }

    public void ResetBattle()
    {
        attack_deck = target_deck = null;
        attack_idx = target_idx = -1;
        campaign_battle_name = "";
    }

    public bool IsBattle()
    {
        return attack_idx > -1;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


