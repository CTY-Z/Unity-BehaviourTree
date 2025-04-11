using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System;
using System.Reflection;
using BT.BTNode;
using System.Linq;
using Sirenix.Utilities;
using static UnityEngine.GraphicsBuffer;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace BT.Editor.Win
{
    public class BehaviourTreeWin : EditorWindow
    {
        public static BehaviourTreeWin winRoot;
        public TreeNodeWin treeNodeWin;

        public static BTInspector target;

        private Label titleTxt;

        private string treeName;

        [MenuItem("BehaviourTree/BehaviourTree _&r")]
        public static void GetWin()
        {
            BehaviourTreeWin wnd = GetWindow<BehaviourTreeWin>("BehaviourTree");
        }

        public static void GetWinByInspector(BTInspector inspector = null)
        {
            if (inspector != null)
            {
                target = inspector;
            }

            BehaviourTreeWin wnd = GetWindow<BehaviourTreeWin>("BehaviourTree");
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += Refresh;

        }

        public void CreateGUI()
        {
            winRoot = this;
            InitWindow();

            titleTxt = rootVisualElement.Q<Label>("Title");
            
            var treeNameTxt = rootVisualElement.Q<TextField>("BTName");
            treeNameTxt.isDelayed = true;
            treeNameTxt.RegisterValueChangedCallback(OnTreeNameTxtChange);
            
            var CreateBtn = rootVisualElement.Q<Button>("CreateBtn");
            CreateBtn.clicked += OnCreate;
            
            var RefreshBtn = rootVisualElement.Q<Button>("RefreshBtn");
            RefreshBtn.clicked += OnRefresh;

            var ResetPosBtn = rootVisualElement.Q<Button>("ResetPosBtn");
            ResetPosBtn.clicked += OnResetPos;
        }

        private void InitWindow()
        {
            VisualElement root_element = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTree/Editor/Win/BehaviourTreeWin.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTree/Editor/Win/BehaviourTreeWin.uss");

            visualTree.CloneTree(root_element);

            treeNodeWin = root_element.Q<TreeNodeWin>();
            CreateTreeRoot(target?.root);
            treeNodeWin.nodes.OfType<NodeView>().ForEach(item => item.CreateLine());

            if (target != null)
                treeNodeWin.SetViewTrans(target.windowData.viewTras);

        }

        private void OnDestroy()
        {
            Save();
        }

        private void OnInspectorUpdate()
        {
            treeNodeWin.nodes.OfType<NodeView>().ForEach(item => item.UpdateData());
        }

        void CreateTreeRoot(BTNodeBase root)
        {
            if (root == null) return;

            treeNodeWin.CreateNode(root);

            switch (root)
            {
                case BTCompositeNode compositeNode:
                    compositeNode.list_childNode.ForEach(CreateChild);
                    break;
                case BTPreconditionNode preconditionNode:
                    CreateChild(preconditionNode.childNode);
                    break;
            }
        }

        void CreateChild(BTNodeBase node)
        {
            if (node == null) return;

            treeNodeWin.CreateNode(node);

            switch (node)
            {
                case BTCompositeNode compositeNode:
                    compositeNode.list_childNode.ForEach(CreateChild);
                    break;
                case BTPreconditionNode preconditionNode:
                    CreateChild(preconditionNode.childNode);
                    break;
            }
        }
        
        void OnCreate()
        {
            GameObject obj = new GameObject(treeName);
            target = obj.AddComponent<BTInspector>();
            target.Init();

            Refresh();
        }

        void OnRefresh()
        {
            Refresh();
        }

        void OnResetPos()
        {
            if (target != null)
            {
                target.windowData.Zero();
                treeNodeWin.SetViewTrans(target.windowData.viewTras);
            }
        }

        void OnTreeNameTxtChange(ChangeEvent<string> e)
        {
            treeName = e.newValue;
        }

        #region KeyDownEvent

        public void Refresh()
        {
            rootVisualElement.Clear();
            InitWindow();
        }

        public void Save()
        {
            if(target != null)
            {
                target.windowData.viewTras.position = treeNodeWin.viewTransform.position;
                target.windowData.viewTras.scale = treeNodeWin.viewTransform.scale;
            }

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }

        #endregion

        public class RightClickMenu : ScriptableObject, ISearchWindowProvider
        {
            public delegate bool SelectEntryDelegate(SearchTreeEntry searchTreeEntry, SearchWindowContext context);
            public SelectEntryDelegate OnSelectEntryHandler;

            public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
            {
                var entries = new List<SearchTreeEntry>();

                entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

                entries = AddNodeType<BTCompositeNode>(entries, "Composite");
                entries = AddNodeType<BTPreconditionNode>(entries, "Precondition");
                entries = AddNodeType<BTActionNode>(entries, "Action");

                return entries;
            }

            public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
            {
                if (OnSelectEntryHandler == null) return false;

                return OnSelectEntryHandler(SearchTreeEntry, context);
            }

            public List<SearchTreeEntry> AddNodeType<Type>(List<SearchTreeEntry> entries, string pathName)
            {
                entries.Add(new SearchTreeGroupEntry(new GUIContent(pathName)) { level = 1 });
                List<System.Type> rootNodeTypes = GetDericedClasses(typeof(Type));
                foreach (var rootType in rootNodeTypes)
                {
                    string menuName = rootType.Name;
                    entries.Add(new SearchTreeEntry(new GUIContent(menuName)) { level = 2, userData = rootType });
                }

                return entries;
            }

            public List<System.Type> GetDericedClasses(System.Type type)
            {
                List<System.Type> derivedClasses = new List<System.Type>();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (System.Type t in assembly.GetTypes())
                    {
                        if(t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))
                            derivedClasses.Add(t);
                    }
                }

                return derivedClasses;
            }
        }

    }
}
