using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MeshFilter))]
public class TerrainUpdater : MonoBehaviour
{
    public Battlefield battlefield;
    private MeshFilter mesh_filter;
    bool terrain_updated = true;


    private void Awake()
    {
        mesh_filter = GetComponent<MeshFilter>();
        if (mesh_filter.mesh == null)
            CreateMesh();
        else
            battlefield.ResetGrid();
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
#if false
        if(m_team_grid.Length > 0)
        for (int y = 0; y < grid_size.y; ++y) {
            int row = y * grid_size.x;
            for (int x = 0; x < grid_size.x; ++x) {
                int idx = x + row;
                    TeamType terrain_team = GetTeam(m_team_grid[idx]);
                    Gizmos.color = team_color[(int)terrain_team + 1];
                    Vector3 pos = GetCellPos(new Vector2Int(x, y));
                Gizmos.DrawCube(pos, Vector3.one * cell_size * 0.9f);
            }
        }
#endif

#if true
        battlefield.GizmoGrid();
#endif

#if false
        // draw at coord (TEST SUCCESS)
        Vector2Int[] coords = {
            new Vector2Int(0, 0),
            new Vector2Int(10, 10)
        };
        Gizmos.color = Color.blue;
        for (int i = 0; i < coords.Length; ++i) {
            Vector3 pos = GetCellPos(coords[i]);
            //Gizmos.DrawSphere(pos, cell_size/2);
            Gizmos.DrawCube(pos, new Vector3(cell_size, cell_size, cell_size));
        }

        // draw at touch pos
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = GetCellPosAtTouch();
            Gizmos.color = Color.red;
            //Gizmos.DrawCube(pos, new Vector3(cell_size, cell_size, cell_size));
            //Debug.Log("Drawing sphere at " + pos);

            Vector2Int coord = GetCellCoordAtTouch();
            Debug.Log("Cell Coord at Touch = " + coord);
        }
#endif
    }
#endif

}
