using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainUpdater))]
public class BattlefieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainUpdater _target = (TerrainUpdater)target;

        if (GUILayout.Button("Build Terrain")) {
            _target.EditorCreateMesh();
        }
    }

}
