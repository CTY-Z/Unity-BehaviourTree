using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.BTNode
{
    public abstract class BTActionNode : BTNodeBase
    {

    }

    public class SetObjectActive : BTActionNode
    {
        [LabelText("Is Active"), SerializeField, FoldoutGroup("@nodeName")]
        private bool isActive;
        [LabelText("Particle"), SerializeField, FoldoutGroup("@nodeName")]
        private GameObject particle;

        public override BehaviourState Tick()
        {
            particle.SetActive(isActive);
            return BehaviourState.succeed;
        }
    }
}

