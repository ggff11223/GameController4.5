using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W Key Pressed");
            animator.SetTrigger("Idle ");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A Key Pressed");
            animator.SetTrigger("Action Idle To Standing Idle ");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S Key Pressed");
            animator.SetTrigger("Walking ");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D Key Pressed");
            animator.SetTrigger("Wave Hip Hop Dance");
        }
    }
}

