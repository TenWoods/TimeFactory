using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Header("格子的大小")]
    private float grid_size;
    [SerializeField]
    [Header("格子材质")]
    private Material grid_material;
    private Vector3 screen_size;
    //行数
    private int row_count;
    //列数
    private int col_count;
    //世界是否移动
    private bool isMove;
    //节拍长度
    [SerializeField]
    private float gcd;
    //节拍计次
    private int beat_times;
    //按键冷却计时
    private float timer;

    public bool IsMove 
    {
        get
        {
            return isMove;
        }
    }

    public int Beat_times

    {
        get
        {
            return beat_times;
        }
    }

    public float GCD
    {
        get
        {
            return gcd;
        }
    }

    private void Start()
    {
        timer = 0.0f;
        beat_times = 0;
        Vector3 leftDown_vieport = new Vector3(0, 0);
        Vector3 rightUp_vieport = new Vector3(1, 1);
        Vector3 leftDown_world = Camera.main.ViewportToWorldPoint(leftDown_vieport);
        Vector3 rightUp_world = Camera.main.ViewportToWorldPoint(rightUp_vieport);
        screen_size = rightUp_world - leftDown_world;
        row_count = (int)(screen_size.y / grid_size) + 1;
        col_count = (int)(screen_size.x / grid_size) + 1;
    }

    private void Update()
    {
        if (isMove)
        {
            timer += Time.deltaTime;
            if (timer >= gcd)
            {
                isMove = false;
            }
        }
    }

    
    
    private void OnRenderObject() 
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        //Debug.Log("?");
        Gizmos.color = Color.black;
        Vector3 startP;
        Vector3 endP;
        //画横线
        for (int i = 0; i < row_count; i++)
        {
            startP = new Vector3(-screen_size.x / 2, -(int)screen_size.y / 2 + i * grid_size - 0.5f);
            endP = new Vector3(screen_size.x / 2, -(int)screen_size.y / 2 + i * grid_size - 0.5f);
            GL.Begin(GL.LINES);
            grid_material.SetPass(0);
            //GL.Color(Color.black);
            GL.Vertex3(startP.x, startP.y, 0.0f);
            GL.Vertex3(endP.x, endP.y, 0.0f);
            GL.End();
        }
        //画竖线
        for (int i = 0; i < col_count; i++)
        {
            startP = new Vector3(-(int)screen_size.x / 2  + i * grid_size - 0.5f, -screen_size.y / 2);
            endP = new Vector3(-(int)screen_size.x / 2 + i * grid_size - 0.5f, screen_size.y / 2);
            GL.Begin(GL.LINES);
            grid_material.SetPass(0);
            //GL.Color(Color.black);
            GL.Vertex3(startP.x, startP.y, 0.0f);
            GL.Vertex3(endP.x, endP.y, 0.0f);
            GL.End();
        }
    }

    public void DropTheBeat()
    {
        if (!isMove)
        {
            timer = 0.0f;
            isMove = true;
            beat_times++;
        }
    }

}
