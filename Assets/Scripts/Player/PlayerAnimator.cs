using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]

    public Animator anim;

    public PlayerController pc;

    private void Awake()
    {
        if (pc == null)
        {
            gameObject.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        MoveAnim();
    }

    void MoveAnim()
    {
        if (pc.MovementDirection != Vector2.zero)
        {
            anim.SetFloat("Horizontal", pc.MovementDirection.normalized.x);
            anim.SetFloat("Vertical", pc.MovementDirection.normalized.y);
        }

        anim.SetFloat("Speed", pc.MovementSpeed);
        anim.SetBool("Dashing", pc.IsDashing);
    }

    void Die()
    {
        anim.SetTrigger("OnDeath");
    }

    void OnDamage()
    {
        anim.SetTrigger("OnHit");
    }
}
