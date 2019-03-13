using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    // Global containers for all active mesh/terrain tags
    public static List<MeshFilter> m_Meshes = new List<MeshFilter>();

    public static List<NavMeshSourceTag> m_SourceTags = new List<NavMeshSourceTag>();
    public int navMeshSettings = 0;
    void OnEnable()
    {
        var m = GetComponent<MeshFilter>();
        if (m != null)
        {
            m_Meshes.Add(m);
            m_SourceTags.Add(this);
        }

    }

    void OnDisable()
    {
        var m = GetComponent<MeshFilter>();
        if (m != null)
        {
            m_Meshes.Remove(m);
        }

    }

    // Collect all the navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();

        for (var i = 0; i < m_SourceTags.Count; ++i)
        {
            var source = m_SourceTags[i];
            var mf =source.GetComponent<MeshFilter>();
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = source.navMeshSettings;
            
            sources.Add(s);
        }

    }
}
