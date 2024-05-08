using UnityEngine;
using UnityEngine.EventSystems;
using Parabox.CSG;

public class SelectionController : MonoBehaviour
{
    public static SelectionController _SelectionController;

    [SerializeField] Material m_HighlightMaterial;
    [SerializeField] Material m_SelectionMaterial;
    Material m_OriginalMaterialHighlight;
    Material m_OriginalMaterialSelection;
    Transform m_HighlightTransform;
    Transform m_SelectionTransform;
    RaycastHit m_RaycastHit;

    void Awake()
    {
        _SelectionController = this;
    }

    void Update()
    {
        if (m_HighlightTransform != null)
        {
            m_HighlightTransform.GetComponent<MeshRenderer>().sharedMaterial = m_OriginalMaterialHighlight;
            m_HighlightTransform = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out m_RaycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            if (!m_RaycastHit.transform.CompareTag("Box")) return;

            m_HighlightTransform = m_RaycastHit.transform;
            if (m_HighlightTransform != m_SelectionTransform)
            {
                if (m_HighlightTransform.GetComponent<MeshRenderer>().material != m_HighlightMaterial)
                {
                    m_OriginalMaterialHighlight = m_HighlightTransform.GetComponent<MeshRenderer>().material;
                    m_HighlightTransform.GetComponent<MeshRenderer>().material = m_HighlightMaterial;
                }
            }
            else
            {
                m_HighlightTransform = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (m_HighlightTransform)
            {
                if (m_SelectionTransform != null)
                {
                    m_SelectionTransform.GetComponent<MeshRenderer>().material = m_OriginalMaterialSelection;
                }
                m_SelectionTransform = m_RaycastHit.transform;
                if (m_SelectionTransform.GetComponent<MeshRenderer>().material != m_SelectionMaterial)
                {
                    m_OriginalMaterialSelection = m_OriginalMaterialHighlight;
                    m_SelectionTransform.GetComponent<MeshRenderer>().material = m_SelectionMaterial;
                }
                m_HighlightTransform = null;
            }
            else
            {
                if (m_SelectionTransform)
                {
                    m_SelectionTransform.GetComponent<MeshRenderer>().material = m_OriginalMaterialSelection;
                    m_SelectionTransform = null;
                }
            }
        }
    }

    public void Subtract(GameObject leftObj, GameObject rightObj)
    {
        Model result;
        result = CSG.Subtract(leftObj, rightObj);
        GenerateBarycentric(leftObj, result);
    }

    /**
     * Rebuild mesh with individual triangles, adding barycentric coordinates
     * in the colors channel.  Not the most ideal wireframe implementation,
     * but it works and didn't take an inordinate amount of time :)
     */
    public void GenerateBarycentric(GameObject go, Model result)
    {
        Vector3 originalScale = go.transform.localScale;
        go.transform.localScale = Vector3.one;

        Mesh m = result.mesh;

        if (m == null) return;

        int[] tris = m.triangles;
        int triangleCount = tris.Length;

        Vector3[] mesh_vertices = m.vertices;
        Vector3[] mesh_normals = m.normals;
        Vector2[] mesh_uv = m.uv;

        Vector3[] vertices = new Vector3[triangleCount];
        Vector3[] normals = new Vector3[triangleCount];
        Vector2[] uv = new Vector2[triangleCount];
        Color[] colors = new Color[triangleCount];

        for (int i = 0; i < triangleCount; i++)
        {
            vertices[i] = mesh_vertices[tris[i]];
            normals[i] = mesh_normals[tris[i]];
        }

        Mesh wireframeMesh = new Mesh();

        wireframeMesh.Clear();
        wireframeMesh.vertices = vertices;
        wireframeMesh.triangles = tris;
        wireframeMesh.normals = normals;

        go.transform.localScale = originalScale;
        go.GetComponent<MeshFilter>().sharedMesh = wireframeMesh;
    }
}