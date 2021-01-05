using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supervisor : HasEye
{
    private float rotate_cd;
    private float rotate_timer;
    private float time_degree = 0.0f;

    private void Update()
    {
        CheckWorkers();
    }
}
