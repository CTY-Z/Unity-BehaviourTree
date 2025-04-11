using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.BTNode
{
    public abstract class BTPreconditionNode : BTNodeBase
    {
        [FoldoutGroup("@nodeName"), LabelText("child")]
        public BTNodeBase childNode;
    }

    public class Delay : BTPreconditionNode
    {
        [LabelText("timer"), SerializeField, FoldoutGroup("@nodeName")]
        private float timer;

        private float currentTimer;

        public override BehaviourState Tick()
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= timer)
            {
                currentTimer = 0;
                childNode.Tick();
                return BehaviourState.succeed;
            }

            return BehaviourState.running;
        }
    }

    public class Condition : BTPreconditionNode
    {
        [FoldoutGroup("@nodeName"), LabelText("condition"), SerializeField]
        public bool isActive;

        public override BehaviourState Tick()
        {
            if (isActive)
                return childNode.Tick();

            return BehaviourState.failure;
        }
    }

}

