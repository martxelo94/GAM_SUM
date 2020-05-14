using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   // Text
using UnityEngine.Assertions;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour
{
    [HideInInspector]
    public Image image;

    [SerializeField]
    private CardType m_type;
    public CardType type
    {
        get { return m_type; }
        set { m_type = value; SetCardName(value); SetImage(value); }
    }

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SetCardName(CardType _type)
    {
        Text text = gameObject.GetComponentInChildren<Text>();
        text.text = _type.ToString();
    }

    public void SetImage(CardType _type)
    {
        // TODO: change image
    }
    
}
