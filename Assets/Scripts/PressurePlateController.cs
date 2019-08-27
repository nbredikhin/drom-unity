using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateBroadcaster))]
public class PressurePlateController : MonoBehaviour
{
    private HashSet<Collider2D> plateColliders;
    public Animator plateAnimator;

    private StateBroadcaster stateBroadcaster;

    void Start()
    {
        plateColliders = new HashSet<Collider2D>();
        if (plateAnimator == null)
        {
            plateAnimator = GetComponent<Animator>();
        }

        stateBroadcaster = GetComponent<StateBroadcaster>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        plateColliders.Add(collider);

        // If first object enter plate
        if (plateColliders.Count == 1)
        {
            plateAnimator.SetBool("Pressed", true);
            stateBroadcaster.BroadcastState(true, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        plateColliders.Remove(collider);
        plateColliders.RemoveWhere(c => c == null);

        // If last object exit plate
        if (plateColliders.Count == 0)
        {
            plateAnimator.SetBool("Pressed", false);
            stateBroadcaster.BroadcastState(false, 0.25f);
        }
    }
}
