using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    public GameObject[] thePlayers;

    private Animator movementAnimator;
    public Transform spawnedObjectTransform;
    public float speed = 5f;
    public int theScore;

    public NetworkVariable<int> networkPlayerScore = new NetworkVariable<int>(0);
    public int clientScore;

    public TextMesh playerScoreDisplay;

    private NetworkVariable<int> theScoreForEachPlayer = new NetworkVariable<int>(0,
    NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    
    public override void OnNetworkSpawn()
    {
        movementAnimator = this.GetComponent<Animator>();
        theScore = 0;
    }

    void OnGUI()
    {
        thePlayers = GameObject.FindGameObjectsWithTag("Player");
        int x = 0;
        foreach (GameObject player in thePlayers)
        {
            GUI.Label(new Rect(10, 60 + (15 * x), 300, 20), "PlayerID " +
            player.GetComponent<NetworkObject>().NetworkObjectId + " has the score of " +
            player.GetComponent<PlayerMovement>().theScoreForEachPlayer.Value);
            x++;

        }
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
                UpdateScoreServerRpc(1, OwnerClientId);
            }
        }

        if(target.gameObject.tag.Equals("Pickup") == true)
        {
            if(!IsOwner) return;
            theScoreForEachPlayer.Value = theScoreForEachPlayer.Value + 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScoreServerRpc(int addValue, ulong clientID)
    {
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].
        PlayerObject.GetComponent<PlayerMovement>().networkPlayerScore.Value =
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].
        PlayerObject.GetComponent<PlayerMovement>().networkPlayerScore.Value + addValue;

        NotifyScoreClientRpc(clientID,
        NetworkManager.Singleton.ConnectedClients[OwnerClientId].
        PlayerObject.GetComponent<PlayerMovement>().networkPlayerScore.Value);

    }

    [ClientRpc]
    private void NotifyScoreClientRpc(ulong targetClientId, int newValue)
    {
        clientScore = newValue;
        playerScoreDisplay.text = "Player " + (targetClientId).ToString() + ": " + clientScore.ToString();
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
            else if (Input.GetKey(KeyCode.R))
            {
                createBulletShotFromClientServerRpc(transform.position.x,
                transform.position.y, transform.position.z, transform.rotation);
            }
            else { HandleMovementServerRpc(0, this.NetworkObjectId); }
        }
        //Debug.Log(transform.position);
    }

    [ServerRpc]
    private void createBulletShotFromClientServerRpc(float positionX, float positionY, 
    float positionZ, Quaternion vector3Rotation)
    {
        GameObject spawnedObject = Instantiate(spawnedObjectTransform,
        new Vector3(positionX, positionY, positionZ), vector3Rotation).gameObject;
        spawnedObject.GetComponent<NetworkObject>().Spawn(true);
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
