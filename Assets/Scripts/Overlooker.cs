using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlooker : MonoBehaviour
{
    [SerializeField]
    private int segment;
    /*视野半径*/
    [SerializeField]
    [Header("视野距离")]
    private float radius;
    private float innerRadius = 0.0f;
    [SerializeField]
    [Header("视野大小")]
    private float degree;
    private Mesh fan_mesh;
    [SerializeField]
    private GameObject eyeArea;
    
    private void Start()
    {
        GenerateMesh();
        eyeArea.GetComponent<MeshFilter>().mesh = fan_mesh;
        //eyeArea.GetComponent<MeshCollider>().sharedMesh = fan_mesh;
    }

    private void Update()
    {
        
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
}
