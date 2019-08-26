using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public GameObject[] messageReceivers;
    public string messageName;
    public Animator animator;
    public float delay = 0.0f;

    public bool state = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    IEnumerator SendMessagesCoroutine()
    {
        yield return new WaitForSeconds(delay);

        foreach (var go in messageReceivers)
        {
            go.SendMessage(messageName, state);
        }
    }

    void OnInteract()
    {
        state = !state;
        if (animator != null)
        {
            animator.SetBool("State", state);
        }
        StartCoroutine(SendMessagesCoroutine());
    }
}
