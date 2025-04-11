using BT.BTNode;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest : SerializedMonoBehaviour
{
    [OdinSerialize]
    BTNodeBase root;

    private void Update()
    {
        root.Tick();
    }

#if UNITY_EDITOR

    public void OpenBTWin()
    {
        GetInstanceID();
        UnityEditor.EditorApplication.ExecuteMenuItem("BehaviourTree/BehaviourTree");
    }

#endif
}
