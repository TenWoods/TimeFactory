using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoInPoint : MonoBehaviour
{
    /*卸货口朝向*/
    [SerializeField]
    [Header("卸货口朝向")]
    private Direction in_direction;
    [SerializeField]
    private List<CargoType> cargoes;

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
    }

    private void Update()
    {
        
    }

    /*接收货物*/
    public void GetCargo(CargoType c)
    {
        cargoes.Add(c);
    }

}
