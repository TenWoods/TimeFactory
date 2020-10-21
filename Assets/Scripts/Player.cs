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
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "AreaClock")
            {
                
                current_area = hit.collider.gameObject.GetComponent<TimeArea>();
                current_area.isSelected = true;
            }
            else
            {
                if (current_area != null)
                {
                    current_area.isSelected = false;
                    current_area.ButtonOFF();
                }
            }
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
    }
}
