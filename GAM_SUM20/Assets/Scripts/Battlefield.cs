﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Battlefield : MonoBehaviour
{
    // grid spatial dimensions
    public Vector2Int grid_size;
    public Vector3 grid_center;
    public float cell_size = 1.0f;

    // which team belongs a cell (0: neutral, 1: player, -1: opponent
    [HideInInspector]
    public int[] m_team_grid;   // how much influenced by team
    public MeshFilter terrain_mesh_filter;
    Color[] terrain_mesh_colors;
    public Color[] team_color;
    public int[] team_terrain_influence;
    bool terrain_updated = true;


    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(team_color.Length == (int)TeamType.TeamCount);
        Assert.IsTrue(team_terrain_influence.Length == (int)TeamType.TeamCount);
        //grid_size = grid_size;
        ResizeGrid(grid_size);
    }

    // Update is called once per frame
    void Update()
    {
        if (terrain_updated)
            StartCoroutine(UpdateCellColors());
    }
    public void SetVertexColor(int coordX, int coordY, TeamType team)
    {
        int idx = coordX + 1 + (coordY + 1) * (grid_size.x + 2);
        terrain_mesh_colors[idx] = team_color[(int)team];
        terrain_mesh_filter.mesh.colors = terrain_mesh_colors;
    }
    public void SetVertexColor(Vector2Int coord, TeamType team)
    {
        SetVertexColor(coord.x, coord.y, team);
    }

    public void ResizeGrid(Vector2Int size)
    {
        m_team_grid = new int[size.x * size.y];
        for (int i = 0; i < m_team_grid.Length; ++i) {
            m_team_grid[i] = 0;
        }

        // create terrain mesh (plane)
        int vertexCount = grid_size.x * grid_size.y + 2 * (grid_size.y + grid_size.x + 2);
        Vector3[] vertices = new Vector3[vertexCount];
        // vertices
        for (int y = -1; y <= grid_size.y; ++y) {
            int rowIdx = (y + 1) * (grid_size.x+2);
            for (int x = -1; x <= grid_size.x; ++x) {
                Vector3 pos = GetCellPos(new Vector2Int(x, y));
                int idx = (x + 1) + rowIdx;
                vertices[idx] = pos;
            }
        }
        const float BORDER_EXTENSION = 2.0f;
        // extend borders
        for (int i = 0; i < grid_size.x + 2; ++i) {
            // horizontal border
            Vector3 v = vertices[i];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[i] = v;
            v = vertices[vertices.Length - i - 1];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[vertices.Length - i - 1] = v;
        }
        for (int i = 1; i < grid_size.y + 1; ++i)
        {
            // vertical border
            Vector3 v = vertices[i * (grid_size.x + 2)];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[i * (grid_size.x + 2)] = v;
            v = vertices[(i + 1) * (grid_size.x + 2) - 1];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[(i + 1) * (grid_size.x + 2) - 1] = v;
        }
        // triangles
        int triangleCount = 6 * (grid_size.x + 1) * (grid_size.y + 1);
        List<int> triangles = new List<int>();
        for (int y = 0; y < grid_size.y + 1; ++y) {
            int rowIdx = y * (grid_size.x + 2);
            int nextRowIdx = (y + 1) * (grid_size.x + 2);
            for (int x = 0; x < grid_size.x + 1; ++x) {
                triangles.Add(x + rowIdx);
                triangles.Add(x + nextRowIdx + 1);
                triangles.Add(x + rowIdx + 1);

                triangles.Add(x + rowIdx);
                triangles.Add(x + nextRowIdx);
                triangles.Add(x + nextRowIdx + 1);
            }
        }
        
        Mesh terrain_mesh = new Mesh();
        terrain_mesh.name = "TerrainGrid";

       terrain_mesh_colors = new Color[vertexCount];
       //for (int i = 0; i < terrain_mesh_colors.Length / 2; ++i)
       //     terrain_mesh_colors[i] = Color.yellow;

        Assert.IsTrue(triangles.Count == triangleCount);
        terrain_mesh.vertices = vertices;
        terrain_mesh.triangles = triangles.ToArray();
        terrain_mesh.colors = terrain_mesh_colors;
        terrain_mesh.RecalculateNormals();

        terrain_mesh_filter.mesh = terrain_mesh;

        Debug.Log("Grid Resized");
    }
    public bool CaptureCell(Vector3 pos, TeamType team)
    {
        Vector2Int coord = GetCellCoord(pos);
        int idx = coord.x + coord.y * grid_size.x;
        // outsude grid
        if (idx >= m_team_grid.Length || idx < 0)
            return false;
        TeamType terrain_team = GetTeam(m_team_grid[idx]);
        if (terrain_team != team)
        {
            //SetVertexColor(coord, team);
            m_team_grid[idx] += team_terrain_influence[(int)team];
        }
        return true;
    }
    public void CaptureCells(Vector3 pos, TeamType team, int radius)
    {
        Vector2Int coord = GetCellCoord(pos);
        int minY = Mathf.Max(coord.y - radius, 0);
        int maxY = Mathf.Min(coord.y + radius, grid_size.y - 1);
        int minX = Mathf.Max(coord.x - radius, 0);
        int maxX = Mathf.Min(coord.x + radius, grid_size.x - 1);

        for (int y = minY; y <= maxY; ++y) {
            int row = y * grid_size.x;
            for (int x = minX; x <= maxX; ++x) {
                int idx = x + row;
                TeamType terrain_team = GetTeam(m_team_grid[idx]);
                if (terrain_team != team)
                {
                    //SetVertexColor(x, y, team);
                    m_team_grid[idx] += team_terrain_influence[(int)team];
                }
            }
        }
    }

    IEnumerator UpdateCellColors()
    {
        terrain_updated = false;
        for (int y = 0; y < grid_size.y; ++y) {
            int row = (y + 1) * (grid_size.x + 2) + 1;
            for (int x = 0; x < grid_size.x; ++x) {
                int idx = x + row;
                terrain_mesh_colors[idx] = team_color[(int)GetTeam(m_team_grid[x + y * grid_size.x])];
                terrain_mesh_filter.mesh.colors = terrain_mesh_colors;
            }
            yield return null;  // update a row per frame
        }
        terrain_updated = true;
    }

    public Vector3 GetCellPos(Vector2Int coord)
    {
        float x = grid_center.x + (coord.x - grid_size.x / 2.0f + 0.5f) * cell_size;
        float y = grid_center.y + (coord.y - grid_size.y / 2.0f + 0.5f) * cell_size;
        return new Vector3(x, y, grid_center.z);
    }
    public Vector3 GetCellPosAtTouch()
    {
#if UNITY_EDITOR
        Vector3 screenPos = Input.mousePosition;
#else
        Vector3 screenPos = Input.touches[0].position;
#endif
        screenPos.z = grid_center.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos -= grid_center;
        // center at cell

        worldPos.x = Mathf.Floor(worldPos.x / cell_size) + 0.5f;
        worldPos.y = Mathf.Floor(worldPos.y / cell_size) + 0.5f;
        worldPos.x *= cell_size;
        worldPos.y *= cell_size;

        worldPos += grid_center;

        return worldPos;
    }
    public Vector2Int GetCellCoordAtTouch()
    {
#if UNITY_EDITOR
        Vector3 screenPos = Input.mousePosition;
#else
        Vector3 screenPos = Input.touches[0].position;
#endif
        screenPos.z = grid_center.z;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return GetCellCoord(worldPos);
    }
    public Vector2Int GetCellCoord(Vector3 worldPos)
    {
        worldPos -= grid_center;
        // center at cell

        worldPos.x = Mathf.Floor(worldPos.x / cell_size) + 0.5f;
        worldPos.y = Mathf.Floor(worldPos.y / cell_size) + 0.5f;

        worldPos.x += grid_size.x / 2.0f;
        worldPos.y += grid_size.y / 2.0f;

        return new Vector2Int((int)worldPos.x, (int)worldPos.y);

    }
    public bool IsInsideGrid(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < grid_size.x && coord.y >= 0 && coord.y < grid_size.y;
    }
    public TeamType GetTeam(int val)
    {
        if (val > 0)
            return TeamType.Player;
        else if (val < 0)
            return TeamType.Opponent;
        return TeamType.None;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(m_team_grid.Length > 0)
        for (int y = 0; y < grid_size.y; ++y) {
            int row = y * grid_size.x;
            for (int x = 0; x < grid_size.x; ++x) {
                int idx = x + row;
                    TeamType terrain_team = GetTeam(m_team_grid[idx]);
                    Gizmos.color = team_color[(int)terrain_team];
                    Vector3 pos = GetCellPos(new Vector2Int(x, y));
                Gizmos.DrawCube(pos, Vector3.one * cell_size * 0.9f);
            }
        }
#if false
        Gizmos.color = Color.white;
        float hh_size = (grid_size.x * cell_size) / 2;
        float vh_size = (grid_size.y * cell_size) / 2;
        // draw horizontals
        Vector3 start = grid_center + new Vector3(-hh_size, vh_size, 0);
        Vector3 end =   grid_center + new Vector3(hh_size, vh_size, 0);
        for (int i = 0; i <= grid_size.y; ++i) {
            Gizmos.DrawLine(start, end);
            // update start and end
            start.y -= cell_size;
            end.y -= cell_size;
        }
        // draw verticals
        start = grid_center + new Vector3(hh_size, vh_size, 0);
        end = grid_center + new Vector3(hh_size, -vh_size, 0);
        for (int i = 0; i <= grid_size.x; ++i)
        {
            Gizmos.DrawLine(start, end);
            // update start and end
            start.x -= cell_size;
            end.x -= cell_size;
        }
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
