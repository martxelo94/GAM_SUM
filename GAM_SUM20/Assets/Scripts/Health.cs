using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int hit_points = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Damage(int damage)
    {
        hit_points -= damage;
        if (hit_points <= 0)
            return true;
        return false;
    }
}
