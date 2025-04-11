using BT.BTNode;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.Win
{
    public class NodeView : Node
    {
        public BTNodeBase data;

        public Port input;
        public Port output;

        public NodeView(BTNodeBase data)
        {
            this.data = data;

            InitNodeView(data);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Set Root", SetRoot);
        }

        public void InitNodeView(BTNodeBase data)
        {
            input = +this;
            input.portName = "input";
            inputContainer.Add(input);

            if (!(data is BTActionNode))
            {
                output = this - (data is BTPreconditionNode);
                output.portName = "output";
                outputContainer.Add(output);
            }
        }

        public void UpdateData()
        {
            this.title = data.nodeName;
            this.name = data.nodeName;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            data.nodePos = new Vector2(newPos.xMin, newPos.yMin);
        }

        public void CreateLine()
        {
            TreeNodeWin treeNodeWin = BehaviourTreeWin.winRoot.treeNodeWin;

            switch (data)
            {
                case BTCompositeNode compositeNode:
                    compositeNode.list_childNode.ForEach(item =>
                    {
                        treeNodeWin.dic_Guid_node.TryGetValue(item.Guid, out NodeView node);
                        treeNodeWin.AddElement(Link(this.output, node.input));
                    });
                    break;
                case BTPreconditionNode preconditionNode:
                    if(preconditionNode.childNode != null)
                    {
                        treeNodeWin.dic_Guid_node.TryGetValue(preconditionNode.childNode.Guid, out NodeView node);
                        treeNodeWin.AddElement(Link(this.output, node.input));
                    }
                    break;
                default:
                    break;
            }
        }

        public static Edge Link(Port output, Port input)
        {
            Edge temp_Edge = new Edge() { output = output, input = input  };

            temp_Edge?.input.Connect(temp_Edge);
            temp_Edge?.output.Connect(temp_Edge);

            return temp_Edge;
        }

        public void SetRoot(DropdownMenuAction obj)
        {
            BehaviourTreeWin.target.root = data;
        }

        public static Port operator +(NodeView view)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(NodeView));
            return port;
        }
        public static Port operator -(NodeView view, bool isSingle)
        {
            Port port = Port.Create<Edge>(Orientation.Horizontal, Direction.Output, 
                isSingle? Port.Capacity.Single : Port.Capacity.Multi, typeof(NodeView));
            return port;
        }
    }
}

