using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*方向*/
public enum Direction
{
    UP = 0,
    LEFT = 1,
    DOWN = 2,
    RIGHT = 3
    
}

/*货物类型*/
public enum CargoType
{
    NORMAL = 0,
    Alice = 1,
    Block = 2,
    EMPTY
}

/*分段路线*/
public struct Route
{
    public Direction direction;
    public int length;

    public Route(Direction dir, int l)
    {
        direction = dir;
        length = l;
    }
}

public class Worker : MonoBehaviour
{
    /*搬运工序号*/
    [Header("搬运工序号")]
    [SerializeField]
    private int id;
    /*节拍积攒速度*/
    [SerializeField] //Debug
    private float speed = 1.0f;
    [Header("单位移动距离")]
    [SerializeField]
    /*每一次移动的距离*/
    private float move_length = 1.0f;
    /*货物*/
    [SerializeField]
    private CargoType[] cargoes;
    private GameObject[] cargoes_obj;
    /*货物显示偏移量*/
    [SerializeField]
    private float cargo_offset = 1.0f;
    /*货物计数*/
    private int cargo_count;
    /*路线文件*/
    [SerializeField]
    private string route_path;
    /*搬运工路线*/
    private List<Route> worker_routes;
    /*搬运工当前路线前进程度*/
    private int current_route;
    private int current_length;
    private float move_cd;
    private float move_timer;
    private int last_beat;
    private float beat_count;
    [SerializeField] //debug
    private GameManager gameManager;
    //private bool isMove = false;
    /*搬运工前进方向*/
    private bool move_direction;  //true为按路线前进，false为按路线倒退
    private Rigidbody2D rd;
    /*货物对象池*/
    private ObjectPool[] cargo_objPool;
    
    public CargoType[] Cargoes
    {
        get
        {
            return cargoes;
        }
    }

    public bool Move_direction
    {
        get
        {
            return move_direction;
        }
        set
        {
            move_direction = value;
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
        //cargoes = new CargoType[4]{CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY};
        cargo_count = 0;
        cargoes_obj = new GameObject[4]{null, null, null, null};
        //对象池初始化
        GameObject temp = null;
        cargo_objPool = new ObjectPool[2];
        temp = GameObject.Find("CargoNormalPool");
        if (temp == null)
        {
            Debug.Log("No CargoNormalPool");
            return;
        }
        cargo_objPool[0] = temp.GetComponent<ObjectPool>();
        temp = null;
        temp = GameObject.Find("CargoWatchPool");
        if (temp == null)
        {
            Debug.Log("No CargoWatchPool");
            return;
        }
        cargo_objPool[1] = temp.GetComponent<ObjectPool>();
        //初始化路线
        LoadRoute();
        current_route = 0;
        current_length = 0;
        move_cd = gameManager.GCD / 2;
        move_timer = move_cd;
        last_beat = 0;
        beat_count = 0.0f;
        move_direction = true;
        rd = GetComponent<Rigidbody2D>();

        for (int i = 0; i < 4; i++)
        {
            if (cargoes[i] != CargoType.EMPTY && cargoes[i] != CargoType.Block)
            {
                AddCargoObject((Direction)i, cargoes[i]);
            }
        }
    }
    
    private void Update()
    {
        if (!gameManager.IsMove && (move_timer + Time.deltaTime) < speed)
        {
            move_timer = move_cd;
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
        if (move_timer < move_cd)
        {
            move_timer += Time.deltaTime;
            return;
        }
        beat_count -= 1.0f;
        move_timer = 0.0f;
        if (move_direction)
            MoveForward();
        else
            MoveBack();
    }

    /*加载路线*/
    private void LoadRoute()
    {
        worker_routes = new List<Route>();
        try
        {
            StreamReader sr = new StreamReader(Application.dataPath + "/Data/" + route_path);
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] data = line.Split(',');
                int dir = int.Parse(data[0]);
                int length = int.Parse(data[1]);
                Direction dir_data = (Direction)dir; 
                Route route_data = new Route(dir_data, length);
                //Debug.Log(route_data);
                worker_routes.Add(route_data);
                line = sr.ReadLine();
            }
        }
        catch
        {
            Debug.Log("Read Error");
        }
    }

    /*按路线前向移动*/
    private void MoveForward()
    {   
        switch(worker_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position += new Vector3(0.0f, move_length, 0.0f);
            break;
            case Direction.DOWN :
            transform.position += new Vector3(0.0f, -move_length, 0.0f);
            break;
            case Direction.RIGHT :
            transform.position += new Vector3(move_length, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position += new Vector3(-move_length, 0.0f, 0.0f);
            break;
        }
        current_length += 1;
        if (current_length >= worker_routes[current_route].length)
        {
            current_route++;
            if (current_route >= worker_routes.Count)
            {
                current_route = worker_routes.Count - 1;
                current_length = worker_routes[current_route].length;
                move_direction = false;
            }
            else
            {
                current_length = 0;
            }
        }
    }

    /*按路线回退*/
    private void MoveBack()
    {
        switch(worker_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position -= new Vector3(0.0f, move_length, 0.0f);
            break;
            case Direction.DOWN :
            transform.position -= new Vector3(0.0f, -move_length, 0.0f);
            break;
            case Direction.RIGHT :
            transform.position -= new Vector3(move_length, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position -= new Vector3(-move_length, 0.0f, 0.0f);
            break;
        }
        current_length -= 1;
        if (current_length <= 0)
        {
            current_route--;
            if (current_route < 0)
            {
                current_route = 0;
                current_length = 0;
                move_direction = true;
            }
            else
            {
                current_length = worker_routes[current_route].length;
            }
        }
        
    }

    /*添加货物的的显示*/
    private void AddCargoObject(Direction dir, CargoType type)
    {
        GameObject obj = cargo_objPool[(int)type].GetObject();
        obj.transform.parent = transform;
        obj.transform.localScale = new Vector3(1.0f / transform.localScale.x, 1.0f / transform.localScale.y, 1.0f / transform.localScale.z);
        switch (dir)
        {
            case Direction.UP :
            obj.transform.localPosition = new Vector3(0.0f, cargo_offset, 0.0f);
            break;
            case Direction.DOWN :
            obj.transform.localPosition = new Vector3(0.0f, -cargo_offset, 0.0f);
            break;
            case Direction.RIGHT:
            obj.transform.localPosition = new Vector3(cargo_offset, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            obj.transform.localPosition = new Vector3(-cargo_offset, 0.0f, 0.0f);
            break;
        }
        cargoes_obj[(int)dir] = obj;
    }

    /*删除货物显示*/
    private void DeleteCargoObject(int index, CargoType type)
    {
        //Debug.Log(id.ToString() + ' ' + index.ToString() + ' ' + type.ToString());
        cargo_objPool[(int)type].ReturnObject(cargoes_obj[index]);
        cargoes_obj[index] = null;
    }

    /*触发检测*/
    private void OnTriggerEnter2D(Collider2D other) 
    {
        //遇到障碍物回退
        if (other.tag == "Barrier")
        {
            move_direction  = !move_direction;
            move_timer = 0.0f;
            current_length -= 1;
            //Debug.Log("Back");
            return;
        }
        else if (other.tag == "Worker")
        {
            Worker w = other.GetComponent<Worker>();
            if (w.id < id)
            {
                return;
            }
            Vector3 dir = w.transform.position - transform.position;
            Direction passD = CalcuDirection(w.transform.position, transform.position);
            if (cargoes[(int)passD] == CargoType.EMPTY)
            {
                Debug.Log("Empty");
                return;
            }
            Direction getD = passD + 2;
            if ((int)getD > 3)
                getD -= 4;
            if (!w.IsEmpty(getD))
                return;
            //Debug.Log(passD.ToString() + ' ' + getD.ToString());
            w.GetCargo(getD, PassCargo(passD));
        }
        else if (other.tag == "InPoint")
        {
            //Debug.Log(1);
            UnloadPort ci = other.GetComponent<UnloadPort>();
            if (cargoes[(int)ci.In_direction] == CargoType.EMPTY || cargoes[(int)ci.In_direction] == CargoType.Block)
            {
                return;
            }
            ci.GetCargo(PassCargo(ci.In_direction));
            Debug.Log(2);
        }
    }

    /*递交货物*/
    public CargoType PassCargo(Direction dir)
    {
        int index = (int)dir;
        CargoType c = cargoes[index];
        cargoes[index] = CargoType.EMPTY;
        cargo_count--;
        DeleteCargoObject(index, c);
        return c;
    }

    /*接收货物*/
    public bool GetCargo(Direction dir, CargoType type)
    {
        int index = (int)dir;
        //Debug.Log(cargoes[index]);
        if (cargoes[index] == CargoType.EMPTY)
        {
            cargoes[index] = type;
            cargo_count++;
            AddCargoObject(dir, type);
            return true;
        }
        return false;
    }

    /*相对方向判定*/
    private Direction CalcuDirection(Vector3 target, Vector3 ori)
    {
        Vector3 dir = target - ori;
        Direction direction;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
        {
            if (dir.x > 0)
                direction = Direction.RIGHT;
                else
                direction = Direction.LEFT;
            }
        else
        {
            if (dir.z > 0)
                direction = Direction.UP;
            else
                direction = Direction.DOWN;
        }
        return direction;
    }

    /*判断对应位置是否为空*/
    public bool IsEmpty(Direction dir)
    {
        if (cargoes[(int)dir] == CargoType.EMPTY)
            return true;
        else
            return false;
    }

    public void ChangeMoveDirection()
    {

    }
    
}
