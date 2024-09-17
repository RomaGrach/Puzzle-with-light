using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject target;
    private GameObject _center;
    public float speed = 5;
    private float ROTATION_SPEED = 0.3f;

    float minFov = 35f;
    float maxFov = 100f;
    float sensitivity = 17f;

    private DefaultInputAction _inputs;
    private Vector2 prevMousePosition = Vector2.zero;
    void Start()
    {
        _center = GameObject.Find("Center");
        transform.LookAt(_center.transform);
    }

    void Update()
    {
        Vector2 mousePosition = _inputs.UI.Point.ReadValue<Vector2>();

        if (prevMousePosition.Equals(Vector2.zero))
        {
            prevMousePosition = mousePosition;
        }
        //transform.Rotate(0, speed * Time.deltaTime, 0); 

        if (_inputs.UI.RightClick.IsPressed())
        {
            float camAngleDelta = ROTATION_SPEED; //* Time.deltaTime;
            float camAngleDeltaAxisY = camAngleDelta * (mousePosition.x - prevMousePosition.x);
            //float camAngleDeltaAxisX = camAngleDelta * (prevMousePosition.y - mousePosition.y);
            transform.RotateAround(_center.transform.position, Vector3.up, camAngleDeltaAxisY);
        }

        //Zoom
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * -sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;

        prevMousePosition = mousePosition;
    }

    void OnEnable()
    {
        if (_inputs == null)
        {
            _inputs = new DefaultInputAction();
        }
        _inputs.Player.Enable();
        _inputs.UI.Enable();
    }
    void OnDisable()
    {
        _inputs.Player.Disable();
        _inputs.UI.Disable();
    }
}
