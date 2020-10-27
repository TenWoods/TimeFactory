using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlooker : BaseTimeObject
{
    private int segment = 10;
    /*视野半径*/
    [SerializeField, Header("视野距离")]
    private float radius;
    /*视野角度*/
    [SerializeField, Header("视野角度")]
    private float degree;
    private Mesh fan_mesh;
    [SerializeField]
    private GameObject eyeArea;
    /*场景中所有的工人*/
    private Worker[] workers;
    [SerializeField]
    private GameObject cargo_point;
    /*货物对象池*/
    private ObjectPool watch_objPool;
    private float time_degree = 0.0f;
    
    private void Start()
    {
        //对象池初始化
        GameObject temp = null;
        temp = GameObject.Find("CargoWatchPool");
        if (temp == null)
        {
            Debug.Log("No CargoWatchPool");
            return;
        }
        watch_objPool = temp.GetComponent<ObjectPool>();

        GenerateMesh();
        eyeArea.GetComponent<MeshFilter>().mesh = fan_mesh;
        eyeArea.transform.rotation = Quaternion.Euler(0.0f, degree / 2.0f - 90.0f, 0.0f);

        GameObject[] worker_obj = GameObject.FindGameObjectsWithTag("Worker");
        workers = new Worker[worker_obj.Length];
        for (int i = 0; i < worker_obj.Length; i++)
        {
            workers[i] = worker_obj[i].GetComponent<Worker>();
        }
    }

    private void Update()
    {
        CheckWorkers();
        time_degree += time.deltaTime * 10.0f;
        transform.rotation = Quaternion.Euler(0.0f, time_degree, 0.0f);
    }

    private void GenerateMesh()
    {
        int vertex_num = segment + 2;
        Vector3[] vertices = new Vector3[vertex_num];
        Vector2[] uvs = new Vector2[vertex_num];
        float radians = Mathf.Deg2Rad * degree;
        float per_radians = radians / segment;
        float current_radians = 0.0f;
        vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
        float x, z;
        for (int i = 1; i < vertex_num; i++)
        {
            x = Mathf.Cos(current_radians);
            z = Mathf.Sin(current_radians);
            vertices[i] = new Vector3(radius * x, 0.0f, radius * z);
            uvs[i] = new Vector2(x, z);
            current_radians += per_radians;
        }
        int[] triangles = new int[segment * 3];
        int index = 0;
        for (int i = 0; i < segment; i++)
        {
            triangles[index] = i + 2;
            triangles[index + 1] = i + 1;
            triangles[index + 2] = 0;
            index += 3;
        }
        fan_mesh = new Mesh();
        fan_mesh.vertices = vertices;
        fan_mesh.triangles = triangles;
        fan_mesh.uv = uvs;
        fan_mesh.RecalculateNormals();
        
    }

    private void CheckWorkers()
    {
        for (int i = 0; i < workers.Length; i++)
        {
            Vector3 dierction = workers[i].gameObject.transform.position - transform.position;
            //Debug.Log(dierction.normalized);
            float distance = dierction.magnitude;
            if (distance > radius)
            {
                continue;
            } 
            // float angle = Vector3.Angle(dierction.normalized, transform.forward.normalized);
            // Debug.Log(angle);
            float dir_degree = Vector3.Dot(dierction.normalized, transform.forward.normalized);
            if (dir_degree < 0)
            {
                continue;
            }
            if (dir_degree >= Mathf.Cos(degree / 2 * Mathf.Deg2Rad))
            {
                for (int j = 0; j < workers[i].Cargoes.Length; j++)
                {
                    if (workers[i].Cargoes[j] == CargoType.WATCH)
                    {
                        CargoType temp = workers[i].PassCargo((Direction)j);
                        GameObject watch = watch_objPool.GetObject();
                        watch.transform.parent = cargo_point.transform;
                        watch.transform.localPosition = Vector3.zero;
                        watch.transform.localScale = Vector3.one;
                        Debug.Log("See");
                    }
                }
            }
        }
    }
}
