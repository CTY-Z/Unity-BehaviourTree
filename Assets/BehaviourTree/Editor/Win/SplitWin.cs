using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;



namespace BT.Editor.Win
{
    public class SplitWin : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitWin, UxmlTraits> { }

        public SplitWin() 
        {

        }
    }
}