using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DigitalRuby;

public class HealthController : MonoBehaviour
{
    public float MaxHealth = 100.0f;
    public float HealthCoolDownSec = 0.5f;

    bool coolingDown;
    public bool damageBlocked = false;
    float coolDownStart;

    public float health;

    public AudioClip DeathSound;
    public AudioClip HitSound;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;

        DigitalRuby.SoundManagerNamespace.SoundManager.MaxDuplicateAudioClips = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!coolingDown)
        {
            return;
        }
        if (Time.realtimeSinceStartup - coolDownStart >= HealthCoolDownSec)
        {
            coolingDown = false;
        }
    }

    void DecreaseHealth(float value)
    {
        if (coolingDown || health <= 0 || (damageBlocked && value < MaxHealth))
        {
            return;
        }

        health -= value;
        Debug.Log("Got hit, now have " + health + " HP");
        if (health <= 0)
        {
            health = 0;
            SendMessage("Die");

            DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), DeathSound);

            return;
        }

        SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
        DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), HitSound);

        coolDownStart = Time.realtimeSinceStartup;
        coolingDown = true;
    }
}
