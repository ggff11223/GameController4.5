
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float speed = 10.0f;        
    public float jumpForce = 8.0f;       
    public float gravity = 20.0f;         
    public float rotationSpeed = 100.0f;  

    
    public bool isGrounded = false;       
    public bool isDef = false;            
    public bool isDancing = false;         
    public bool isWalking = false;         
    
    public bool isTaking = false;       
    
    private Animator animator;             
    private CharacterController characterController; 
    private Vector3 inputVector = Vector3.zero;  
    private Vector3 targetDirection = Vector3.zero; 
    private Vector3 moveDirection = Vector3.zero; 
    private Vector3 velocity = Vector3.zero;       
    
    GameController gameController; 
    
    void Awake()
    {
        // เริ่มต้นคอมโพเนนต์
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }                              
    void Start()
    {
        
        Time.timeScale = 1;
        isGrounded = characterController.isGrounded;
       
        if (gameController == null)
        {
            gameController = GameObject.Find("GameController").GetComponent<GameController>();
        }
        
    }

  
    void Update()
    {
        float z = Input.GetAxis("Horizontal");
        float x = -(Input.GetAxis("Vertical"));
        Debug.Log("z:" + z);
        Debug.Log("x:" + x);
        animator.SetFloat("inputX", x);
        animator.SetFloat("inputZ", z);

        if (z != 0 || x != 0)
        {
            isWalking = true;
            animator.SetBool("isWalking", isWalking);
            Debug.Log("isWalking:" + isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
            Debug.Log("isWalking:" + isWalking);
        }

        isGrounded = characterController.isGrounded;
        if (isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection *= speed;
        }
        characterController.Move(moveDirection * Time.deltaTime);
        inputVector = new Vector3(x, 0.0f, z);
        UpdateMovement();

      
        if (isTaking)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                animator.SetBool("isTaking", isTaking);
                Debug.Log("isTaking:" + isTaking);
                StartCoroutine(WaitforTaking(4.7f));
                // getItem = true;
                gameController.GetItem();
                Debug.Log("Item:" + gameController.getItem);
            }
        }
       
    }

    IEnumerator WaitforTaking(float time)
    {
        yield return new WaitForSeconds(time);
        isTaking = false;
        animator.SetBool("isTaking", isTaking);
        Debug.Log("isTaking:" + isTaking);
    }
    void UpdateMovement()
    {
        Vector3 motion = inputVector;
        motion = ((Mathf.Abs(motion.x) > 1) || (Mathf.Abs(motion.z) > 1)) ? motion.normalized : motion;
        RotationMovement();
        ViewRelativeMovement();
    }
    void RotationMovement()
    {
        if (inputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    void ViewRelativeMovement()
    {
       
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        targetDirection = (Input.GetAxis("Horizontal") * right) + (Input.GetAxis("Vertical") * forward);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    //------------------------------------------------------------
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            isTaking = true;
            Debug.Log("isTaking:" + isTaking);
        }
    }
    //------------------------------------------------------------
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            isTaking = false;
            Debug.Log("isTaking:" + isTaking);
        }
    }
    //------------------------------------------------------------
}