using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class Battlefield : ScriptableObject
{
    private Camera _mainCamera = null;
    public Camera mainCamera {
        get {
            if (_mainCamera == null)
                mainCamera = Camera.main;
            return _mainCamera;
        }
        private set { _mainCamera = value; }
    }

    // grid spatial dimensions
    public Vector2Int grid_size;
    public Vector3 grid_center;
    public float cell_size = 1.0f;

    // which team belongs a cell (0: neutral, 1: player, -1: opponent
    public Color[] team_color;
    public int[] team_terrain_influence;
    public float time_to_secure_terrain = 5.0f;

    public float TERRAIN_ALPHA = 0.5f;
    // per cell
    [HideInInspector]
    public int[] team_adjacency_flags;
    [HideInInspector]
    public int[] m_team_grid;   // how much influenced by team
    [HideInInspector]
    public float[] terrain_captured_times;
    [HideInInspector]
    public Color[] terrain_mesh_colors;

    private void OnEnable()
    {
        Assert.IsTrue(team_color.Length == ((int)TeamType.TeamCount + 1));
        Assert.IsTrue(team_terrain_influence.Length == (int)TeamType.TeamCount);

        ResizeGrid(grid_size);
        //mainCamera = Camera.main;
        //Assert.IsTrue(mainCamera != null);
    }

    public Color[] UpdateTerrainRow(int y)
    {
        int row = (y + 1) * (grid_size.x + 2) + 1;
        for (int x = 0; x < grid_size.x; ++x)
        {
            int idx = x + y * grid_size.x;
            if (m_team_grid[idx] == 0)
                continue;
            int meshIdx = x + row;
            terrain_captured_times[idx] += Time.deltaTime * grid_size.y;
            Color c = team_color[(int)GetTeam(m_team_grid[idx]) + 1];
            if (terrain_captured_times[idx] < time_to_secure_terrain)
            {
                c = c / (0.1f + terrain_captured_times[idx]);
            }
            c.a = TERRAIN_ALPHA;
            terrain_mesh_colors[meshIdx] = c;
        }
        return terrain_mesh_colors;
    }

#if false
    public void SetVertexColor(int coordX, int coordY, TeamType team)
    {
        int idx = coordX + 1 + (coordY + 1) * (grid_size.x + 2);
        terrain_mesh_colors[idx] = team_color[(int)team + 1];
        terrain_mesh_filter.mesh.colors = terrain_mesh_colors;
    }
    public void SetVertexColor(Vector2Int coord, TeamType team)
    {
        SetVertexColor(coord.x, coord.y, team);
    }
#endif

    public void ResetGrid()
    {
        int size = grid_size.x * grid_size.y;
        Assert.IsTrue(team_adjacency_flags != null && team_adjacency_flags.Length == size);
        for (int i = 0; i < team_adjacency_flags.Length; ++i)
            team_adjacency_flags[i] = 0;
        Assert.IsTrue(m_team_grid != null && m_team_grid.Length == size);
        for (int i = 0; i < m_team_grid.Length; ++i)
        {
            m_team_grid[i] = 0;
        }
        Assert.IsTrue(terrain_captured_times != null && terrain_captured_times.Length == size);
        for (int i = grid_size.x; i < terrain_captured_times.Length - grid_size.x; ++i)
        {
            terrain_captured_times[i] = 0.0f;
            m_team_grid[i] = 0;
        }
        for (int i = 0; i < grid_size.x; ++i)
        {
            terrain_captured_times[i] = terrain_captured_times[terrain_captured_times.Length - 1 - i] = time_to_secure_terrain;
            m_team_grid[i] = 100;
            m_team_grid[m_team_grid.Length - 1 - i] = -100;
        }
        int vertexCount = grid_size.x * grid_size.y + 2 * (grid_size.y + grid_size.x + 2);
        Assert.IsTrue(terrain_mesh_colors != null && terrain_mesh_colors.Length == vertexCount);
        // update extreme rows
        int bot_row = (grid_size.x + 2) * 2;
        for (int i = 0; i < bot_row; ++i)
        {
            Color c = team_color[(int)TeamType.Player + 1] * 1.2f;
            terrain_mesh_colors[i] = c;
        }
        int top_idx = (grid_size.x + 2) * (grid_size.y);
        for (int i = top_idx; i < terrain_mesh_colors.Length; ++i)
        {
            Color c = team_color[(int)TeamType.Opponent + 1] * 1.2f;
            terrain_mesh_colors[i] = c;
        }
        // mid rows
        for (int i = bot_row; i < top_idx; ++i)
        {
            Color c = team_color[(int)TeamType.None + 1];
            terrain_mesh_colors[i] = c;
        }

    }

    public void CreateGrid()
    {
        ResizeGrid(grid_size);
    }

    public void ResizeGrid(Vector2Int size)
    {
        grid_size = size;
        team_adjacency_flags = new int[size.x * size.y];
        for (int i = 0; i < team_adjacency_flags.Length; ++i)
            team_adjacency_flags[i] = 0;
        m_team_grid = new int[size.x * size.y];
        for (int i = 0; i < m_team_grid.Length; ++i) {
            m_team_grid[i] = 0;
        }
        terrain_captured_times = new float[size.x * size.y];
        for (int i = size.x; i < terrain_captured_times.Length - size.x; ++i)
        {
            terrain_captured_times[i] = 0.0f;
            m_team_grid[i] = 0;
        }
        for (int i = 0; i < size.x; ++i) {
            terrain_captured_times[i] = terrain_captured_times[terrain_captured_times.Length - 1 - i] = time_to_secure_terrain;
            m_team_grid[i] = 100;
            m_team_grid[m_team_grid.Length - 1 - i] = -100;
        }
        terrain_mesh_colors = CreateMeshColors(size);
    }

    public Color[] CreateMeshColors(Vector2Int size)
    {
        int vertexCount = size.x * size.y + 2 * (size.y + size.x + 2);
        Color[] mesh_colors = new Color[vertexCount];
        //for (int i = 0; i < mesh_colors.Length / 2; ++i)
        //     mesh_colors[i] = Color.yellow;
        // update extreme rows
        int bot_idx = (size.x + 2) * 2;
        for (int i = 0; i < bot_idx; ++i)
        {
            Color c = team_color[(int)TeamType.Player + 1] * 1.2f;
            c.a = TERRAIN_ALPHA;
            mesh_colors[i] = c;
        }
        int top_idx = (size.x + 2) * (size.y);
        for (int i = top_idx; i < mesh_colors.Length; ++i)
        {
            Color c = team_color[(int)TeamType.Opponent + 1] * 1.2f;
            c.a = TERRAIN_ALPHA;
            mesh_colors[i] = c;
        }
        for (int i = bot_idx; i < top_idx; ++i)
        {
            Color c = team_color[(int)TeamType.None + 1];
            c.a = 0f;
            mesh_colors[i] = c;
        }
        return mesh_colors;
    }

    public Mesh CreateGridMesh(Vector2Int size)
    {
        // create terrain mesh (plane)
        int vertexCount = size.x * size.y + 2 * (size.y + size.x + 2);
        Vector3[] vertices = new Vector3[vertexCount];
        // vertices
        for (int y = -1; y <= size.y; ++y)
        {
            int rowIdx = (y + 1) * (size.x + 2);
            for (int x = -1; x <= size.x; ++x)
            {
                Vector3 pos = GetCellPos(new Vector2Int(x, y));
                int idx = (x + 1) + rowIdx;
                vertices[idx] = pos;
            }
        }
        const float BORDER_EXTENSION = 1f;
        // extend borders
        for (int i = 0; i < size.x + 2; ++i)
        {
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
        for (int i = 1; i < size.y + 1; ++i)
        {
            // vertical border
            Vector3 v = vertices[i * (size.x + 2)];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[i * (size.x + 2)] = v;
            v = vertices[(i + 1) * (size.x + 2) - 1];
            v -= grid_center;
            v *= BORDER_EXTENSION;
            v += grid_center;
            vertices[(i + 1) * (size.x + 2) - 1] = v;
        }
        // triangles
        int triangleCount = 6 * (size.x + 1) * (size.y + 1);
        List<int> triangles = new List<int>();
        for (int y = 0; y < size.y + 1; ++y)
        {
            int rowIdx = y * (size.x + 2);
            int nextRowIdx = (y + 1) * (size.x + 2);
            for (int x = 0; x < size.x + 1; ++x)
            {
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

        Assert.IsTrue(terrain_mesh_colors != null);
        Assert.IsTrue(terrain_mesh_colors.Length == vertexCount);
        Assert.IsTrue(triangles.Count == triangleCount);

        terrain_mesh.vertices = vertices;
        terrain_mesh.triangles = triangles.ToArray();
        terrain_mesh.colors = terrain_mesh_colors;
        terrain_mesh.RecalculateNormals();


        Debug.Log("Grid Resized");

        return terrain_mesh;

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
            m_team_grid[idx] += team_terrain_influence[(int)team];
            terrain_captured_times[idx] = 0.0f;
            UpdateTeamAdjacencyFlag(idx, team);
        }
        return true;
    }
    public void CaptureCells(Vector3 pos, TeamType team, int radius)
    {
        Vector2Int coord = GetCellCoord(pos);
        // ignore top and bot rows to allow unit spawning
        int minY = Mathf.Max(coord.y - radius, 1);
        int maxY = Mathf.Min(coord.y + radius, grid_size.y - 2);
        int minX = Mathf.Max(coord.x - radius, 0);
        int maxX = Mathf.Min(coord.x + radius, grid_size.x - 1);

        for (int y = minY; y <= maxY; ++y) {
            int row = y * grid_size.x;
            for (int x = minX; x <= maxX; ++x) {
                int idx = x + row;
                TeamType terrain_team = GetTeam(m_team_grid[idx]);
                if (terrain_team != team)
                {
                    m_team_grid[idx] += team_terrain_influence[(int)team];
                    m_team_grid[idx] = Mathf.Min(1, Mathf.Max(m_team_grid[idx], -1));   // magic clamp
                    terrain_captured_times[idx] = 0;
                }
                //else
                //    terrain_captured_times[idx] = Mathf.Min(terrain_captured_times[idx] + 1.0f, time_to_secure_terrain);
            }
        }
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
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
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
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
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
    public bool SnapToCaptured(ref Vector2Int coord, TeamType team)
    {
        if (coord.x < 0 || coord.x >= grid_size.x || coord.y < 0 || coord.y >= grid_size.y)
            return false;
        if (team == TeamType.Player)
        {
            int idx = coord.x + coord.y * grid_size.x;
            for (int i = idx; i >= 0; i -= grid_size.x)
            {
                if (GetTeam(m_team_grid[i]) == team && terrain_captured_times[i] >= time_to_secure_terrain)
                {
                    coord = new Vector2Int(coord.x, i / grid_size.x);
                    return true;
                }
            }
        }
        else
        {
           Assert.IsTrue(team == TeamType.Opponent);
           int idx = coord.x + coord.y * grid_size.x; // NINJA BUG?
            for (int i = idx; i < m_team_grid.Length; i += grid_size.x)
            {
                if (GetTeam(m_team_grid[i]) == team && terrain_captured_times[i] >= time_to_secure_terrain)
                {
                    coord = new Vector2Int(coord.x, i / grid_size.x);
                    return true;
                }
            }
        }
        return false;
    }


    public void GizmoGrid()
    {
        Gizmos.color = Color.white;
        float hh_size = (grid_size.x * cell_size) / 2;
        float vh_size = (grid_size.y * cell_size) / 2;
        // draw horizontals
        Vector3 start = grid_center + new Vector3(-hh_size, vh_size, 0);
        Vector3 end = grid_center + new Vector3(hh_size, vh_size, 0);
        for (int i = 0; i <= grid_size.y; ++i)
        {
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

    }

    // which adjacent tiles mismatch team
    public int GetBorderFlag(Vector2Int coord)
    {
        Assert.IsTrue(IsInsideGrid(coord) == true);
        int flag = 0;
        int idx = coord.x + coord.y * grid_size.x;
        TeamType team = GetTeam(m_team_grid[idx]);
        // check mismatches with adjacents

        // north

        // west

        // south

        // east

        return flag;
    }

    void UpdateTeamAdjacencyFlag(int idx, TeamType team)
    {
        int neighbor_idx = -1;
        int x_coord = idx - (idx / grid_size.x) * grid_size.x;
        // north
        neighbor_idx = idx - grid_size.x;
        if (neighbor_idx >= 0) {

        }
        // west
        if (x_coord - 1 >= 0) {
            neighbor_idx = idx - 1;

        }
        // south
        neighbor_idx = idx + grid_size.x;
        if (neighbor_idx < grid_size.x * grid_size.y) {

        }
        // east
        if(x_coord + 1 < grid_size.x) {
            neighbor_idx = idx + 1;

        }
    }
}
