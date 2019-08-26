using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public GameObject exitGameObject;
    public GameObject nextCamera;

    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            doorAnimator.SetBool("IsOpened", !doorAnimator.GetBool("IsOpened"));
            Debug.Log(doorAnimator.GetBool("IsOpened"));
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (exitGameObject && collider.gameObject.tag == "Player")
        {
            collider.gameObject.transform.position = exitGameObject.transform.position;
            foreach (var camera in GameObject.FindGameObjectsWithTag("Camera"))
            {
                if (camera != nextCamera)
                {
                    camera.SetActive(false);
                }
            }
            nextCamera.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        if (exitGameObject != null)
            Debug.DrawLine(this.transform.position, exitGameObject.transform.position, Color.blue);
    }
}
