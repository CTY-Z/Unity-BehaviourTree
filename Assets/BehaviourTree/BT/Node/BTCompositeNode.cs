using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.BTNode
{
    public abstract class BTCompositeNode : BTNodeBase
    {
        [FoldoutGroup("@nodeName"), LabelText("child")]
        public List<BTNodeBase> list_childNode = new List<BTNodeBase>();
    }

    public class Sequence : BTCompositeNode
    {
        public Sequence() { }
        public Sequence(string name)
        {
            Guid = System.Guid.NewGuid().ToString();
            nodeName = name;
        }

        private int idx;
        public override BehaviourState Tick()
        {
            var state = list_childNode[idx].Tick();

            switch (state)
            {
                case BehaviourState.succeed:
                    idx++;
                    if (idx >= list_childNode.Count)
                    {
                        idx = 0;
                        return BehaviourState.succeed;
                    }
                    return BehaviourState.succeed;

                case BehaviourState.failure:
                    idx = 0;
                    return BehaviourState.failure;

                case BehaviourState.running:
                    return state;
            }

            return BehaviourState.None;
        }
    }

    public class Selector : BTCompositeNode
    {
        private int idx;
        public override BehaviourState Tick()
        {
            var state = list_childNode[idx].Tick();

            switch (state)
            {
                case BehaviourState.succeed:
                    idx = 0;
                    return state;
                case BehaviourState.failure:
                    idx++;
                    if (idx >= list_childNode.Count)
                    {
                        idx = 0;
                        return BehaviourState.failure;
                    }
                    break;
                default:
                    return state;
            }

            return BehaviourState.failure;
        }
    }
}

