using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Patrolman : HasEye
{
    /*移动速度*/
    [Header("巡逻员移动速度")]
    [SerializeField]
    private float speed;
    [Header("单位移动距离")]
    [SerializeField]
    /*每一次移动的距离*/
    private float move_length = 1.0f;
    /*路线文件*/
    [SerializeField]
    private string route_path;
    /*巡逻员路线*/
    private List<Route> patrolman_routes;
    /*巡逻员当前路线前进程度*/
    private int current_route;
    private int current_length;
    private float move_timer;
    /*巡逻员前进方向*/
    private bool move_direction;  //true为按路线前进，false为按路线倒退
    private Rigidbody2D rd;
    [SerializeField]
    private GameManager gameManager;

    private void Start()
    {
        base.Start();
        LoadRoute();
        current_route = 0;
        current_length = 0;
        move_timer = speed;
        move_direction = true;
        rd = GetComponent<Rigidbody2D>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        if (!gameManager.IsMove)
        {
            //move_timer = speed;
            return;
        }
        if (move_timer < speed)
        {
            move_timer += time.deltaTime;
            return;
        }
        if (move_direction)
            MoveForward();
        else
            MoveBack();
    }

    /*加载路线*/
    private void LoadRoute()
    {
        patrolman_routes = new List<Route>();
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
                patrolman_routes.Add(route_data);
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
        move_timer = 0.0f;
        switch(patrolman_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position += new Vector3(0.0f, move_length, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            break;
            case Direction.DOWN :
            transform.position += new Vector3(0.0f, -move_length, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f);
            break;
            case Direction.RIGHT :
            transform.position += new Vector3(move_length, 0.0f, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            transform.position += new Vector3(-move_length, 0.0f, 0.0f);
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f);
            break;
        }
        current_length += 1;
        if (current_length >= patrolman_routes[current_route].length)
        {
            current_route++;
            if (current_route >= patrolman_routes.Count)
            {
                current_route = patrolman_routes.Count - 1;
                current_length = patrolman_routes[current_route].length;
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
        if (move_timer < speed)
        {
            move_timer += time.deltaTime;
            return;
        }
        move_timer = 0.0f;
        switch(patrolman_routes[current_route].direction)
        {
            case Direction.UP :
            transform.position -= new Vector3(0.0f, move_length, 0.0f);
            eyeArea.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 270.0f - (degree / 2));
            break;
            case Direction.DOWN :
            transform.position -= new Vector3(0.0f, -move_length, 0.0f);
            eyeArea.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - (degree / 2));
            break;
            case Direction.RIGHT :
            transform.position -= new Vector3(move_length, 0.0f, 0.0f);
            eyeArea.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180.0f - (degree / 2));
            break;
            case Direction.LEFT :
            transform.position -= new Vector3(-move_length, 0.0f, 0.0f);
            eyeArea.transform.rotation = Quaternion.Euler(0.0f, 0.0f,  -degree / 2);
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
                current_length = patrolman_routes[current_route].length;
            }
        }
        
    }
}
