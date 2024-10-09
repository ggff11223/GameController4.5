using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller123 : MonoBehaviour
{
    public Animator playerAnim;
    public Rigidbody playerRigid;
    public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed;
    public float jumpForce = 10f; // เพิ่มค่า jumpForce

    public bool walking;
    public Transform playerTrans;
    private bool isJumping = false;
    private bool isGrounded = true; // ตรวจสอบว่าตัวละครอยู่บนพื้นหรือไม่

    void Start()
    {
        olw_speed = w_speed;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRigid.velocity = transform.forward * w_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRigid.velocity = -transform.forward * wb_speed * Time.deltaTime;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isJumping)
        {
            playerAnim.SetTrigger("walk");
            playerAnim.ResetTrigger("Idle");
            walking = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnim.ResetTrigger("walk");
            playerAnim.SetTrigger("Idle");
            walking = false;
        }
        if (Input.GetKeyDown(KeyCode.S) && !isJumping)
        {
            playerAnim.SetTrigger("WalkingBackwards");
            playerAnim.ResetTrigger("Idle");
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnim.ResetTrigger("WalkingBackwards");
            playerAnim.SetTrigger("Idle");
        }

        if (Input.GetKey(KeyCode.A))
        {
            playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
        }

        if (walking)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                w_speed += rn_speed;
                playerAnim.SetTrigger("Running");
                playerAnim.ResetTrigger("walk");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                w_speed = olw_speed;
                playerAnim.ResetTrigger("Running");
                playerAnim.SetTrigger("walk");
            }
        }

        // การกระโดด
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // เช็คว่าตัวละครอยู่บนพื้น
        {
            playerRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerAnim.SetTrigger("MutantJumping");
            isJumping = true;
            isGrounded = false; // ตั้งค่าสถานะไม่อยู่บนพื้น
        }
    }

    // ตรวจสอบการชนกับพื้น
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // ให้แน่ใจว่าแท็กของพื้นคือ "Ground"
        {
            isGrounded = true; // เมื่อตัวละครแตะพื้นอีกครั้ง
            isJumping = false;
        }
    }
}
