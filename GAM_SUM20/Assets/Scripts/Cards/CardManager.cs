﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CardManager : MonoBehaviour
{

    public Vector2Int[] costs;
    public GameObject[] blueprints; // blueprinted card before being played
    public GameObject[] card_prefabs;   // cards to play all logic included

    private void Awake()
    {
        Assert.IsTrue(card_prefabs.Length == blueprints.Length);
        Assert.IsTrue(blueprints.Length == costs.Length);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}