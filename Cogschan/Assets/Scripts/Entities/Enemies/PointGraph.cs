using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PointGraph : MonoBehaviour
{
    /// <summary>
    /// A class that wraps a pair of ints
    /// </summary>
    [System.Serializable]
    public class Edge
    {
        public int Node;
        public int OtherNode;

        public Edge(int node, int otherNode)
        {
            Node = node;
            OtherNode = otherNode;
        }
    }

    public enum DebugDrawModeType
    {
        Node,
        NodeIndex,
    }

    public enum AutoEdgeModeType
    {
        None,
        Line,
        Loop,
        LineOfSight,
    }

    [Header("Graph")]
    [SerializeField] private Vector3[] _nodes;
    [SerializeField] private Edge[] _edges;
    [SerializeField] private AutoEdgeModeType _autoEdgeMode = AutoEdgeModeType.None;
    [Header("Debug Only")]
    [SerializeField] private Color _nodeColor;
    [SerializeField] private float _nodeRadius;
    [SerializeField] private Color _edgeColor;

    public Vector3[] Nodes { get => _nodes; set => _nodes = value; }
    public Color DebugNodeColor { get => _nodeColor; set => _nodeColor = value; }
    public float DebugNodeRadius { get => _nodeRadius; set => _nodeRadius = value; }
    public Color DebugEdgeColor { get => _edgeColor; set => _edgeColor = value; }
    public DebugDrawModeType DebugDrawMode { get; set; } = DebugDrawModeType.Node;
    public AutoEdgeModeType AutoEdgeMode { get => _autoEdgeMode; set => _autoEdgeMode = value; }

    public Vector3 GetNode(int index)
    {
        return transform.TransformPoint(Nodes[index]);
    }

    public int[] GetAdjacent(int index)
    {
        List<int> result = new List<int>();

        foreach (Edge edge in _edges)
        {
            if (edge.Node == index)
            {
                if (edge.OtherNode != index) result.Add(edge.OtherNode);
            }
            else if (edge.OtherNode == index)
            {
                if (edge.Node != index) result.Add(edge.Node);
            }
        }

        return result.ToArray();
    }

    public void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            if (AutoEdgeMode == AutoEdgeModeType.Line)
            {
                if (_nodes.Length == 0)
                {
                    _edges = new Edge[0];
                }
                else
                {
                    _edges = new Edge[_nodes.Length - 1];
                    for (int i = 0; i < _nodes.Length - 1; i++)
                        _edges[i] = new Edge(i, i + 1);
                }
            }
            else if (AutoEdgeMode == AutoEdgeModeType.Loop)
            {
                if (_nodes.Length == 0)
                {
                    _edges = new Edge[0];
                }
                else
                {
                    _edges = new Edge[_nodes.Length];
                    for (int i = 0; i < _nodes.Length - 1; i++)
                        _edges[i] = new Edge(i, i + 1);
                    _edges[_nodes.Length - 1] = new Edge(_nodes.Length - 1, 0);
                }
            }
            else if (AutoEdgeMode == AutoEdgeModeType.LineOfSight)
            {
                var edges = new List<Edge>();
                for (int i = 0; i < _nodes.Length; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (i == j) continue;
                        if (!Physics.Raycast(GetNode(i), GetNode(j) - GetNode(i), out RaycastHit hit, Vector3.Distance(GetNode(i), GetNode(j)), LayerMask.GetMask("Level")))
                            edges.Add(new Edge(i, j));
                    }
                }
                _edges = edges.ToArray();
            }
        }
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = DebugEdgeColor;
        foreach (Edge edge in _edges)
        {
            if (edge.Node >= Nodes.Length || edge.OtherNode >= Nodes.Length) continue;

            Gizmos.DrawLine(GetNode(edge.Node), GetNode(edge.OtherNode));
        }

        if (DebugDrawMode == DebugDrawModeType.Node)
        {
            Gizmos.color = DebugNodeColor;
            for (int i = 0; i < Nodes.Length; i++)
                Gizmos.DrawSphere(GetNode(i), DebugNodeRadius);
        }
        else if (DebugDrawMode == DebugDrawModeType.NodeIndex)
        {
            Gizmos.color = DebugNodeColor;
            for (int i = 0; i < Nodes.Length; i++)
            {
                Gizmos.DrawWireSphere(GetNode(i), DebugNodeRadius);

                EditorUtils.DrawTextPretty(GetNode(i), i.ToString(), DebugNodeColor.GetNeutralContrastColor(), 24, EditorUtils.Urbanist.Black, DebugNodeColor, 6);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PointGraph))]
public class PointGraphEditor : Editor
{
    private PointGraph _pointGraph;

    private void OnEnable()
    {
        _pointGraph = (PointGraph)target;
        _pointGraph.DebugDrawMode = PointGraph.DebugDrawModeType.NodeIndex;
    }

    private void OnDisable()
    {
        if (_pointGraph != null)
            _pointGraph.DebugDrawMode = PointGraph.DebugDrawModeType.Node;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    private void OnSceneGUI()
    {
        if (_pointGraph.Nodes != null)
        {
            for (int i = 0; i < _pointGraph.Nodes.Length; i++)
            {
                EditorGUI.BeginChangeCheck();
                var newNodePos = _pointGraph.transform.InverseTransformPoint(Handles.PositionHandle(_pointGraph.transform.TransformPoint(_pointGraph.Nodes[i]), Quaternion.identity));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Node Position");
                    _pointGraph.Nodes[i] = newNodePos;
                    _pointGraph.OnValidate();
                }
            }
        }
    }
}
#endif
