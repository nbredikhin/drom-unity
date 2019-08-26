using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    float reloadTimestamp;
    public float ReloadTimeSec = 1.0f;
    float coolDownTimestamp;
    public float CoolDownTimeSec = 0.5f;

    public int MaxAmmo = 1;
    int currentAmmo;

    bool doneShooting = false;
    Vector2 shootingDirection;

    public GameObject ProjectilePrefab;
    GameObject projectile;

    enum WeaponState { Idle, Reloading, Shooting, CoolDown };
    WeaponState currentState;

    void Start()
    {
        currentAmmo = MaxAmmo;
        currentState = WeaponState.Idle;
    }

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
                if (!doneShooting)
                {
                    break;
                }
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
        return (Time.time - coolDownTimestamp >= CoolDownTimeSec);
    }

    private bool isReloadComplete()
    {
        return (Time.time - reloadTimestamp >= ReloadTimeSec);
    }

    void DoneShooting()
    {
        doneShooting = true;
    }

    bool Attack(Vector2 direction)
    {
        shootingDirection = direction.normalized;
        return ChangeState(WeaponState.Shooting);
    }

    bool ChangeState(WeaponState newState)
    {
        if (newState == WeaponState.Reloading)
        {
            reloadTimestamp = Time.time;
            currentState = newState;
            return true;
        }

        if (newState == WeaponState.CoolDown)
        {
            coolDownTimestamp = Time.time;
            currentState = newState;
            return true;
        }

        if (newState == WeaponState.Shooting)
        {
            if (currentState != WeaponState.Idle)
            {
                return false;
            }

            var q = Quaternion.FromToRotation(transform.up, new Vector3(shootingDirection.x, shootingDirection.y, 0));
            projectile = Instantiate(ProjectilePrefab,
                                     transform.position,
                                     q,
                                     transform);
            projectile.SendMessage("LaunchAttack", shootingDirection);
            --currentAmmo;

            currentState = newState;
            return true;
        }

        currentState = newState;

        return true;
    }
}
