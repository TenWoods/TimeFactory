using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadPort : MonoBehaviour
{
    /*货物数据路径*/
    [SerializeField]
    private string path;
    /*出货口初始朝向*/
    [SerializeField]
    private Direction direction;
    /*出货口的存货*/
    private Stack<CargoType> cargoes;
    /*当前货物*/
    private GameObject current_cargo;
    /*相对位置*/
    [SerializeField]
    private float offset = 1.0f;
    /*节拍积攒速度*/
    [SerializeField] //Debug
    private float speed = 1.0f;
    private int last_beat;
    private float beat_count;
    private float rotate_cd;
    private float rotate_timer;
    private float degree;
    public Transform pointer;
    private Worker worker;
    [SerializeField]
    private GameManager gameManager;

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
        LoadData();
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
        worker = null;
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
        //PassCargo();
    }

    /*读取货物数据*/
    private void LoadData()
    {
        cargoes = new Stack<CargoType>();
        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + path);
            string line = sr.ReadLine();
            string[] data = line.Split(',');
            int cargoType;
            for (int i = data.Length-1; i >= 0; i--)
            {
                cargoType = int.Parse(data[i]);
                cargoes.Push((CargoType)cargoType);
            }
        }
        catch
        {
            Debug.Log("Error");
        }
    }

    private void PassCargo()
    {
        if (worker == null)
            return;
        CargoType cargo = cargoes.Pop();
        if (!worker.GetCargo(direction, cargo))
        {
            //没递交的货物压回
            cargoes.Push(cargo);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        //当接触到搬运工后递交货物
        if (other.tag == "Worker")
        {
            worker = other.GetComponent<Worker>();
            PassCargo();
        }    
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.tag == "Worker")
        {
            if (!gameManager.IsMove)
            {
                return;
            }
            PassCargo();
        }   
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Worker")
        {
            worker = null;
        }
    }
}
