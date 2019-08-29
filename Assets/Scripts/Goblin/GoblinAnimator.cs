using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimator : MonoBehaviour
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
}
