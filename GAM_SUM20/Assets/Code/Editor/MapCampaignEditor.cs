﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapCampaign))]
public class MapCampaignEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapCampaign _target = (MapCampaign)target;
        if (GUILayout.Button("Delete"))
        {
            MapCampaign.DeleteFile();
        }
        if (GUILayout.Button("Save"))
        {
            _target.SaveFile();
        }
        if (GUILayout.Button("Update Nodes"))
        {
            _target.FindNodes();
            _target.UpdateNodes();
            _target.FindArmies();
        }
        //if (GUILayout.Button("Find Nodes"))
        //{
        //    _target.FindNodes();
        //}
        //if (GUILayout.Button("Find Armies"))
        //{
        //    _target.FindArmies();
        //}
    }
}
