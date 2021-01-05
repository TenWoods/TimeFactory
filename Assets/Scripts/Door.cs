using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : BaseTimeObject
{
    /*门条件设置*/
    [SerializeField]
    [Header("设置条件")]
    private int[] conditions;
    /*full型指针*/
    public GameObject[] pointerFs;
    /*empty型指针*/
    public GameObject[] pointerEs;
    private void Start()
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i] == 1)
            {
                pointerFs[i].SetActive(true);
            }
            else if (conditions[i] == 0)
            {
                pointerEs[i].SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Worker")
        {
            //Debug.Log(0);
            Worker w = other.GetComponent<Worker>();
            bool[] results = new bool[4]{true, true, true, true};
            //通过门的条件判断
            for (int i = 0; i < 4; i++)
            {
                if (conditions[i] == 0)
                {
                    if (w.Cargoes[i] != CargoType.EMPTY && w.Cargoes[i] != CargoType.Block)
                    {
                        results[i] = false;
                    }
                }
                else if (conditions[i] == 1)
                {
                    if (w.Cargoes[i] == CargoType.EMPTY || w.Cargoes[i] == CargoType.Block)
                    {
                        results[i] = false;
                    }
                }
            }
            if (!results[0] || !results[1] || !results[2] || !results[3])
            {
                w.Move_direction = !w.Move_direction;
            }
        }
    }
}
