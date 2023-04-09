using System;
using System.Collections;
using System.Numerics;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.UIElements;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerMotor : MonoBehaviour
{
    private float speedConstant = 100f;
    
    protected Rigidbody _rigidBody;
    protected Collider _collider;

    protected Vector3 _nextMoveXVelocity;
    protected Vector3 _nextMoveZVelocity;
    #region 外部访问属性
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get => _currentSpeed;
    }

    private float _currentXSpeed;

    public float CurrentXSpeed
    {
        get => _currentXSpeed;
    }

    private float _currentYSpeed;
    public float CurrentYSpeed
    {
        get => _currentXSpeed;
    }


    #endregion
    
    #region 人物属性
    [Header("人物移动属性")]
    [SerializeField] private float Zspeed = 1f;

    public float ZSpeed
    {
        get => Zspeed;
        set => Zspeed = value;
    }

    [SerializeField] private float Xspeed = 1f;
    // [SerializeField] private float XMaxSpeed = 2f;
    // [SerializeField] private float XAccSpeed = 1f;
    [SerializeField] private float jumpVerticalForce = 2f;
    #endregion
    #region 碰撞/射线属性
    [Header("碰撞/射线属性")] 
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float detectionLength = 0.2f;
    #endregion
    
    #region 布尔变量，motor状态
    protected bool canMove = false; //是否可以移动（包括跳跃） 移动的总控制
    protected bool canMoveRight = true; //是否可以像左移动
    protected bool canMoveLeft = true; //是否可以像右移动
    #endregion
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        
        canMove = false;
        if(_rigidBody == null) Debug.LogError("未找到RigidBody");

        _rigidBody.isKinematic = true;

    }

    public void MoveForward()
    {
        var dir = transform.InverseTransformDirection(_rigidBody.velocity);
        var curZSpeed = dir.z;
        if (curZSpeed < Zspeed * Time.deltaTime * speedConstant)
        {
            _nextMoveZVelocity = transform.forward * Zspeed * Time.deltaTime * speedConstant;

        }
        else
        {
            dir.x = 0;
            dir.y = 0;
            var curV = transform.TransformDirection(dir);
            _nextMoveZVelocity = curV;
        }
    }

    public void MoveHorizontal(float dir)
    {
        Vector3 pos;
        //检测是否可以往左走，是否可以往右走
        if (!canMoveRight && dir > 0)
        {
            pos = Vector3.zero;
            
        }else if (!canMoveLeft && dir < 0)
        {
            pos = Vector3.zero;

        }
        else
        {
           pos  =  transform.right * dir;

        }
        _nextMoveXVelocity = pos * Xspeed * Time.deltaTime * speedConstant;

    }

    public virtual void Move()
    {
        if (canMove)
        {
            Vector3 velBeforeSlope = (_nextMoveXVelocity + _nextMoveZVelocity);
        
            Vector3 velAfterSlope = velBeforeSlope;
            RaycastHit hit;
            if (IsOnSlope(out hit))
            {
                var dot = Vector3.Dot(hit.normal, transform.forward);
                //判断是不是在上坡
                 //if (dot < 0f)
                 //{
                      //velAfterSlope = Vector3.ProjectOnPlane(velBeforeSlope, hit.normal);
                
                 //}else 
                 //{
                     //velAfterSlope.y = _rigidBody.velocity.y;
                     
                //}

                if (dot > 0f)
                {
                    //velAfterSlope = Vector3.ProjectOnPlane(velBeforeSlope, hit.normal);
                    velAfterSlope.y = _rigidBody.velocity.y;

                }

            }
            else
            {
                velAfterSlope.y = _rigidBody.velocity.y;
            }
            _rigidBody.velocity = velAfterSlope;
            
        }

    }

    void FixedUpdate()
    {
        var locVel = transform.InverseTransformDirection(_rigidBody.velocity);//转换为本地坐标系
        _currentYSpeed = locVel.y;
        _currentXSpeed = locVel.x;
        _currentSpeed = locVel.z;

        RotateInFixedUpdate();
        //addExtraInitialGravity();
    }

    public float extraGravity = 15f;
    //想加一点重力，但是没啥效果
    void addExtraInitialGravity()
    {
        if (isOneTimeFalling())
        {
            _rigidBody.AddForce(transform.up * -1 * extraGravity * speedConstant);
        }
    }
    public void MotorStart()
    {
        canMove = true;
        _rigidBody.isKinematic = false;

    }

    public void JumpVertical()
    {
        if (!canMove) return;
        if (IsGrounded())
        {
            _rigidBody.AddForce(Vector3.up * jumpVerticalForce * speedConstant);
        }
    }

    public bool IsFalling()
    {
        bool falling;
        if (_rigidBody.velocity.y < -0.2f)
        {
            falling = true;
        }
        else
        {
            falling = false;
        }

        return falling;
    }

    private bool isOneTimeFall = false;
    public bool isOneTimeFalling()
    {
        //if (!IsGrounded() && isOneTimeFall)
        if(IsFalling() && isOneTimeFall)
        {
            isOneTimeFall = false;
            return true;
        }

        if (IsGrounded())
        {
            isOneTimeFall = true;
        }

        return false;
    }
    protected bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _collider.bounds.extents.y + 0.2f,groundLayer);
    }

    protected bool IsOnSlope(out RaycastHit hit)//检测是否在斜坡上
    {
        
        if (Physics.Raycast(
                transform.position, Vector3.down,  out hit,
                _collider.bounds.extents.y + detectionLength, groundLayer))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private UnityEngine.Quaternion rotate_StartPos;
    private UnityEngine.Quaternion rotate_EndPos;
    private float rotate_duration;
    private float rotate_addUpTime;
    private AnimationCurve rotate_curve;
    private bool rotate_start = false;
    public void RotateInSelfAxis(Vector3 rotateAngle, float duration,AnimationCurve curve)//自身坐标系旋转
    {
        rotate_StartPos = transform.rotation;
        rotate_EndPos = UnityEngine.Quaternion.Euler(rotate_StartPos.eulerAngles + rotateAngle);
        rotate_duration = duration;
        rotate_addUpTime = 0;
        rotate_start = true;
        rotate_curve = curve;
        // UnityEngine.Quaternion qt = UnityEngine.Quaternion.Slerp(rotate_StartPos, UnityEngine.Quaternion.Euler(rotate_StartPos.eulerAngles + rotateAngle), 0.1f);
        // transform.Rotate(qt.eulerAngles);
    }

    private void RotateInFixedUpdate()
    {

        if (!rotate_start) return;
        rotate_addUpTime += Time.deltaTime / rotate_duration;
        if (rotate_addUpTime > 1f)
        {
            rotate_start = false;
            return;
        }

        UnityEngine.Quaternion qt =
            UnityEngine.Quaternion.SlerpUnclamped(rotate_StartPos, rotate_EndPos, rotate_curve.Evaluate(rotate_addUpTime));
        transform.Rotate(qt.eulerAngles - transform.rotation.eulerAngles);
        if (rotate_addUpTime > 1f) rotate_start = false;

    }

    public void DisableMoveLeft(bool disable)
    {
        if (disable)
        {
            canMoveLeft = false;
        }
        else
        {
            canMoveLeft = true;
        }
    }

    public void DisableMoveRight(bool disable)
    {
        if (disable)
        {
            canMoveRight = false;

        }
        else
        {
            canMoveRight = true;
        }
    }

    
}
