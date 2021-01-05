using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasEye : BaseTimeObject
{
    private int segment = 10;
    /*视野半径*/
    [SerializeField, Header("视野距离")]
    private float radius;
    /*视野角度*/
    [SerializeField, Header("视野角度")]
    protected float degree;
    private Mesh fan_mesh;
    [SerializeField]
    protected GameObject eyeArea;
    /*场景中所有的工人*/
    [SerializeField]
    private Worker[] workers;
    
    protected virtual void Start()
    {
        GenerateMesh();
        eyeArea.GetComponent<MeshFilter>().mesh = fan_mesh;
        eyeArea.transform.rotation = Quaternion.Euler(0.0f, 0.0f,  - (degree / 2));

        GameObject[] worker_obj = GameObject.FindGameObjectsWithTag("Worker");
        workers = new Worker[worker_obj.Length];
        for (int i = 0; i < worker_obj.Length; i++)
        {
            workers[i] = worker_obj[i].GetComponent<Worker>();
        }
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
        float x, y;
        for (int i = 1; i < vertex_num; i++)
        {
            x = Mathf.Cos(current_radians);
            y = Mathf.Sin(current_radians);
            vertices[i] = new Vector3(radius * x, radius * y, 0.0f);
            uvs[i] = new Vector2(x, y);
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

    protected void CheckWorkers()
    {
        
        for (int i = 0; i < workers.Length; i++)
        {
            Vector3 dierction = workers[i].gameObject.transform.position - transform.position;
            
            float distance = dierction.magnitude;
            if (distance > radius)
            {
                Debug.Log("Out");
                continue;
            } 
            float dir_degree = Vector3.Dot(dierction.normalized, transform.forward.normalized);
            Debug.Log(transform.forward.normalized);
            if (dir_degree < 0)
            {
                Debug.Log("Back");
                continue;
            }
            
            if (dir_degree >= Mathf.Cos(degree / 2 * Mathf.Deg2Rad))
            {
                Debug.Log("See");
                // for (int j = 0; j < workers[i].Cargoes.Length; j++)
                // {
                //     if (workers[i].Cargoes[j] == CargoType.Alice)
                //     {
                //         Debug.Log("See");
                //     }
                // }
            }
        }
    }
}
