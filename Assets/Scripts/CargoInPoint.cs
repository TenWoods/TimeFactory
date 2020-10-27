using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoInPoint : BaseTimeObject
{
    /*卸货口朝向*/
    [SerializeField]
    [Header("卸货口朝向")]
    private Direction in_direction;
    [SerializeField]
    private List<CargoType> cargoes;
    [SerializeField]
    [Header("卸货冷却时间")]
    private float in_cd;
    private float cd_timer;
    private bool is_incd;

    public Direction In_direction
    {
        get
        {
            return in_direction;
        }
    }

    private void Start()
    {
        cargoes = new List<CargoType>();
        cd_timer = 0.0f;
    }

    private void Update()
    {
        if (is_incd)
        {
            cd_timer += time.deltaTime;
            if (cd_timer > in_cd)
            {
                cd_timer = 0.0f;
                is_incd = false;
            }
        }
    }

    /*接收货物*/
    public void GetCargo(CargoType c)
    {
        cargoes.Add(c);
        is_incd = true;
    }

    public bool IsInCD()
    {
        return is_incd;
    }

}
