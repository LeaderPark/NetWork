using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    const float speed = 3.0f;
    bool isMoving = false;
    bool bMovable = false;

    float timeTODest;
    float elapsed;

    GameManager gm;
    Vector3 targetPos;
    Vector3 originPos;
        Vector2 mousePosition;
    private void Start() 
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        originPos = transform.position;
        targetPos = originPos;
        timeTODest = 0;
        isMoving = false;
    }
    
    private void Update() 
    {
        if (isMoving)
        {

        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime);
        }
    }   
}
