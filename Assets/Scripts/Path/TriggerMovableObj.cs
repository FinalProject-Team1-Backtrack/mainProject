using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Path
{
    public class TriggerMovableObj : TriggerBase
    {
        [Header("放入点，生成曲线，让玩家以曲线模式移动，当前玩家位置为起点位置，最后一个点为终点位置")] [SerializeField]
        private Transform[] points;
        [SerializeField] private Transform moveObjTransform;
        private Vector3[] pointPos;
        [SerializeField] private float duration;
        [SerializeField] private PathType type = PathType.CatmullRom;
        [SerializeField] private Ease easeType = DOTween.defaultEaseType;

        private void Start()
        {
            if (points.Length == 0) Debug.LogWarning(transform.name + " 未加入点，生成不了曲线");
            pointPos = new Vector3[points.Length];
            isOneTime = true;
            for (var i = 0; i < points.Length; i++)
            {
                pointPos[i] = points[i].position;
            }
        }

        protected override void enterEvent()
        {
            base.enterEvent();
            Tween tw = moveObjTransform.DOPath(pointPos, duration, type).SetLookAt(0.01f).SetEase(easeType);
            
        }
        
        
        

    }
}