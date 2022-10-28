using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    CharacterController controller;
    public Transform fpsCamera;

    public float sensitivity = 200f;
    public float speed = 15f;

    float xRotation = 0f;

    bool isGrounded;
    public Transform groundSensor;
    public float sensorRadius;
    public LayerMask ground;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //variables para almacenar el movimiento del raton en cada eje
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Le vamos restando el valor del movimiento del raton en el eje Y
        xRotation -= mouseY;
        //Limitamos la rotacion de la camara entre -90 y 90 
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        //Rotamos la camara en el eje X (mirar arriba y abajo)
        fpsCamera.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Rotamos el personaje en el eje Y para que pueda girar sobre si mismo
        transform.Rotate(Vector3.up * mouseX);

        //Almacenamos los Inputs de movimiento
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //Creamos un vector donde multiplicamos el transform rigth por el input de movimiento lateral
        //y el transform forward por el movimiento hacia delante para que cuando movamos el personaje
        //se mueva en la direccion correcta
        Vector3 move = transform.right * x + transform.forward * z;

        //Funcion del character controller a la que le pasamos el Vector que hemos creado y lo multiplicamos por la velocidad para movernos
        controller.Move(move.normalized * speed * Time.deltaTime);

        Jump();
    }

    void Jump()
    {
        isGrounded = Physics.CheckSphere(groundSensor.position, sensorRadius, ground);
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
