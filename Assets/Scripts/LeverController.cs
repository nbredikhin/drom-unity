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

    public AudioClip LeverSound;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        stateBroadcaster = GetComponent<StateBroadcaster>();

        animator.SetBool("State", state);
        animator.speed = 100;
        stateBroadcaster.BroadcastState(state, delay);
    }

    void OnInteract()
    {
        state = !state;

        animator.speed = 1;
        animator.SetBool("State", state);
        stateBroadcaster.BroadcastState(state, delay);

        DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), LeverSound);
    }
}
