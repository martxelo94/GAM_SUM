using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapNode))]
[CanEditMultipleObjects]
public class MapNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapNode _target = (MapNode)target;

        if (GUILayout.Button("Create Links"))
        {
            _target.CreateLinks();
        }

        if (GUILayout.Button("Create Army"))
        {
            _target.SpawnArmy();
        }
        if (GUILayout.Button("Remove Army"))
        {
            _target.DestroyArmy();
        }
        // update link positions
        //Debug.Log(_target.name + " is editing.");
        //_target.UpdateLinkPositions();
        //for(int i = 0; i < _target.parentNodes.Count; ++i)
        //{
        //    MapNode parentNode = _target.parentNodes[i];
        //    parentNode.UpdateLinkPositions();
        //}
    }
}
