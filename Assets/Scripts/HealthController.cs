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

    public bool isFlashingEnabled = true;
    public SpriteRenderer flashingSprite;
    public float flashingTime = 1.25f;
    public float flashingInterval = 0.25f;
    private float flashingStartTime = 0.0f;
    private bool isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;

        DigitalRuby.SoundManagerNamespace.SoundManager.MaxDuplicateAudioClips = 1;

        flashingStartTime = -flashingTime;
        if (isFlashingEnabled && flashingSprite == null)
        {
            flashingSprite = GetComponent<SpriteRenderer>();
        }
    }

    void StartFlashing()
    {
        if (flashingSprite == null)
        {
            return;
        }

        isFlashing = true;
        flashingStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (isFlashing)
        {
            if (Time.time - flashingStartTime > flashingTime)
            {
                isFlashing = false;
                flashingSprite.enabled = true;
            }
            else
            {
                flashingSprite.enabled = Mathf.Repeat(Time.time, flashingInterval) > flashingInterval * 0.5f;
            }
        }

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

        StartFlashing();

        SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);
        DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), HitSound);

        coolDownStart = Time.realtimeSinceStartup;
        coolingDown = true;
    }
}
