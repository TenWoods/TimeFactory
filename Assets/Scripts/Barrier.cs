using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    /*装置对应的搬运工*/
    [SerializeField]
    [Header("对应搬运工")]
    private Worker m_worker;
    /*阻挡物*/
    [SerializeField]
    [Header("阻挡物")]
    private GameObject[] barrieres;

    private void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (m_worker.Cargoes[i] != CargoType.EMPTY)
            {
                barrieres[i].SetActive(true);
            }
            else
            {
                barrieres[i].SetActive(false);
            }
        }
    }
}
