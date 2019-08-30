using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapMode
{
    Closed,
    OpenOnCollision,
    OpenOnDelay
}

public class TrapFloor : MonoBehaviour
{
    public TrapMode trapMode;
    public float openDelay;
    public float damage = 10000.0f;
    public bool randomizeStartTime = false;

    private bool isTrapOpened;
    private Animator animator;
    // Objects currently being within trap
    private HashSet<Collider2D> trapColliders;
    // Time when trap was last opened
    private float trapOpenedTime = 0.0f;

    public AudioClip TrapSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        trapColliders = new HashSet<Collider2D>();
        if (randomizeStartTime)
        {
            trapOpenedTime = Time.time + Random.Range(0.0f, openDelay);
        }
        else
        {
            trapOpenedTime = -openDelay/2.0f;
        }
    }

    void Update()
    {
        if (trapMode == TrapMode.OpenOnDelay)
        {
            if (Time.time - trapOpenedTime > openDelay)
            {
                trapOpenedTime = Time.time;
                OpenTrap();
            }
        }
    }

    private IEnumerator OpenCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Open Trap");
        DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), TrapSound);
    }

    public void OpenTrap()
    {
        StartCoroutine(OpenCoroutine(trapMode == TrapMode.OpenOnDelay ? 0.0f : openDelay));
    }

    public void OnTrapOpen()
    {
        isTrapOpened = true;

        trapColliders.RemoveWhere(collider =>
        {
            if (collider != null)
                collider.gameObject.SendMessage("DecreaseHealth", damage);
            return true;
        });
    }

    public void OnTrapClose()
    {
        isTrapOpened = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        trapColliders.Add(collider);

        if (isTrapOpened)
        {
            collider.gameObject.SendMessage("DecreaseHealth", damage);
        }
        else
        {
            if (trapMode == TrapMode.OpenOnCollision)
            {
                OpenTrap();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        trapColliders.Remove(collider);
        trapColliders.RemoveWhere(c => c == null);
    }
}
