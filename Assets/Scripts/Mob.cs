using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob : MonoBehaviour
{
    public int m_health;
    public int health
    {
        get
        {
            return m_health;
        }
        set
        {
            if (value >= 0)
            {
                m_health = value;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
