﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Text
using UnityEngine.Assertions;

[RequireComponent(typeof(RawImage))]
public class CardImage : MonoBehaviour
{
    public RawImage image;

    [SerializeField]
    private CardType m_type;
    public CardType type
    {
        get { return m_type; }
        set {
            m_type = value;
            //SetCardName();
            SetImage();
        }
    }

    public CardManager cardManager;

    private void Awake()
    {
        Assert.IsTrue(cardManager != null);
        Assert.IsTrue(image != null);
    }

    // Start is called before the first frame update
    void Start()
    {
        // update image
        type = m_type;
    }

    void SetCardName()
    {
        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = m_type.ToString();
    }

    void SetImage()
    {
        // TODO: change image
        if (m_type == CardType.None)
            image.texture = cardManager.cardReverseTexture;
        else
            image.texture = cardManager.cards[(int)m_type].cardTexture;
    }
    
}
