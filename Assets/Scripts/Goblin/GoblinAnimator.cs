using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAnimator : MonoBehaviour
{
    private GoblinController goblinController;
    public Animator spriteAnimator;

    void Start()
    {
        goblinController = GetComponent<GoblinController>();
    }

    // Update is called once per frame
    void Update()
    {
        var velocity = goblinController.MovementVelocity;
        var speed = velocity.magnitude;
        spriteAnimator.SetFloat("Speed", speed);
        if (speed > 0.1)
        {
            spriteAnimator.SetFloat("Horizontal", velocity.x);
            spriteAnimator.SetFloat("Vertical", velocity.y);
        }
    }
}
