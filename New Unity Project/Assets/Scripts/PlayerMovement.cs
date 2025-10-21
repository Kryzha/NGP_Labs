using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    private Animator movementAnimator;
    public float speed = 5f;
    
    public override void OnNetworkSpawn()
    {
        movementAnimator = this.GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if(target.gameObject.tag.Equals("Ground") == true)
        {
            movementAnimator.SetBool("isJump", false);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isJump", true);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
        }
        Debug.Log(transform.position);
    }
}
