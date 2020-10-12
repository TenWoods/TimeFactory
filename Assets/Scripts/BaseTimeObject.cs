using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

//所有
[RequireComponent(typeof(Timeline))]
public class BaseTimeObject : MonoBehaviour
{
    public Timeline time
    {
        get
        {
            return GetComponent<Timeline>();
        }
    }
}
