using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    Animator animator;
    public bool facingUp;
    public bool facingRight;
    public bool moving;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("FacingUp", facingUp);
        animator.SetBool("FacingRight", facingRight);
        animator.SetBool("IsMoving", moving);
    }
}
