using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private TimeArea current_area;

    private void Start()
    {
        current_area = null;
    }

    private void Update()
    {
        CheckArea();
        CheckButton();
    }

    private void CheckArea()
    { 
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider)
        {
            if (hit.collider.tag == "AreaClock")
            {
                if (current_area != null)
                {
                    current_area.isSelected = false;
                    //current_area.ButtonOFF();
                }
                current_area = hit.collider.gameObject.GetComponent<TimeArea>();
                current_area.isSelected = true;
            }
        }
        else
        {
            if (current_area != null)
            {
                current_area.isSelected = false;
                current_area.ButtonOFF();
            }
            current_area = null;
        }
    }

    private void CheckButton()
    {
        if (Input.GetMouseButton(0))
        {
            if (current_area != null)
            {
                current_area.ButtonON();
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (current_area != null)
            {
                current_area.ButtonOFF();
            }
        }
    }
}
