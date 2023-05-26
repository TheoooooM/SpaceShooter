using FishNet.Object;
using UnityEngine;

public class Controller : NetworkBehaviour
{
    private PlayerInputs _inputs;

    [SerializeField]private float speed = 5;
    private Vector2 _lastDir;

    private bool _scaleUp;
    private float _latency;
    
    private void Awake()
    {
        _inputs = new PlayerInputs();

        _inputs.Movement.Move.performed += ctx => _lastDir = ctx.ReadValue<Vector2>();
        _inputs.Movement.Move.canceled  += _ => _lastDir = Vector2.zero;
        
        _inputs.Enable();
    }

    private void Update()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.C))
        {
            _latency = Time.time;
            ScaleMasterRPC();
        }
    }

    void Move()
    {
        if(!IsOwner)return;
        var dir3 = new Vector3(_lastDir.x, 0, _lastDir.y);
        transform.position += dir3 * (speed * Time.deltaTime);
    }

    [ServerRpc]
    void ScaleMasterRPC()=>ScaleRPC();
    

    [ObserversRpc]
    void ScaleRPC()
    {
        _latency = Time.time - _latency;
        Debug.Log($"Latency: {_latency*100} ms");
        if (!_scaleUp)
        {
            transform.localScale *= 2;
            _scaleUp = true;
        }
        else
        {
            transform.localScale *= .5f;
            _scaleUp = false;
        }
    }
    
}
