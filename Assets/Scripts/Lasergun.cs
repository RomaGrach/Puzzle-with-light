using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lasergun : MonoBehaviour
{
    private GameObject trigger;
    void Start()
    {
        Lasermanager.instance.AddLaser(this);
        trigger = GameObject.Find("Trigger");
    }
    //void FixedUpdate()
    //{
    //    transform.RotateAround(transform.position, Vector3.up, -20f * Time.deltaTime);
    //}

    void Update()
    {

        //if (Input.GetMouseButtonDown(0))
        //{
            RaycastHit hit;

            int layerMask = 1 << 8;
            layerMask = ~layerMask;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("Lasergun.update:  hit.transform.gameobject=" + hit.transform.gameObject);
                if (hit.transform.gameObject == trigger)
                {
                    Debug.Log("Lasergun.update:  if destroy");
                    Destroy(trigger);
                }
            }
        //}
    }
}
