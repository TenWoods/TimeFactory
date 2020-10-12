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
    public float length;

    public Route(Direction dir, float l)
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
    private CargoType[] cargoes;
    /*货物计数*/
    private int cargo_count;
    /*路线文件*/
    [SerializeField]
    private string route_path;
    /*搬运工路线*/
    private List<Route> worker_routes;
    /*搬运工当前路线前进程度*/
    private int current_route;
    private float current_length;
    /*搬运工前进方向*/
    private bool move_direction;  //true为按路线前进，false为按路线倒退
    private Rigidbody2D rd;
    
    
    private void Start()
    {
        cargoes = new CargoType[4]{CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY, CargoType.EMPTY};
        cargo_count = 0;

        LoadRoute();
        current_route = 0;
        current_length = 0.0f;
        move_direction = true;
        rd = GetComponent<Rigidbody2D>();

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
                float length = float.Parse(data[1]);
                Direction dir_data = (Direction)dir; 
                Route route_data = new Route(dir_data, length);
                Debug.Log(route_data);
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
        float move_length = speed * time.deltaTime;
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
        current_length += move_length;
        if (current_length > worker_routes[current_route].length)
        {
            current_route++;
            if (current_route > (worker_routes.Count - 1))
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
        float move_length = speed * time.deltaTime;
        switch(worker_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position -= new Vector3(0.0f, move_length, 0.0f);
            break;
            case Direction.DOWN :
            transform.position -= new Vector3(0.0f, -move_length,0.0f);
            break;
            case Direction.RIGHT :
            transform.position -= new Vector3(move_length, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position -= new Vector3(-move_length, 0.0f, 0.0f);
            break;
        }
        current_length -= move_length;
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

    /*接收货物*/
    public bool GetCargo(Direction dir, CargoType type)
    {
        int index = (int)dir;
        if (cargoes[index] == CargoType.EMPTY)
        {
            cargoes[index] = type;
            cargo_count++;
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
                return true;
            }
        }
        return false;
    }

    /*递交货物*/
    public CargoType PassCargo(Direction dir)
    {
        int index = (int)dir;
        return cargoes[index];
    }
    
}
