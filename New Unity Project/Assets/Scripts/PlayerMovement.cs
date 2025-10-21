using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    private Animator movementAnimator;
    public float speed = 5f;
    public int theScore;
    
    public override void OnNetworkSpawn()
    {
        movementAnimator = this.GetComponent<Animator>();
        theScore = 0;
    }

    void OnCollisionEnter2D(Collision2D target)
    {
        if(!IsSpawned) return;
        if(target.gameObject.tag.Equals("Ground") == true)
        {
            movementAnimator.SetBool("isJump", false);
        }

        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        return;

        if(target.gameObject.tag.Equals("ScoreItem") == true)
        {
            if(NetworkManager.Singleton.LocalClientId == OwnerClientId)
            {
                scoreCollectedServerRpc(OwnerClientId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void scoreCollectedServerRpc(ulong clientId)
    {
        //request players to send all their scores
        scoreCollectedClientRpc(clientId);
    }

    [ClientRpc]
    private void scoreCollectedClientRpc(ulong targetClientId)
    {
        //get the TargetClientID, compare it to the owner id and if the same - update the score
        if (targetClientId == OwnerClientId)
        {
            GameObject theClientObject = this.gameObject;

            NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent
            <PlayerMovement>().theScore =
            NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent
            <PlayerMovement>().theScore + 1;
        }
        Debug.Log("the score of player " + targetClientId + " is " +
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject.GetComponent
        <PlayerMovement>().theScore);
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                HandleMovementServerRpc(1,this.NetworkObjectId);
                // transform.position += new Vector3(speed * Time.deltaTime, 0f,0f);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                HandleMovementServerRpc(2, this.NetworkObjectId);
                // transform.position -= new Vector3(speed * Time.deltaTime, 0f,0f);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                HandleMovementServerRpc(3, this.NetworkObjectId);
                // transform.position += new Vector3(0f, speed * Time.deltaTime,0f);
                // movementAnimator.SetBool("isJump", true);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                HandleMovementServerRpc(4, this.NetworkObjectId);
                // transform.position -= new Vector3(0f, speed * Time.deltaTime,0f);
            }
            else { HandleMovementServerRpc(0, this.NetworkObjectId); }
        }
        //Debug.Log(transform.position);
    }

    [ServerRpc]
    void HandleMovementServerRpc(int movementDirection, ulong playerID)
    {
        // Debug.Log("the player " + playerID + " just moves from position " +
        // NetworkManager.Singleton.ConnectedClients[0].PlayerObject.transform.position);
        HandleMovementClientRpc(movementDirection);
    }

    [ClientRpc]
    void HandleMovementClientRpc(int movementDirection)
    {
        switch (movementDirection)
        {
        case 1:
            transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isRun", true);
            break;
        case 2:
            transform.position -= new Vector3(speed * Time.deltaTime, 0f, 0f);
            movementAnimator.SetBool("isRun", true);
            break;
        case 3:
            transform.position += new Vector3(0f, speed * Time.deltaTime, 0f);
            movementAnimator.SetBool("isJump", true);
            break;
        case 4:
            transform.position -= new Vector3(0f, speed * Time.deltaTime, 0f);
            break;
        case 0:
        default:
            movementAnimator.SetBool("isRun", false);
            break;
        }
    }
}
