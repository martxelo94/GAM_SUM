using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TileBorder
{
    // 1 north, 2 west, 3 south, 4 east
    public int flag = 0; // which sides are different
    Transform[] sides = null;

    public void Set(int _flag, Vector2Int coord, Battlefield battlefield, GameObject border_prefab, Transform border_parent)
    {
        flag = _flag;

        // clear if no mismatches
        if (flag == 0)
        {
            if (sides != null)
            {
                Assert.IsTrue(sides.Length == 4);
                for (int i = 0; i < 4; ++i)
                {
                    GameObject.Destroy(sides[i].gameObject);
                }
                sides = null;
            }
        }
        else {
            if (sides == null) {
                sides = new Transform[4];
                Vector3 center = battlefield.GetCellPos(coord);
                for (int i = 0; i < 4; ++i) {
                    GameObject border = GameObject.Instantiate(border_prefab, border_parent);
                    border.transform.position = center + battlefield.cell_size * Vector3.up;
                    border.transform.RotateAround(center, Vector3.forward, i * 90f);
                    sides[i] = border.transform;
                    border.SetActive(false);
                }
            }
            // activate from flag
            sides[0].gameObject.SetActive((flag & 1) != 0);
            sides[1].gameObject.SetActive((flag & 2) != 0);
            sides[2].gameObject.SetActive((flag & 4) != 0);
            sides[3].gameObject.SetActive((flag & 8) != 0);
        }

    }
}

[RequireComponent(typeof(MeshFilter))]
public class TerrainUpdater : MonoBehaviour
{
    public Vector2Int grid_size;
    public Vector3 grid_center;

    public Battlefield battlefield;
    private MeshFilter mesh_filter;
    bool terrain_updated = true;

    private Dictionary<Vector2Int, TileBorder> borders;


    private void Awake()
    {
        borders = new Dictionary<Vector2Int, TileBorder>();

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
