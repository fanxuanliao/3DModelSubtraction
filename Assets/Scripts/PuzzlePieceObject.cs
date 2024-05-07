using UnityEngine;
using Parabox.CSG;

public class PuzzlePieceObject : MonoBehaviour
{
    Vector3 m_Offset;
    float m_ZCoord;
    GameObject m_Composite;

    [SerializeField]
    MeshFilter m_MeshFilter;
    [SerializeField]
    MeshRenderer m_MeshRenderer;

    enum PuzzlePieceStatus
    {
        None,
        Selected,
        Dragging,
        Subtract,
    }

    void Start()
    {
        m_MeshFilter = this.gameObject.GetComponent<MeshFilter>();
        m_MeshRenderer = this.gameObject.GetComponent<MeshRenderer>();
    }

    Vector3 GetMouseAsWorldPoint()
    {

        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = m_ZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    #region mouse handler
    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + m_Offset;
    }

    void OnMouseDown()
    {

        m_ZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;
        m_Offset = gameObject.transform.position - GetMouseAsWorldPoint();
    }
    #endregion


    void OnTriggerEnter(Collider other)
    {
        var otherPiece = other.transform.GetComponent<PuzzlePieceObject>();
        if (otherPiece != null && otherPiece.tag == "Hole")
        {
            SelectionController._SelectionController.Subtract(this.gameObject, otherPiece.gameObject);
        }
    }



}