using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent(typeof(MeshFilter))]
public class TerrainUpdater : MonoBehaviour
{
    public Vector2Int grid_size;
    public Vector3 grid_center;

    public Battlefield battlefield;
    private MeshFilter mesh_filter;
    bool terrain_updated = true;

    private Dictionary<Vector2Int, GameObject> borders;


    private void Awake()
    {
        battlefield.grid_size = grid_size;
        battlefield.grid_center = grid_center;

        mesh_filter = GetComponent<MeshFilter>();
        if (mesh_filter.mesh != null)
            mesh_filter.mesh = null;
        battlefield.CreateGrid();
        CreateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (terrain_updated)
            StartCoroutine(UpdateCellColors());

    }

    public void CreateMesh()
    {
        Assert.IsTrue(mesh_filter != null);
        mesh_filter.mesh = battlefield.CreateGridMesh(battlefield.grid_size);
    }
    public void EditorCreateMesh()
    {
        battlefield.grid_size = grid_size;
        battlefield.grid_center = grid_center;

        if (mesh_filter == null)
            mesh_filter = GetComponent<MeshFilter>();
        battlefield.CreateGrid();
        mesh_filter.mesh = battlefield.CreateGridMesh(battlefield.grid_size);
    }
    IEnumerator UpdateCellColors()
    {
        terrain_updated = false; // coroutine control
        Vector2Int grid_size = battlefield.grid_size;

        for (int y = 1; y < grid_size.y - 1; ++y)
        {
            mesh_filter.mesh.colors = battlefield.UpdateTerrainRow(y);
            yield return null;  // update a row per frame
        }

        terrain_updated = true; // coroutine control
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

#if true
        battlefield.GizmoGrid();
#endif

    }
#endif
}
