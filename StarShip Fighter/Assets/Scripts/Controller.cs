using System;
using FishNet.Object;
using UnityEngine;

public class Controller : NetworkBehaviour
{
    private PlayerInputs _inputs;

    [SerializeField]private float speed = 5;
    private Vector2 lastDir;

    private bool scaleUp;
    
    private void Awake()
    {
        _inputs = new PlayerInputs();

        _inputs.Movement.Move.performed += ctx => lastDir = ctx.ReadValue<Vector2>();
        _inputs.Movement.Move.canceled  += ctx => lastDir = Vector2.zero;
        
        _inputs.Enable();
    }

    private void Update()
    {
        Move();
        if(Input.GetKeyDown(KeyCode.S))ScaleMasterRPC();
    }

    void Move()
    {
        if(!IsOwner)return;
        var dir3 = new Vector3(lastDir.x, 0, lastDir.y);
        transform.position += dir3 * speed * Time.deltaTime;
    }

    [ServerRpc]
    void ScaleMasterRPC()
    {
        ScaleRPC();
    }

    [ObserversRpc]
    void ScaleRPC()
    {
        if (!scaleUp)
        {
            transform.localScale *= 2;
            scaleUp = true;
        }
        else
        {
            transform.localScale *= .5f;
            scaleUp = false;
        }
    }
    
}
