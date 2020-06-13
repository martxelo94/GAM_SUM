using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardData
{
    public float spawnTime;
    public Vector2Int cost;
    public GameObject spawnedPrefab;
    public GameObject blueprintPrefab;
    public Texture cardTexture;
}

