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
    WATCH = 1,
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

public class Worker : BaseTimeObject
{
    /*搬运工序号*/
    [Header("搬运工序号")]
    [SerializeField]
    private int id;
    /*移动速度*/
    [Header("搬运工移动速度")]
    [SerializeField]
    private float speed;
    /*货物*/
    [SerializeField]
    private CargoType[] cargoes;
    private GameObject[] cargoes_obj;
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
    private float move_timer;
    /*搬运工前进方向*/
    private bool move_direction;  //true为按路线前进，false为按路线倒退
    private Rigidbody rd;
    /*货物对象池*/
    private ObjectPool[] cargo_objPool;
    
    public CargoType[] Cargoes
    {
        get
        {
            return cargoes;
        }
    }

    private void Start()
    {
        cargoes = new CargoType[4]{CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY};
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

        LoadRoute();
        current_route = 0;
        current_length = 0;
        move_timer = speed;
        move_direction = true;
        rd = GetComponent<Rigidbody>();

    }
    
    private void Update()
    {
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
        if (move_timer < speed)
        {
            move_timer += time.deltaTime;
            return;
        }
        move_timer = 0.0f;
        switch(worker_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position += new Vector3(0.0f, 0.0f, 1.0f);
            break;
            case Direction.DOWN :
            transform.position += new Vector3(0.0f, 0.0f, -1.0f);
            break;
            case Direction.RIGHT :
            transform.position += new Vector3(1.0f, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
            break;
        }
        current_length += 1;
        //Debug.Log("move" + move_timer.ToString());
        if (current_length > worker_routes[current_route].length)
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
        if (move_timer < speed )
        {
            move_timer += time.deltaTime;
            return;
        }
        move_timer = 0.0f;
        switch(worker_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position -= new Vector3(0.0f, 0.0f, 1.0f);
            break;
            case Direction.DOWN :
            transform.position -= new Vector3(0.0f, 0.0f, -1.0f);
            break;
            case Direction.RIGHT :
            transform.position -= new Vector3(1.0f, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position -= new Vector3(-1.0f, 0.0f, 0.0f);
            break;
        }
       //Debug.Log("move" + move_timer.ToString());
        current_length -= 1;
        if (current_length < 0)
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
        obj.transform.localScale = Vector3.one;
        switch (dir)
        {
            case Direction.UP :
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            break;
            case Direction.DOWN :
            obj.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
            break;
            case Direction.RIGHT:
            obj.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            obj.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
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
    private void OnTriggerEnter(Collider other) 
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
            CargoInPoint ci = other.GetComponent<CargoInPoint>();
            Direction in_dir = CalcuDirection(other.transform.position, transform.position);
            if (cargoes[(int)in_dir] == CargoType.EMPTY)
            {
                return;
            }
            Direction match_dir = in_dir + 2;
            if ((int)match_dir > 3)
            {
                match_dir -= 4;
            }
            if (match_dir == ci.In_direction && !ci.IsInCD())
            {
                ci.GetCargo(PassCargo(in_dir));
            }
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
        if (cargoes[index] == CargoType.EMPTY)
        {
            cargoes[index] = type;
            cargo_count++;
            AddCargoObject(dir, type);
            return true;
        }
        for (int i = 0; i < 3; i++)
        {
            if (++index > 3)
                index -= 4;
            if (cargoes[index] == CargoType.EMPTY)
            {
                cargoes[index] = type;
                cargo_count++;
                AddCargoObject((Direction)index, type);
                return true;
            }
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
    
}
