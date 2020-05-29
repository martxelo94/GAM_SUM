using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Unit))]
public class UnitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Unit _target = (Unit)target;

        // update sensor range
        if (_target.sensor != null)
        {
            _target.sensor.SetLayer(_target.team);
            _target.sensor.SetRange(_target.common.sensorRange);
        }
    }
}
