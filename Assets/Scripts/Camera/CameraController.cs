using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Common;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoSingleton<CameraController>
{
    #region CamProperty
    [HideInInspector] public Transform vcam1;
    [HideInInspector] public Transform vcam2;
    [HideInInspector] public CinemachineVirtualCamera vcam1CM;
    [HideInInspector] public CinemachineVirtualCamera vcam2CM;
    [HideInInspector] public Transform curVcam;
    #endregion

    [SerializeField] public bool isFixOnPath = true;
    
    private Transform _playerTr;
    private PlayerMotor _playerMotor;
    private Transform _lookAtPoint;
    private void Awake()
    {
        vcam1 = this.transform.Find("CM vcam1");
        vcam2 = this.transform.Find("CM vcam2");
        if(vcam1 == null) Debug.LogError("找不到vcam1");
        if(vcam2 == null) Debug.LogError("找不到vcam1");

        vcam1CM = vcam1.GetComponent<CinemachineVirtualCamera>();
        vcam2CM = vcam2.GetComponent<CinemachineVirtualCamera>();//获取vcam2的CinemachineVirtualCamera组件
        _playerTr = PlayerController.Instance.transform;
        _lookAtPoint = _playerTr.Find("LookAtPoint");
        _playerMotor = _playerTr.GetComponent<PlayerMotor>();
        if(_lookAtPoint == null) Debug.LogError("Player里没有LookAtPoint，摄像机无法跟踪");
        vcam1CM.Follow = _lookAtPoint;
        vcam1CM.LookAt = _lookAtPoint;
        
        vcam2CM.Follow = _lookAtPoint;
        vcam2CM.LookAt = _lookAtPoint;
        
        CinemachineTransposer transposer1 = vcam1CM.GetCinemachineComponent<CinemachineTransposer>();//获取vcam1的transposer组件
        CinemachineTransposer transposer2 = vcam2CM.GetCinemachineComponent<CinemachineTransposer>();
        transposer2.m_FollowOffset = transposer1.m_FollowOffset;

        curVcam = vcam1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFixOnPath)
        {
            FixCameraOnPath();
        }
        else
        {
            _lookAtPoint.localPosition = Vector3.zero;
        }
    }

    
    void FixCameraOnPath()
    {
        RaycastHit hit;
        if (_playerMotor.RayCastBottom(out hit))
        {
            Transform hitTr = hit.collider.transform;
            if (hitTr.position.x - _lookAtPoint.position.x < 5f)
            {
                _lookAtPoint.position = new Vector3(hitTr.position.x, _lookAtPoint.position.y, _lookAtPoint.position.z);
            }
        }
        
    }

}
