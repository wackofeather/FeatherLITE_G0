using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerBase : NetworkBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected InputActionReference move;
    [SerializeField] protected InputActionReference fire;
    [SerializeField] protected InputActionReference scope;
    [SerializeField] protected InputActionReference pause;
    [SerializeField] protected InputActionReference Unpause;

    [SerializeField] protected Transform Rotatables;
    [SerializeField] protected Transform PlayerCamera;
    [SerializeField] protected Transform CameraHolder;
    [SerializeField] protected Transform Exterior;
    [SerializeField] protected Transform Viewport;

    [SerializeField] protected List<GameObject> OwnerOnlyObjects;
    [SerializeField] protected List<GameObject> DummyOnlyObjects;

    [SerializeField] protected float speed;
    [SerializeField] protected float mouseSens;
    [SerializeField] protected float accel;
    [SerializeField] protected float tooFastaccel;
    [SerializeField] protected float BreakNeckSpeed;






    protected Vector3 inputVector;

    protected float xRotation = 0;
    protected float yRotation = 0;

    protected Vector3 putTogetherVelocity;







    /// <summary>
    /// grapple
    /// </summary>



    [SerializeField] protected GrapplingGun grapplingGunScript;





    [SerializeField] protected UnityEngine.LineRenderer lineRenderer;




    public bool CanMove;



}
