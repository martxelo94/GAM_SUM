using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public bool is_enemy = false;

    Battlefield battlefield;
    Movement[] troops;

    // Start is called before the first frame update
    void Start()
    {
        battlefield = FindObjectOfType<Battlefield>();

        troops = GetComponentsInChildren<Movement>();

        Material mat = Resources.Load<Material>(is_enemy ? "Materials/M_Opponent" : "Materials/M_Player");
        var renderables = GetComponentsInChildren<Renderer>();
        foreach (var r in renderables)
            r.material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        int troops_destroyed = 0;
        // capture land
        foreach (Movement m in troops) {
            if (m == null) {
                troops_destroyed++;
                continue;
            }
            Vector2Int coord = battlefield.GetCellCoord(m.transform.position);
            int idx = coord.x + coord.y * battlefield.grid_size.x;
            if (idx >= battlefield.m_team_grid.Length ||idx < 0)
                continue;
            int terrain_team = battlefield.m_team_grid[idx];
            int new_terrain_team = terrain_team;
            if (terrain_team >= 0 && is_enemy)
                new_terrain_team = -1;
            else if (terrain_team <= 0 && !is_enemy)
                new_terrain_team = 1;
            // update if different
            if ((new_terrain_team < 0 && terrain_team >= 0) || (new_terrain_team > 0 && terrain_team <= 0))
            {
                battlefield.SetVertexColor(coord, new_terrain_team);
                battlefield.m_team_grid[idx] = new_terrain_team;
            }

        }
        if (troops_destroyed == troops.Length) {
            Destroy(gameObject);
        }

    }
}
