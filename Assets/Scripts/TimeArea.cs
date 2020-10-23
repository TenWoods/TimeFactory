using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chronos;

[ExecuteInEditMode]
public class TimeArea : BaseTimeObject
{
    /*是否被选中*/
    public bool isSelected;
    /*边框材质*/
    [SerializeField]
    private Material lineMaterial;
    /*按钮*/
    [SerializeField]
    private Button[] buttons;
    private bool isButtonON;
    /*能力时间*/
    [SerializeField]
    [Header("持续时间")]
    private float power_time;
    private float power_timer = 0.0f;
    /*能力时间进度条*/
    [SerializeField]
    private Slider powerTime_slider;
    private bool isPowerON;
    private AreaClock3D m_clock;
    [SerializeField]
    [Header("冷却时间")]
    private float powerCD;
    private float powerCD_timer = 0.0f;
    /*冷却时间进度条*/
    [SerializeField]
    private Slider powerCD_slider;
    private bool isPowerCD;
    [SerializeField]
    [Header("加速程度")]
    private float accelerate_scale;
    [SerializeField]
    [Header("减速程度")]
    private float decelerate_scale;
    [SerializeField]
    private float rewind_scale;
    

    private void Start()
    {
        m_clock = GetComponent<AreaClock3D>();
        isSelected = false;
        isButtonON = false;
        isPowerON = false;
        isPowerCD = false;
    }

    private void Update()
    {
        if (isPowerON)
        {
            powerTime_slider.gameObject.SetActive(true);
            powerTime_slider.value = 1.0f - power_timer / power_time;
            power_timer += time.deltaTime;
            if (power_timer > power_time)
            {
                isPowerON = false;
                power_timer = 0.0f;
                powerTime_slider.value = 1.0f;
                m_clock.localTimeScale = 1.0f;
                powerTime_slider.gameObject.SetActive(false);
            }
        }
        if (isPowerCD)
        {
            powerCD_slider.value = 1.0f - powerCD_timer / powerCD;
            powerCD_timer += time.deltaTime;
            if (powerCD_timer > powerCD)
            {
                powerCD_timer = 0.0f;
                isPowerCD = false;
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].interactable = true;
                }
                powerCD_slider.value = 1.0f;
                powerCD_slider.gameObject.SetActive(false);
            }
        }
    }

    private void OnRenderObject() 
    {
        if (!isSelected && !isPowerON)
        {
            return;
        }
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider == null)
        {
            return;
        }
        if (lineMaterial == null)
        {
            Debug.LogError("No Material!!");
            return;
        }
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        Vector3 center = collider.bounds.center;
        Vector3 size = collider.bounds.size;
        float rx = size.x / 2.0f;
        float ry = size.y / 2.0f;
        float rz = size.z / 2.0f;
        Vector3 p0, p1, p2, p3;  
        Vector3 p4, p5, p6, p7;
        p0 = center + new Vector3(-rx, -ry, rz);  
        p1 = center + new Vector3(rx, -ry, rz);  
        p2 = center + new Vector3(rx, -ry, -rz);  
        p3 = center + new Vector3(-rx, -ry, -rz);  
        p4 = center + new Vector3(-rx, ry, rz);  
        p5 = center + new Vector3(rx, ry, rz);  
        p6 = center + new Vector3(rx, ry, -rz);  
        p7 = center + new Vector3(-rx, ry, -rz);
        DrawLine(p0, p1);
        DrawLine(p1, p2);
        DrawLine(p2, p3);
        DrawLine(p0, p3);
        DrawLine(p4, p5);
        DrawLine(p5, p6); 
        DrawLine(p6, p7);
        DrawLine(p4, p7);
        DrawLine(p0, p4); 
        DrawLine(p1, p5);
        DrawLine(p2, p6);
        DrawLine(p3, p7);
        GL.PopMatrix(); 
    }

    private void DrawLine(Vector3 p0, Vector3 p1)
    {
        GL.Begin(GL.LINES);  
        GL.Color(Color.cyan);  
        GL.Vertex(p0);  
        GL.Vertex(p1);  
        GL.End();  
    }

    public void ButtonON()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(true);
        }
        if (isPowerCD)
        {
            powerCD_slider.gameObject.SetActive(true);
        }
    }

    public void ButtonOFF()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        powerCD_slider.gameObject.SetActive(false);
    }

    public void Accelerate()
    {
        m_clock.localTimeScale = accelerate_scale;
        isPowerON = true;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        isPowerCD = true;
        powerCD_slider.gameObject.SetActive(true);
    }

    public void Decelerate()
    {
        m_clock.localTimeScale = decelerate_scale;
        isPowerON = true;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        isPowerCD = true;
        powerCD_slider.gameObject.SetActive(true);
    }

    public void Rewind()
    {
        m_clock.localTimeScale = rewind_scale;
        isPowerON = true;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        isPowerCD = true;
        powerCD_slider.gameObject.SetActive(true);
    }
}
