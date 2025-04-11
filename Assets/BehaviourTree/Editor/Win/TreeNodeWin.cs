using BT.BTNode;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static BT.Editor.Win.BehaviourTreeWin;

namespace BT.Editor.Win
{
    public class TreeNodeWin : GraphView
    {
        RightClickMenu menuWindowProvider;

        public Dictionary<string, NodeView> dic_Guid_node = new();

        public new class UxmlFactory : UxmlFactory<TreeNodeWin, UxmlTraits> { }

        public TreeNodeWin()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/Win/BehaviourTreeWin.uss"));

            InitMenu();

            graphViewChanged += OnGraphViewChange;
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
        }

        private GraphViewChange OnGraphViewChange(GraphViewChange change)
        {
            if(change.edgesToCreate != null)
            {
                change.edgesToCreate.ForEach(item =>
                {
                    item.LinkAddData();
                });
            }

            if (change.elementsToRemove != null)
            {
                change.elementsToRemove.ForEach(item =>
                {
                    if(item is Edge edge)
                        edge.UnlinkRemoveData();
                });
            }

            return change;
        }

        public void SetViewTrans(ViewTransform trans)
        {
            this.viewTransform.position = trans.position;
            this.viewTransform.scale = trans.scale;
        }

        public void InitMenu()
        {
            menuWindowProvider = ScriptableObject.CreateInstance<RightClickMenu>();

            menuWindowProvider.OnSelectEntryHandler = OnMenuSelectEntry;

            nodeCreationRequest += context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), menuWindowProvider);
            };
        }

        private bool OnMenuSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var root = winRoot.rootVisualElement;

            var mousePos = root.ChangeCoordinatesTo(root.parent, context.screenMousePosition - winRoot.position.position);
            var finalPos = contentViewContainer.WorldToLocal(mousePos);

            CreateNode((Type)SearchTreeEntry.userData, finalPos);

            return true;
        }

        public NodeView CreateNode(BTNodeBase node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.SetPosition(new Rect(node.nodePos, Vector2.one));
            AddElement(nodeView);

            dic_Guid_node.Add(nodeView.data.Guid, nodeView);

            return nodeView;
        }

        private void CreateNode(Type t, Vector2 pos)
        {
            BTNodeBase data = (BTNodeBase)Activator.CreateInstance(t);
            data.nodeName = t.Name;

            NodeView node = new NodeView(data);
            data.Guid = System.Guid.NewGuid().ToString();
            node.SetPosition(new Rect(pos, Vector2.one));
            AddElement(node);

            dic_Guid_node.Add(data.Guid, node);
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.Where(endPorts => endPorts.direction != startAnchor.direction && endPorts.node != startAnchor.node).ToList();
        }

        #region KeDownEvent

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Tab)
                evt.StopPropagation();

            if (!evt.ctrlKey) return;
            switch (evt.keyCode)
            {
                case KeyCode.Space:
                    SearchWindow.Open(new SearchWindowContext(new Vector2(0, 0)), menuWindowProvider);
                    evt.StopPropagation(); 
                    break;
                case KeyCode.S:
                    winRoot.Save();
                    evt.StopPropagation();
                    break;
                case KeyCode.E:

                    evt.StopPropagation();
                    break;
                case KeyCode.X:

                    evt.StopPropagation();
                    break;
                case KeyCode.C:
                    Copy();
                    evt.StopPropagation();
                    break;
                case KeyCode.V:
                    Paste();
                    evt.StopPropagation();
                    break;
                case KeyCode.Z:

                    evt.StopPropagation();
                    break;
            }
        }

        List<BTNodeBase> list_deepCopyData;
        private void Copy()
        {
            list_deepCopyData = selection.OfType<NodeView>().Select(item => item.data).ToList().CloneData();
        }

        private void Paste()
        {
            if (list_deepCopyData == null) return;

            List<NodeView> list_nodeView = new List<NodeView>();
            for (int i = 0; i < list_deepCopyData.Count; i++)
            {
                BTNodeBase item = list_deepCopyData[i];
                list_nodeView.Add(CreateNode(item));
            }

            list_nodeView.ForEach(item => item.CreateLine());
            list_deepCopyData = list_deepCopyData.CloneData();
        }

        #endregion
    }
}


