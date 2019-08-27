using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateBroadcaster))]
public class LeverController : MonoBehaviour
{
    public Animator animator;
    public float delay = 0.0f;

    public bool state = false;
    private StateBroadcaster stateBroadcaster;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        stateBroadcaster = GetComponent<StateBroadcaster>();

        animator.SetBool("State", state);
        stateBroadcaster.BroadcastState(state, delay);
    }

    void OnInteract()
    {
        state = !state;

        animator.SetBool("State", state);
        stateBroadcaster.BroadcastState(state, delay);
    }
}
