﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class HealthBar : MonoBehaviour
{
    public Transform bar;
    float zOffset = 2f;
    // set by squad
    [HideInInspector]
    public Unit stats;

    // Start is called before the first frame update
    void Start()
    {
        //stats = gameObject.GetComponentInParent<UnitStats>();
        Assert.IsTrue(stats != null);
        Assert.IsTrue(stats.common.maxHitPoints > 0.0f);
        //transform.localScale = new Vector3(stats.maxHitPoints * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = stats.transform.position + Vector3.back * zOffset;
        transform.eulerAngles = Vector3.zero;
        float factor = (float)stats.currentHitPoints / stats.common.maxHitPoints;
        //Debug.Log("Factor health " + factor.ToString());
        bar.transform.localPosition = new Vector3((-0.5f + factor / 2) * 4, 0, -1);
        bar.transform.localScale = new Vector3(factor, 1, 1);
    }

    public void SetTeamColor(Color color)
    {
        //set color
        SpriteRenderer sprite = bar.GetComponent<SpriteRenderer>();
        Assert.IsTrue(sprite != null);
        sprite.color = color;
    }
    public void SetUnit(Unit unit)
    {
        stats = unit;
    }
}
