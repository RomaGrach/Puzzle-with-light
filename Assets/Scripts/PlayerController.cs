using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 targetPosition;
    Quaternion targetRotation;
    // Start is called before the first frame update
    void Start()
    {
        SwipeManager.instance.moveElement += Move;
        SwipeManager.instance.rotateEvent += Rotate;
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, 0.05f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1.5f);
    }

    void Move(SwipeManager.MoveDirection direction)
    {
        switch(direction)
        {
            case SwipeManager.MoveDirection.Down:
                targetPosition += Vector3.down;
                break;
            case SwipeManager.MoveDirection.Up:
                targetPosition += Vector3.up;
                break;
            case SwipeManager.MoveDirection.Left:
                targetPosition += Vector3.left;
                break;
            case SwipeManager.MoveDirection.Right:
                targetPosition += Vector3.right;
                break;
        }
    }

    void Rotate(SwipeManager.RotationDirection direction)
    {
        switch(direction)
        {
            case SwipeManager.RotationDirection.Clockwise: 
                targetRotation *= Quaternion.Euler(Vector3.forward * -90); 
                break;
            case SwipeManager.RotationDirection.CounterClockwise:
                targetRotation *= Quaternion.Euler(Vector3.forward * 90);
                break;
        }
    }
}
