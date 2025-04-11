using BT.BTNode;
using BT.Editor.Win;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BT.Editor
{
    public static class BTEx
    {
        public static void LinkAddData(this Edge edge)
        {
            NodeView outNode = (NodeView)edge.output.node;
            NodeView inputNode = (NodeView)edge.input.node;

            switch (outNode.data)
            {
                case BTCompositeNode compositeNode:
                    compositeNode.list_childNode.Add(inputNode.data);
                    return;
                case BTPreconditionNode preconditionNode:
                    preconditionNode.childNode = inputNode.data;
                    return;
            }
        }

        public static void UnlinkRemoveData(this Edge edge)
        {
            NodeView outNode = (NodeView)edge.output.node;
            NodeView inputNode = (NodeView)edge.input.node;

            switch (outNode.data)
            {
                case BTCompositeNode compositeNode:
                    compositeNode.list_childNode.Remove(inputNode.data);
                    return;
                case BTPreconditionNode preconditionNode:
                    preconditionNode.childNode = null;
                    return;
            }
        }

        public static List<BTNodeBase> CloneData(this List<BTNodeBase> list_data)
        {
            byte[] array_nodeByte = SerializationUtility.SerializeValue(list_data, DataFormat.Binary);
            var list_deepCopyData = SerializationUtility.DeserializeValue<List<BTNodeBase>>(array_nodeByte, DataFormat.Binary);

            for (int i = 0; i < list_deepCopyData.Count; i++)
            {
                var item = list_deepCopyData[i];
               item.Guid = System.Guid.NewGuid().ToString();

                switch (item)
                {
                    case BTCompositeNode compositeNode:
                        if (compositeNode.list_childNode.Count == 0) break;
                        compositeNode.list_childNode = compositeNode.list_childNode.Intersect(list_deepCopyData).ToList();
                        break;
                    case BTPreconditionNode preconditionNode:
                        if(preconditionNode.childNode == null) break;
                        if(!list_deepCopyData.Exists(temp => temp == preconditionNode.childNode))
                            preconditionNode.childNode = null;
                        break;
                }
                item.nodePos += Vector2.one * 50;
            }

            return list_deepCopyData;
        }
    }
}


