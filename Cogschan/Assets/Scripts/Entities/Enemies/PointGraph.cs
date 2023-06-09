using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointGraph : MonoBehaviour
{
    /// <summary>
    /// A class that wraps a pair of ints
    /// </summary>
    [System.Serializable]
    public class Edge
    {
        public int Item1;
        public int Item2;
    }

    [Header("Graph")]
    [SerializeField] private Vector3[] _nodes;
    [SerializeField] private Edge[] _edges;
    [Header("Debug Only")]
    [SerializeField] private Color _nodeColor;
    [SerializeField] private float _nodeRadius;
    [SerializeField] private Color _edgeColor;

    public Vector3 GetNode(int index)
    {
        return transform.position + Vector3.Scale(transform.lossyScale, transform.rotation * _nodes[index]);
    }

    public int[] GetAdjacent(int index)
    {
        List<int> result = new List<int>();

        foreach (Edge edge in _edges)
        {
            if (edge.Item1 == index)
            {
                if (edge.Item2 != index) result.Add(edge.Item2);
            }
            else if (edge.Item2 == index)
            {
                if (edge.Item1 != index) result.Add(edge.Item1);
            }
        }

        return result.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _edgeColor;
        foreach (Edge edge in _edges)
        {
            if (edge.Item1 >= _nodes.Length || edge.Item2 >= _nodes.Length) continue;

            Gizmos.DrawLine(GetNode(edge.Item1), GetNode(edge.Item2));
        }

        Gizmos.color = _nodeColor;
        for (int i = 0; i < _nodes.Length; i++)
        {
            Gizmos.DrawSphere(GetNode(i), _nodeRadius);
        }
    }
}
