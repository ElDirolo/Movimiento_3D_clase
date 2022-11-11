using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    public Transform cam;
    public Transform LookAtTransform;

    //variables para controlar velocidad, altura de salto y gravedad
    public float speed = 5;
    public float jumpHeight = 1;
    public float gravity = -9.81f;
    [SerializeField]private float pushStrength;

    //variables para el ground sensor
    public bool isGrounded;
    public Transform groundSensor;
    public float sensorRadius = 0.1f;
    public LayerMask ground;
    private Vector3 playerVelocity;

    //variables para rotacion del personaje
    private float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    //variables para el movimiento del raton con virtual camera
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    public GameObject[] cameras;

    //Variables para coger objectos
    public GameObject objectToPick;
    [SerializeField]private GameObject pickedObject;
    [SerializeField]Transform interactionZone;
    // Start is called before the first frame update
    void Start()
    {
        //Asignamos el character controller a su variable
        controller = GetComponent<CharacterController>();

        anim = GetComponentInChildren<Animator>();

        //Con esto podemos esconder el icono del raton para que no moleste
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovementTPS2();
        Jump();
        PickObjects();
    }
#region FuncionMovimiento
    void MovementTPS2()
    {
        float z = Input.GetAxisRaw("Vertical");
        anim.SetFloat("VelZ", z);
        float x = Input.GetAxisRaw("Horizontal");
        anim.SetFloat("VelX", x);
        
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, xAxis.Value, 0);
        LookAtTransform.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, LookAtTransform.eulerAngles.z);
        
        
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if(Input.GetButton("Fire2"))
        {
            cameras[0].SetActive(false);
            cameras[1].SetActive(true);
        }
        else
        {
            cameras[0].SetActive(true);
            cameras[1].SetActive(false);
        }


        if(move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }
#endregion
#region FuncionDeSalto
    //Funcion de salto y gravedad
    void Jump()
    {
        //Le asignamos a la boleana isGrounded su valor dependiendo del CheckSpher
        //CheckSphere crea una esfera pasandole la poscion, radio y layer con la que queremos que interactue
        //si la esfera entra en contacto con la capa que le digamos convertira nuestra boleana en true y si no entra en contacto en false
        isGrounded = Physics.CheckSphere(groundSensor.position, sensorRadius, ground);

        anim.SetBool("Jump", !isGrounded);

        //Si estamos en el suelo y playervelocity es menor que 0 hacemos que le vuelva a poner el valor a 0
        //esto es para evitar que siga aplicando fuerza de gravedad cuando estemos en el suelo y evitar comportamientos extraños
        if(isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        //si estamos en el suelo y pulasamos el imput de salto hacemos que salte el personaje
        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            //Formula para hacer que los saltos sean de una altura concreta
            //la altura depende del valor de jumpHeight 
            //Si jumpHeigt es 1 saltara 1 metro de alto
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity); 
        }

        //a playervelocity.y le iremos sumando el valor de la gravedad
        playerVelocity.y += gravity * Time.deltaTime;
        //como playervelocity en el eje Y es un valor negativo esto nos empuja al personaje hacia abajo
        //asi le aplicaremos la gravedad
        controller.Move(playerVelocity * Time.deltaTime);
    }
#endregion
#region FuncionCoger
    void PickObjects()
    {
        if(Input.GetKey(KeyCode.E))
        {
            if(objectToPick != null && pickedObject == null && objectToPick.gameObject.GetComponent<PickableObject>().isPickable == true) 
            {
                pickedObject = objectToPick;
                pickedObject.GetComponent<PickableObject>().isPickable = false;
                pickedObject.transform.SetParent(interactionZone);
                pickedObject.transform.position = interactionZone.position;
                pickedObject.GetComponent<Rigidbody>().useGravity = false;
                pickedObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        else if(pickedObject != null)
        {
            pickedObject.GetComponent<PickableObject>().isPickable = true;
            pickedObject.transform.SetParent(null);
            pickedObject.GetComponent<Rigidbody>().useGravity = true;
            pickedObject.GetComponent<Rigidbody>().isKinematic = false;
            pickedObject = null;
        }
    }


    private void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if(hit.gameObject.tag == "empujable")
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if(body == null || body.isKinematic)
            {
                return;
            }

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

            body.velocity = pushDir * pushStrength / body.mass;
        }
    }
#endregion
}
