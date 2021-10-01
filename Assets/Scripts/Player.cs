using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 mousePosition;

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        transform.position = Vector2.Lerp(transform.position, mousePosition, Time.deltaTime);
        //transform.position = Vector2.MoveTowards(transform.position, mousePosition, Time.deltaTime);
    }
}
