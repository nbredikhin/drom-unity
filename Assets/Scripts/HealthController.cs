using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float MaxHealth = 100.0f;
    public float HealthCoolDownSec = 0.5f;

    bool coolingDown;
    public bool damageBlocked = false;
    float coolDownStart;

    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
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
            return;
        }

        SendMessage("OnDamage", SendMessageOptions.DontRequireReceiver);

        coolDownStart = Time.realtimeSinceStartup;
        coolingDown = true;
    }
}
