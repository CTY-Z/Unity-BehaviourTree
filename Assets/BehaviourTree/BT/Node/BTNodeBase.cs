using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.BTNode
{
    public enum BehaviourState
    {
        None,
        succeed,
        failure,
        running,
    }


    public abstract class BTNodeBase
    {
        [ReadOnly]
        [FoldoutGroup("@nodeName"), LabelText("Unique Key")]
        public string Guid;
        [FoldoutGroup("@nodeName"), LabelText("Name")]
        public string nodeName;
        [FoldoutGroup("@nodeName"), LabelText("Position")]
        public Vector2 nodePos;

        public abstract BehaviourState Tick();
    }
}


