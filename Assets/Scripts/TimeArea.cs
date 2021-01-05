using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chronos;

public class TimeArea : MonoBehaviour
{
    /*是否被选中*/
    public bool isSelected;
    /*按钮是否出现*/
    public bool isButtonON;
    /*按钮*/
    [SerializeField]
    private Button[] buttons;
    
    /*能力时间*/
    [SerializeField]
    [Header("持续时间")]
    private int power_time;
    private int power_timer;
    private int power_type = -1;
    /*能力时间进度条*/
    [SerializeField]
    private Slider powerTime_slider;
    [SerializeField]
    private bool isPowerON;
    [SerializeField]
    [Header("冷却时间")]
    private int powerCD;
    private int powerCD_timer = 0;
    /*冷却时间进度条*/
    [SerializeField]
    private Slider powerCD_slider;
    [SerializeField]
    private bool isPowerCD;
    private Color areaColor;
    [SerializeField]
    private GameManager gameManager;
    private int last_beat;
    private List<Worker> workers;
    //区域内的卸货口
    [Header("区域内的卸货口")]
    [SerializeField]
    private UnloadPort[] unloadPorts;
    //区域内的上货口
    [Header("区域内的上货口")]
    [SerializeField]
    private LoadPort[] loadPorts;

    private void Start()
    {
        workers = new List<Worker>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        isSelected = false;
        isButtonON = false;
        isPowerON = false;
        isPowerCD = false;
        areaColor = GetComponent<SpriteRenderer>().color;
        last_beat = 0;
        //beat_count = 0;
    }

    private void Update()
    {
        if (isPowerON)
        {
            powerTime_slider.gameObject.SetActive(true);
            powerTime_slider.value = 1.0f - (float)power_timer / power_time;
            power_timer += (gameManager.Beat_times - last_beat);
            if (power_timer > power_time)
            {
                isPowerON = false;
                power_timer = 0;
                powerTime_slider.value = 1.0f;
                powerTime_slider.gameObject.SetActive(false);
                power_type = -1;
                //能力时间结束还原
                for(int i = 0; i < workers.Count; i++)
                {
                    workers[i].Speed = 1.0f;
                }
                for (int i = 0; i < loadPorts.Length; i++)
                {
                    loadPorts[i].Speed = 1.0f;
                }
                for (int i = 0; i < unloadPorts.Length; i++)
                {
                    unloadPorts[i].Speed = 1.0f;
                }
            }
            //设置范围内物体效果
            for(int i = 0; i < workers.Count; i++)
            {
                if (power_type == 1)
                {
                    workers[i].Speed = 2.0f;
                }
                else if (power_type == 0)
                {
                    workers[i].Speed = 0.5f;
                }
            }
            for (int i = 0; i < loadPorts.Length; i++)
            {
                if (power_type == 1)
                {
                    loadPorts[i].Speed = 2.0f;
                }
                else if (power_type == 0)
                {
                    loadPorts[i].Speed = 0.5f;
                }
            }
            for (int i = 0; i < unloadPorts.Length; i++)
            {
                if (power_type == 1)
                {
                    unloadPorts[i].Speed = 2.0f;
                }
                else if (power_type == 0)
                {
                    unloadPorts[i].Speed = 0.5f;
                }
            }
        }
        else
        {
            powerTime_slider.gameObject.SetActive(false);
        }
        if (!isSelected && !isPowerON)
        {
            GetComponent<SpriteRenderer>().color = Color.clear;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = areaColor; 
        }
        if (isPowerCD)
        {
            powerCD_slider.value = 1.0f - (float)powerCD_timer / powerCD;
            powerCD_timer += (gameManager.Beat_times - last_beat);
            if (powerCD_timer > powerCD)
            {
                powerCD_timer = 0;
                isPowerCD = false;
                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].interactable = true;
                }
                powerCD_slider.value = 1.0f;
                powerCD_slider.gameObject.SetActive(false);
            }
        }
        last_beat = gameManager.Beat_times;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Worker")
        {
            
            Worker w = other.gameObject.GetComponent<Worker>();
            workers.Add(w);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Worker")
        {
            //Debug.Log("DELETE");
            Worker w = other.gameObject.GetComponent<Worker>();
            w.Speed = 1.0f;
            workers.Remove(w);
        }
    }

    public void ButtonON()
    {
        isButtonON = true;
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
        isButtonON = false;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        powerCD_slider.gameObject.SetActive(false);
    }

    public void Accelerate()
    {
        isPowerON = true;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        isPowerCD = true;
        powerCD_slider.gameObject.SetActive(true);
        power_type = 1;
        ButtonOFF();
    }

    public void Decelerate()
    {
        isPowerON = true;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        isPowerCD = true;
        powerCD_slider.gameObject.SetActive(true);
        power_type = 0;
        ButtonOFF();
    } 
}
