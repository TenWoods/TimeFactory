using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnloadPort : MonoBehaviour
{
    /*卸货口朝向*/
    [SerializeField]
    [Header("卸货口朝向")]
    private Direction direction;
    [SerializeField]
    private List<CargoType> cargoes;
    /*节拍积攒速度*/
    [SerializeField] //Debug
    private float speed = 1.0f;
    private int last_beat;
    private float beat_count;
    private float rotate_cd;
    private float rotate_timer;
    private float degree;
    [SerializeField]
    private GameManager gameManager;
    public Transform pointer;

    public Direction In_direction
    {
        get
        {
            return direction;
        }
    }

    public float Speed 
    {
        get
        {
            return speed;
        }
        set 
        {
            speed = value;
        }
    }

    private void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        cargoes = new List<CargoType>();
        switch (direction)
        {
            case Direction.UP : 
            pointer.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            degree = 0.0f;
            break;
            case Direction.RIGHT :
            pointer.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f);
            degree = -90.0f;
            break;
            case Direction.DOWN :
            pointer.rotation = Quaternion.Euler(0.0f, 0.0f, -180.0f);
            degree = -180.0f;
            break;
            case Direction.LEFT :
            pointer.rotation = Quaternion.Euler(0.0f, 0.0f, -270.0f);
            degree = -270.0f;
            break;
        }
        rotate_cd = gameManager.GCD / 2;
        rotate_timer = rotate_cd;
        last_beat = 0;
        beat_count = 0.0f;
    }

    private void Update()
    {
        if (!gameManager.IsMove)
        {
            rotate_timer = rotate_cd;
            return;
        }
        //计算beat数
        if (last_beat != gameManager.Beat_times)
        {
            beat_count += (gameManager.Beat_times - last_beat) * speed;
            //Debug.Log(beat_count);
            last_beat = gameManager.Beat_times;
        }
        //大于一个beat才能移动
        if (beat_count < 1.0f)
        {
            return;
        }
        if (rotate_timer < rotate_cd)
        {
            rotate_timer += Time.deltaTime;
            return;
        }
        beat_count -= 1.0f;
        rotate_timer = 0.0f;
        degree -= 90.0f;
        if (degree <= -360.0f)
        {
            degree = 0.0f;
        }
        pointer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, degree);
        if ((direction - 1) < 0)
        {
            direction = (Direction)3;
        }
        else
        {
            direction -= 1;
        }
    }

    /*接收货物*/
    public void GetCargo(CargoType c)
    {
        cargoes.Add(c);
    }
}
