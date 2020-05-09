using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    [SerializeField]
    private Vector2Int m_grid_size;
    public Vector2Int grid_size
    {
        get { return m_grid_size; }
        set { m_grid_size = value; ResizeGrid(value); }
    }
    // which team belongs a cell (0: neutral, 1: player, -1: opponent
    [HideInInspector]
    public int[] m_team_grid;

    // grid spatial dimensions
    public Vector3 grid_center;
    public float cell_size = 1.0f;

    public Color player_color;
    public Color opposite_color;

    // Start is called before the first frame update
    void Start()
    {
        grid_size = m_grid_size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResizeGrid(Vector2Int size)
    {
        m_team_grid = new int[size.x * size.y];
        Debug.Log("Grid Resized");
    }

    public Vector3 GetCellPos(Vector2Int coord)
    {
        float x = grid_center.x + (coord.x - m_grid_size.x / 2.0f + 0.5f) * cell_size;
        float y = grid_center.y + (coord.y - m_grid_size.y / 2.0f + 0.5f) * cell_size;
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

        worldPos.x += m_grid_size.x / 2.0f;
        worldPos.y += m_grid_size.y / 2.0f;

        return new Vector2Int((int)worldPos.x, (int)worldPos.y);

    }
    public bool IsInsideGrid(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < m_grid_size.x && coord.y >= 0 && coord.y < m_grid_size.y;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(m_team_grid.Length > 0)
        for (int y = 0; y < m_grid_size.y; ++y) {
            int row = y * m_grid_size.x;
            for (int x = 0; x < m_grid_size.x; ++x) {
                int idx = x + row;
                    int terrain_team = m_team_grid[idx];
                    if(terrain_team < 0)
                        Gizmos.color = opposite_color;
                    else if(terrain_team > 0)
                        Gizmos.color = player_color;
                    else
                        Gizmos.color = Color.white;

                    Vector3 pos = GetCellPos(new Vector2Int(x, y));
                Gizmos.DrawCube(pos, Vector3.one * cell_size * 0.9f);
            }
        }
#if false
        Gizmos.color = Color.white;
        float hh_size = (m_grid_size.x * cell_size) / 2;
        float vh_size = (m_grid_size.y * cell_size) / 2;
        // draw horizontals
        Vector3 start = grid_center + new Vector3(-hh_size, vh_size, 0);
        Vector3 end =   grid_center + new Vector3(hh_size, vh_size, 0);
        for (int i = 0; i <= m_grid_size.y; ++i) {
            Gizmos.DrawLine(start, end);
            // update start and end
            start.y -= cell_size;
            end.y -= cell_size;
        }
        // draw verticals
        start = grid_center + new Vector3(hh_size, vh_size, 0);
        end = grid_center + new Vector3(hh_size, -vh_size, 0);
        for (int i = 0; i <= m_grid_size.x; ++i)
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
