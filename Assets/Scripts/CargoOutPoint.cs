using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CargoOutPoint : BaseTimeObject
{
    /*货物数据路径*/
    [SerializeField]
    private string path;
    /*出货口朝向*/
    [SerializeField]
    private Direction direction;
    /*出货口的存货*/
    private Stack<CargoType> cargoes;
    /*货物对象池*/
    private ObjectPool[] cargo_objPool;
    /*当前货物*/
    private GameObject current_cargo;
    /*是否更新当前出口的货物*/
    private bool updateCargo;
    /*出货cd*/
    [SerializeField]
    [Header("出货CD")]
    private float out_cd;
    private float out_timer;

    private void Start()
    {
        updateCargo = false;
        LoadData();
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
        
        UpdateCurrentCargo();
        out_timer = 0.0f;
    }
    
    private void Update()
    {
        if (updateCargo)
        {
            if (out_timer < out_cd)
            {
                out_timer += time.deltaTime;
                return;
            }
            out_timer = 0.0f;
            UpdateCurrentCargo();
            updateCargo = false;
        }
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

    /*更新当前货物*/
    private void UpdateCurrentCargo()
    {
        CargoType top = cargoes.Peek();
        current_cargo = cargo_objPool[(int)top].GetObject();
        current_cargo.SetActive(true);
        current_cargo.transform.parent = transform;
        current_cargo.transform.localScale = Vector3.one;
        switch (direction)
        {
            case Direction.UP :
            current_cargo.transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            break;
            case Direction.DOWN :
            current_cargo.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
            break;
            case Direction.RIGHT:
            current_cargo.transform.localPosition = new Vector3(1.0f, 0.0f, 0.0f);
            break;
            case Direction.LEFT :
            current_cargo.transform.localPosition = new Vector3(-1.0f, 0.0f, 0.0f);
            break;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        //当接触到搬运工后递交货物
        if (other.tag == "Worker")
        {
            Worker w = other.GetComponent<Worker>();
            Vector3 dir = transform.position - w.transform.position;
            Direction d;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            {
                if (dir.x > 0)
                    d = Direction.RIGHT;
                else
                    d = Direction.LEFT;
            }
            else
            {
                if (dir.z > 0)
                    d = Direction.UP;
                else
                    d = Direction.DOWN;
            }
            if (cargoes.Count == 0)
            {
                return;
            }
            CargoType cargo = cargoes.Pop();
            if (!w.GetCargo(d, cargo))
            {
                //没递交的货物压回
                cargoes.Push(cargo);
            }
            else
            {
                cargo_objPool[(int)cargo].ReturnObject(current_cargo);
                updateCargo = true;
                //Debug.Log(d.ToString() + ' ' + cargo.ToString());
            }
        }    
    }
}
