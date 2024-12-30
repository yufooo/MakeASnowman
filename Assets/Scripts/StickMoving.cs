using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickMoving : MonoBehaviour
{
    public GameObject HandleButton;
    
    void Update()
    {
        //영역제한
        HandleButton.transform.position = new Vector2(
            Mathf.Clamp(HandleButton.transform.position.x, 
                gameObject.GetComponent<CircleCollider2D>().bounds.min.x, 
                gameObject.GetComponent<CircleCollider2D>().bounds.max.x),
            Mathf.Clamp(HandleButton.transform.position.y,
                gameObject.GetComponent<CircleCollider2D>().bounds.min.y, 
                gameObject.GetComponent<CircleCollider2D>().bounds.max.y));
           
    }
}
