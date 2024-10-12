using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller1 : MonoBehaviour
{
    public Animator playerAnim;          // อ้างอิงถึง Animator
    public Rigidbody playerRigid;        // อ้างอิงถึง Rigidbody
    public float moveSpeed = 5f;         // ความเร็วในการเดิน
    public float runSpeed = 8f;          // ความเร็วในการวิ่ง
    public float jumpForce = 5f;         // แรงกระโดด
    private bool isGrounded;             // ตรวจสอบว่าตัวละครอยู่บนพื้นหรือไม่

    public bool canPickup = false;       // ตรวจสอบว่าสามารถเก็บไอเทมได้หรือไม่
    public GameObject itemToPickup;      // อ้างอิงถึงไอเทมที่สามารถเก็บได้
    public float pickupRange = 2f;       // ระยะทางที่สามารถเก็บไอเทมได้

    private Animator animator;           // อ้างอิงถึง Animator
    private CharacterController characterController; // อ้างอิงถึง CharacterController

    GameController gameController;       // อ้างอิงถึง GameController

    void Awake()
    {
        // เริ่มต้นคอมโพเนนต์
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // กำหนดค่า time scale ในระบบ
        Time.timeScale = 1;
        isGrounded = characterController.isGrounded;

        if (gameController == null)
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }
    }

    void Update()
    {
        // ตรวจสอบการชนพื้น
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        float h = Input.GetAxis("Horizontal");  // อินพุตแกน X
        float v = Input.GetAxis("Vertical");    // อินพุตแกน Z

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed; // ตรวจสอบว่ากดปุ่มวิ่งหรือไม่

        Vector3 moveDirection = new Vector3(h, 0, v).normalized; // ทิศทางการเคลื่อนที่

        // การเคลื่อนที่
        if (moveDirection.magnitude > 0.1f)
        {
            // การหมุนตามทิศทางการเคลื่อนที่
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerAnim.SetTrigger("Running");
                playerAnim.ResetTrigger("Walking");
            }
            else
            {
                playerAnim.SetTrigger("Walking");
                playerAnim.ResetTrigger("Running");
            }
            playerAnim.ResetTrigger("Idle");

            // ตั้งค่า velocity ของ Rigidbody เพื่อไม่ให้ลอย
            if (isGrounded)
            {
                playerRigid.velocity = new Vector3(moveDirection.x * currentSpeed, 0, moveDirection.z * currentSpeed);
            }
            else
            {
                // ปล่อยให้ Rigidbody มีแรงในแนว Y ตามฟิสิกส์ปกติ
                playerRigid.velocity = new Vector3(moveDirection.x * currentSpeed, playerRigid.velocity.y, moveDirection.z * currentSpeed);
            }
        }
        else
        {
            playerAnim.ResetTrigger("Walking");
            playerAnim.ResetTrigger("Running");
            playerAnim.SetTrigger("Idle");
            if (isGrounded)
            {
                playerRigid.velocity = new Vector3(0, 0, 0);
            }
        }

        // การกดปุ่ม E เพื่อหยิบไอเทมหลังจากเดินชน
        if (Input.GetKeyDown(KeyCode.E) && canPickup)
        {
            // ตรวจสอบว่าผู้เล่นอยู่ใกล้ไอเทมหรือไม่ก่อนที่จะหยิบ
            if (Vector3.Distance(transform.position, itemToPickup.transform.position) <= pickupRange)
            {
                PickupItem();
                gameController.GetItem(); // เรียกการเพิ่มคะแนนจาก GameController
            }
        }
    }

    void PickupItem()
    {
        if (itemToPickup != null)
        {
            PlayPickupAnimation(); // เล่นอนิเมชันหยิบไอเทม
            Invoke("DestroyItem", 1.0f); // ทำลายไอเทมหลังจากอนิเมชันหยิบไอเทมเล่นเสร็จ
            canPickup = false; // หยิบไอเทมเสร็จแล้ว
        }
    }

    void DestroyItem()
    {
        Destroy(itemToPickup); // ทำลายไอเทม
        Debug.Log("Item picked up and Score up");
    }

    void PlayPickupAnimation()
    {
        playerAnim.SetTrigger("Taking1"); // เล่นอนิเมชันหยิบไอเทม
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("item")) 
        {
            canPickup = true;  // สามารถเก็บไอเทมได้เมื่อเดินชนไอเทม
            itemToPickup = other.gameObject;
        }
        else if (other.gameObject.CompareTag("Obstacle")) // ตรวจสอบการชนกับอุปสรรค
        {
            // ทำให้ไม่ลอยเมื่อโดนชนกับอุปสรรค
            playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("item"))
        {
            canPickup = false; // ออกจากระยะไอเทมจะไม่สามารถเก็บได้
            itemToPickup = null;
        }
    }
}
