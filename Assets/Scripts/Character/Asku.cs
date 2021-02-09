using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asku : Character
{
    // Start is called before the first frame update
    void Awake()
    {
        this.moveRange = 3;
        this.type = Type.ENEMY;
        this.speed = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
