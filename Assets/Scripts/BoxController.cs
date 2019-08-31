using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public List<AudioClip> PushSounds;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var rb = GetComponent<Rigidbody2D>();
        var AS = GetComponent<AudioSource>();

        if (rb.velocity.magnitude > 0 && !AS.isPlaying)
        {
            DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(AS, PushSounds[Random.Range(0, PushSounds.Count)]);
        }
        if (rb.velocity.magnitude < 0.005 && AS.isPlaying)
        {
            AS.Stop();
        }
    }
}
