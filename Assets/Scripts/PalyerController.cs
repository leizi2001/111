using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalyerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 movement;
    private float horizontalInput;
    private float verticalInput;
    public float speed;//移动速度
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        movement = new Vector2(horizontalInput, verticalInput).normalized;
    }

    private void FixedUpdate()
    {
        //移动代码
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}
