using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    float reloadTimestamp;
    public float ReloadTimeSec = 1.0f;
    float coolDownTimestamp;
    public float CoolDownTimeSec = 1.0f;

    public int MaxAmmo = 1;
    int currentAmmo;

    enum WeaponState { Idle, Reloading, Shooting, CoolDown };
    WeaponState currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = WeaponState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) {
            case WeaponState.Idle:
                // Do nothing
            break;

            // Reload to change all the ammo
            case WeaponState.Reloading:
                if (isReloadComplete())
                {
                    currentAmmo = MaxAmmo;

                    ChangeState(WeaponState.Idle);
                    break;
                }
            break;

            // Short cool down between attacks
            case WeaponState.CoolDown:
                if (isCoolDownComplete())
                {
                    ChangeState(WeaponState.Idle);
                    break;
                }
            break;

            case WeaponState.Shooting:
                Debug.Log("Attacking!");

                --currentAmmo;
                if (currentAmmo <= 0)
                {
                    ChangeState(WeaponState.Reloading);
                }
                else
                {
                    ChangeState(WeaponState.CoolDown);
                }
            break;
        }
    }

    private bool isCoolDownComplete()
    {
        return (Time.realtimeSinceStartup - coolDownTimestamp >= CoolDownTimeSec);
    }

    private bool isReloadComplete()
    {
        return (Time.realtimeSinceStartup - reloadTimestamp >= ReloadTimeSec);
    }

    bool Attack(Vector2 direction)
    {
        return ChangeState(WeaponState.Shooting);
    }

    bool ChangeState(WeaponState newState)
    {
        if (newState == WeaponState.Reloading)
        {
            reloadTimestamp = Time.realtimeSinceStartup;
            currentState = newState;
            return true;
        }

        if (newState == WeaponState.CoolDown)
        {
            coolDownTimestamp = Time.realtimeSinceStartup;
            currentState = newState;
            return true;
        }

        if (newState == WeaponState.Shooting)
        {
            if (currentState != WeaponState.Idle)
            {
                return false;
            }
            currentState = newState;
            return true;
        }

        currentState = newState;

        return true;
    }
}
