using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void LateUpdate()
    {
        //Hacemos que la camara siga al personaje
        transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);
        //hacemos que la camara gire con el personaje
        //en el eje X le ponemos 90 para que la camara apunte hacia abajo
        transform.rotation = Quaternion.Euler(90, player.eulerAngles.y, 0);
    }
}
