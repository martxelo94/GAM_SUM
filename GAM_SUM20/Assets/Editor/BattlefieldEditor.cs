using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Battlefield))]
public class BattlefieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Battlefield _target = (Battlefield)target;

        if (GUILayout.Button("Build Terrain")) {
            _target.ResizeGrid(_target.grid_size);
        }
    }

}
