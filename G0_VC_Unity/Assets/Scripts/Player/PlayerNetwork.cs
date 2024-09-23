using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// An example network serializer with both server and owner authority.
/// Love Tarodev
/// </summary>
public class PlayerNetwork : NetworkBehaviour
{
    /// <summary>
    /// A toggle to test the difference between owner and server auth.
    /// </summary>
    [SerializeField] private bool _serverAuth;
    [SerializeField] private float _cheapInterpolationTime = 0.1f;
    [SerializeField] private GameObject Exterior;
    [SerializeField] private GameObject Viewport;
    [SerializeField] Transform rotatables;
    [SerializeField] PlayerStateMachine playerStateMachine;
    [SerializeField] Player_Inventory inventory;
    private NetworkVariable<PlayerNetworkState> _playerState;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _playerState = new NetworkVariable<PlayerNetworkState>(writePerm: permission);
    }

    private void Update()
    {
        if (IsOwner) TransmitState();
        else ConsumeState();
    }

    #region Transmit State

    public void TransmitState()
    {
        var state = new PlayerNetworkState
        {
            Position = _rb.position,
            Rotation = new Vector2(playerStateMachine.xRotation, playerStateMachine.yRotation),
            isScoping_internal = inventory.isScoping,
            isShooting_internal = inventory.isShooting,
            GrapplePosition = playerStateMachine.grapplePoint,
            currentPlayerState_fl_internal = playerStateMachine.CurrentPlayerState.key,
            currentWeapon_fl_internal = inventory.currentWeapon.key
        };

        if (IsServer || !_serverAuth)
            _playerState.Value = state;
        else
            TransmitStateServerRpc(state);

        
    }

    [ServerRpc]
    private void TransmitStateServerRpc(PlayerNetworkState state)
    {
        _playerState.Value = state;
    }

    #endregion

    #region Interpolate State

    private Vector3 _posVel;
    //private float Exterior_rotVel;

    public void ConsumeState()
    {
        // Here you'll find the cheapest, dirtiest interpolation you'll ever come across. Please do better in your game
        _rb.MovePosition(Vector3.SmoothDamp(_rb.position, _playerState.Value.Position, ref _posVel, _cheapInterpolationTime));

        //Exterior.transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(Exterior.transform.rotation.eulerAngles.y, _playerState.Value.Rotation.y, ref Exterior_rotVel, _cheapInterpolationTime), 0);

        ///Exterior.transform.rotation = Quaternion.RotateTowards(Exterior.transform.rotation, Quaternion.Euler(_playerState.Value.Rotation.x, _playerState.Value.Rotation.y, 0), 3);
        ///Viewport.transform.rotation = Quaternion.RotateTowards(Viewport.transform.rotation, Quaternion.Euler(_playerState.Value.Rotation.x, _playerState.Value.Rotation.y, 0), 3);

        /*Exterior.transform.rotation = Quaternion.Lerp(Exterior.transform.rotation, Quaternion.Euler(_playerState.Value.Rotation.x, _playerState.Value.Rotation.y, 0), 0.3f);
        Viewport.transform.rotation = Quaternion.Lerp(Viewport.transform.rotation, Quaternion.Euler(_playerState.Value.Rotation.x, _playerState.Value.Rotation.y, 0), 0.3f);*/
        //rotatables.transform.rotation = Quaternion.Lerp(rotatables.transform.rotation, Quaternion.Euler(0, _playerState.Value.Rotation.y, 0), 0.3f);
        Exterior.transform.rotation = Quaternion.Lerp(Exterior.transform.rotation, Quaternion.Euler(0, _playerState.Value.Rotation.y, 0), 0.3f);
        playerStateMachine.xRotation = _playerState.Value.Rotation.x;
        playerStateMachine.yRotation = _playerState.Value.Rotation.y;

        //  Debug.Log(_playerState.Value.currentPlayerState_fl_internal);
        if (_playerState.Value.currentPlayerState_fl_internal != 0) playerStateMachine.internal_CurrentState = _playerState.Value.currentPlayerState_fl_internal;
        if (_playerState.Value.currentWeapon_fl_internal != 0) inventory.internal_CurrentWeapon = _playerState.Value.currentWeapon_fl_internal;
        inventory.isScoping = _playerState.Value.isScoping_internal;
        inventory.isShooting = _playerState.Value.isShooting_internal;
        playerStateMachine.grapplePoint = _playerState.Value.GrapplePosition;
        playerStateMachine.updown_Blendconstant = Mathf.Lerp(playerStateMachine.updown_Blendconstant, (_playerState.Value.Rotation.x + 90) / 180, 0.3f);

        playerStateMachine.health = 0;
        //ebug.Log(_playerState.Value.Rotation.x);
/*        float blendconstant = (playerStateMachine.xRotation + 90) / 180;
        playerStateMachine.player_EXT_ARM_anim_controller.SetFloat("Y_Look", blendconstant);
        inventory.EXT_GetCurrentWeaponAnimator().SetFloat("Y_Look", blendconstant);*/

        //playerStateMachine.isGrappling = _playerState.Value.isGrappling_internal;

        //Debug.Log(rotatables.transform.rotation.eulerAngles.y);
    }

    #endregion

    #region Transmit ID

/*    [ServerRpc]
    public void AddPlayer(float clientID)
    {

    }


    [ClientRpc]
    public void AddPlayerLocally(float clientID)
    {

    }*/


    #endregion



    private struct PlayerNetworkState : INetworkSerializable
    {
        private float _posX, _posY, _posZ;

        private float G_posX, G_posY, G_posZ;

        private float _rotX, _rotY;

        //private bool isGrappling;

        private bool isScoping;

        private bool isShooting;

        private float currentPlayerState_fl;

        private float currentWeapon_fl;

        private int _health;

        internal Vector3 Position
        {
            get => new(_posX, _posY, _posZ);
            set
            {
                _posX = value.x;
                _posY = value.y;
                _posZ = value.z;
            }
        }

        internal Vector2 Rotation
        {
            get => new (_rotX, _rotY);
            set
            {
                _rotX = value.x;
                _rotY = value.y;
            }
        }

/*        internal bool isGrappling_internal
        {
            get => isGrappling;
            set
            {
                isGrappling = value;
            }
        }*/

        internal bool isScoping_internal
        {
            get => isScoping;
            set
            {
                isScoping = value;
            }
        }

        internal bool isShooting_internal
        {
            get => isShooting;
            set
            {
                isShooting = value;
            }
        }

        internal Vector3 GrapplePosition
        {
            get => new(G_posX, G_posY, G_posZ);
            set
            {
                G_posX = value.x;
                G_posY = value.y;
                G_posZ = value.z;
            }
        }


        internal float currentPlayerState_fl_internal
        {
            get => currentPlayerState_fl;
            set
            {
                currentPlayerState_fl = value;
            }
        }

        internal float currentWeapon_fl_internal
        {
            get => currentWeapon_fl;
            set
            {
                currentWeapon_fl = value;
            }
        }

        internal int Health
        {
            get => _health;
            set
            {
                _health = value;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _posX);
            serializer.SerializeValue(ref _posY);
            serializer.SerializeValue(ref _posZ);
            serializer.SerializeValue(ref _rotX);
            serializer.SerializeValue(ref _rotY);
            //serializer.SerializeValue(ref isGrappling);
            serializer.SerializeValue(ref isScoping);
            serializer.SerializeValue(ref isShooting);
            serializer.SerializeValue(ref G_posX);
            serializer.SerializeValue(ref G_posY);
            serializer.SerializeValue(ref G_posZ);
            serializer.SerializeValue(ref currentPlayerState_fl);
            serializer.SerializeValue(ref currentWeapon_fl);
            serializer.SerializeValue(ref _health);
        }
    }
}