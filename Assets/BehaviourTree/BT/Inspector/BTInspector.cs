using BT.BTNode;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct ViewTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public Matrix4x4 matrix;
}

public class BTWindowData
{
    public ViewTransform viewTras = new ViewTransform();

    public BTWindowData()
    {
        viewTras.scale = Vector3.one;
    }

    public void Zero()
    {
        viewTras.position = Vector3.zero;
        viewTras.scale = Vector3.one;
    }
}

[DisallowMultipleComponent]
public class BTInspector : SerializedMonoBehaviour
{
    [OdinSerialize]
    public BTNodeBase root;

    [OdinSerialize, ReadOnly]
    public BTWindowData windowData = new BTWindowData();

    public void Init()
    {
        if (root == null)
        {
            BT.BTNode.Sequence sequence = new BT.BTNode.Sequence("Root");
            root = sequence;
        }
    }
}
