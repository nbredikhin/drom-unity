﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private EnemyController goblinController;
    public Animator spriteAnimator;

    void Start()
    {
        goblinController = GetComponent<EnemyController>();
    }

    void Update()
    {
        var direction = goblinController.lookDirection;
        spriteAnimator.SetBool("IsWalking", goblinController.isWalking);
        spriteAnimator.SetFloat("Horizontal", direction.x);
        spriteAnimator.SetFloat("Vertical", direction.y);
    }

    void Attack()
    {
        spriteAnimator.SetTrigger("OnAttack");
    }

    void Die()
    {
        spriteAnimator.SetTrigger("OnDeath");
    }

    void OnDamage()
    {
        spriteAnimator.SetTrigger("OnHit");
    }
}
