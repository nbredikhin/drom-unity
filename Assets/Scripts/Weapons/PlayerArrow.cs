using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    bool isShooting = false;

    public float TravelSpeed = 1.0f;
    public float Damage = 10.0f;
    public float KnockBackForce = 20.0f;

    private GameObject player;
    public AudioClip ArrowSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isShooting)
        {
            return;
        }
        player.transform.position = transform.position;
    }

    void LaunchAttack(Vector2 direction)
    {
        if (isShooting)
        {
            return;
        }

        isShooting = true;
        transform.Rotate(0, 0, 90);
        player = transform.parent.gameObject;
        player.SendMessage("OnArrowFlightStart");
        player.GetComponent<Collider2D>().enabled = false;
        transform.position = transform.parent.position;
        transform.parent = null;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * TravelSpeed;
        DigitalRuby.SoundManagerNamespace.SoundManager.PlayOneShotSound(GetComponent<AudioSource>(), ArrowSound);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name == "Death Tiles") return;
        player.GetComponent<Collider2D>().enabled = true;
        Debug.Log(collider.name);

        collider.SendMessage("DecreaseHealth", Damage, SendMessageOptions.DontRequireReceiver);
        collider.SendMessage("KnockBack",
                             new Vector2(transform.right.x, transform.right.y) * KnockBackForce,
                             SendMessageOptions.DontRequireReceiver);
        player.SendMessage("OnArrowFlightEnd");
        player.SendMessage("DoneShooting");
        Destroy(gameObject);
    }
}
