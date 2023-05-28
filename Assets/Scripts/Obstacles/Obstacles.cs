using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Level1
{


    public class Obstacles : TriggerBase
    {
        [System.Serializable] public class ColliderEvent : UnityEvent{}
        
        [SerializeField] public ColliderEvent CollidePlayerAction;
        protected override void enterEvent()
        {
            Debug.Log("collide player");
            EventManager.Instance.PlayerDeadEventTrigger();
            CollidePlayerAction?.Invoke();
        }
    }
    
    
}