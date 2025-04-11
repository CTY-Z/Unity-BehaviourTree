using BT.BTNode;
using BT.Editor.Win;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CustomEditor(typeof(BTInspector))]
public class BTInspectorEditor : OdinEditor
{
    BTInspector inspector;

    public int TreeID;

    //public Dictionary<string, NodeView> dic_Guid_node = new();

    protected override void OnEnable()
    {
        base.OnEnable();

        inspector = (BTInspector)target;

        inspector.Init();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("Editor"))
        {
            //UnityEditor.EditorApplication.ExecuteMenuItem("BehaviourTree/BehaviourTree");
            BehaviourTreeWin.GetWinByInspector(inspector);
        }

        if (GUILayout.Button("Clear"))
        {
            inspector.root = null;
            //dic_Guid_node.Clear();

            //TODO
            BehaviourTreeWin.winRoot.treeNodeWin.dic_Guid_node.Clear();
        }

        serializedObject.ApplyModifiedProperties();
    }
}

