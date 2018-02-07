using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float velocity;
    public Transform cam;
    public Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector2 inputL = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        float yangle = cam.rotation.eulerAngles.x + inputL.y;
        
        cam.rotation = Quaternion.Euler(yangle, cam.rotation.eulerAngles.y + inputL.x, 0);


        Vector2 inputV = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float force = velocity * inputV.magnitude;
        Vector3 direction = inputV.y * cam.forward + inputV.x * cam.right;

        rb.AddForce(direction * force);
    }
}
