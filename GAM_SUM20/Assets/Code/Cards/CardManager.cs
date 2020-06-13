using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class CardManager : ScriptableObject
{
    public Texture cardReverseTexture;
    public CardData[] cards;

    private void OnEnable()
    {
        Assert.IsTrue(cards.Length == (int)CardType.CardType_Count);
    }

    public GameObject PlayType(CardType type)
    {
        GameObject squadObj = Instantiate(cards[(int)type].spawnedPrefab) as GameObject;

        return squadObj;
    }

    public GameObject BlueprintType(CardType type)
    {
        GameObject blueprintObj = Instantiate(cards[(int)type].blueprintPrefab);
        return blueprintObj;
    }
}
