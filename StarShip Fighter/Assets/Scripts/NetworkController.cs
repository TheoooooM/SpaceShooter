using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkController : NetworkBehaviour
{
    private PlayerInputs _inputs;

    Vector2 _lastDir;
        
    [SerializeField]private float speed = 5;
        
    private void Awake()
    {
        _inputs = new();
            
        _inputs.Movement.Move.performed += ctx => MoveOnperformedMasterRPC(ctx.ReadValue<Vector2>()); //ctx => _lastDir = ctx.ReadValue<Vector2>();
        _inputs.Movement.Move.canceled  += _ => MoveOnCancelMasterRPC();
            
        _inputs.Enable();
    }

    
    [ServerRpc] private void MoveOnperformedMasterRPC(Vector2 ctx) => MoveOnperformedRPC(ctx);
    [ObserversRpc] private void MoveOnperformedRPC(Vector2 ctx)
    {
        _lastDir = ctx;
    }
        
    [ServerRpc] private void MoveOnCancelMasterRPC() => MoveOnCancelRPC();
    [ObserversRpc] private void MoveOnCancelRPC()
    {
        _lastDir = Vector2.zero;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //if(!IsOwner)return;
        var dir3 = new Vector3(_lastDir.x, 0, _lastDir.y);
        transform.position += dir3 * (speed * Time.deltaTime);
    }
}

