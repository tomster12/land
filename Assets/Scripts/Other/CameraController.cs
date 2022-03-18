
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    // Declare preset, config, variables
    [SerializeField] private Transform orbitCentre;

    [SerializeField] private float movementFriction = 0.9f;
    [SerializeField] private float rotationFriction = 0.9f;
    [SerializeField] private float distanceFriction = 0.9f;

    private Vector3 moveVel;
    private Vector2 rot = new Vector2(0.45f, 4.7f), rotVel;
    private float dist = 10.0f, distVel;


    public void Update()
    {
        // Move with WASD, pan with MMB
        Vector3 dirHorz = transform.right;
        Vector3 dirForw = Quaternion.Euler(0, -90, 0) * transform.right;
        Vector3 dirVert = transform.up;
        moveVel += dirHorz * Input.GetAxisRaw("Horizontal") * 0.005f;
        moveVel += dirForw * Input.GetAxisRaw("Vertical") * 0.005f;
        if (Input.GetMouseButton(2))
        {
            moveVel += dirHorz * -Input.GetAxisRaw("Mouse X") * 0.008f;
            moveVel += dirVert * -Input.GetAxisRaw("Mouse Y") * 0.008f;
        }
        orbitCentre.position += moveVel;
        moveVel *= movementFriction;


        // Zoom with mouse wheel
        distVel += Input.GetAxis("Mouse ScrollWheel") * -0.75f;
        dist += distVel;
        dist = Mathf.Clamp(dist, 1.0f, 20.0f);
        distVel *= distanceFriction;


        // Rotate orbit with RMB
        if (Input.GetMouseButton(1))
        {
            rotVel.x += Input.GetAxis("Mouse Y") * -0.004f;
            rotVel.y += Input.GetAxis("Mouse X") * -0.004f;
        }
        rot += rotVel;
        rot.x = Mathf.Clamp(rot.x, -Mathf.PI / 2.0f + 0.01f, Mathf.PI / 2.0f - 0.01f);
        rot.y = (rot.y + Mathf.PI * 2) % (Mathf.PI * 2);
        rotVel *= rotationFriction;


        // Set cameras position based on orbit parameters
        transform.position = new Vector3(
          Mathf.Cos(rot.y) * Mathf.Cos(rot.x),
          Mathf.Sin(rot.x),
          Mathf.Sin(rot.y) * Mathf.Cos(rot.x)
        ) * dist + orbitCentre.position;
        transform.LookAt(orbitCentre);
    }
}
