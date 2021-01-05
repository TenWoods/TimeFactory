using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField]
    [Header("开关方向")]
    private Direction direction;
    [SerializeField]
    private float laserLength = 1.0f;
    [SerializeField]//Debug
    private bool isON;
    [SerializeField]
    private GameObject laser;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (isON)
        {
            Ray2D ray;
            switch (direction)
            {
                case Direction.UP :
                ray = new Ray2D(transform.position, Vector2.up * laserLength);
                break;
                case Direction.LEFT :
                ray = new Ray2D(transform.position, Vector2.left * laserLength);
                break;
                case Direction.DOWN :
                ray = new Ray2D(transform.position, Vector2.down * laserLength);
                break;
                case Direction.RIGHT :
                ray = new Ray2D(transform.position, Vector2.right * laserLength);
                break;
                default :
                ray = new Ray2D(); //待改进
                break;
            }
            RaycastHit2D info = Physics2D.Raycast(ray.origin, ray.direction, laserLength);
            if (info.collider != null)
            {
                if (info.collider.tag == "Worker")
                {
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                    laser.SetActive(true);
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.black);
                laser.SetActive(false);
            }
        }
    }

    public void TurnON()
    {
        isON = true;
    }

    public void TurnOFF()
    {
        isON = false;
    }
}
