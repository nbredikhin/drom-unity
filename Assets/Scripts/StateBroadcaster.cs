using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBroadcaster : MonoBehaviour
{
    public GameObject[] messageReceivers;
    public string messageName;

    public void BroadcastState(bool state)
    {
        if (messageReceivers == null || messageReceivers.Length == 0)
            return;

        foreach (var go in messageReceivers)
        {
            go.SendMessage(messageName, state);
        }
    }

    public void BroadcastState(bool state, float delay)
    {
        StartCoroutine(DelayCoroutine(state, delay));
    }

    private IEnumerator DelayCoroutine(bool state, float delay)
    {
        yield return new WaitForSeconds(delay);
        BroadcastState(state);
    }
}
