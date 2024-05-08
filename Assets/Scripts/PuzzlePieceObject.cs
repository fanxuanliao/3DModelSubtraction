using UnityEngine;

public class PuzzlePieceObject : MonoBehaviour
{
    Vector3 m_Offset;
    float m_ZCoord;

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


    void OnTriggerStay(Collider other)
    {
        if (other != null && other.tag == "Hole")
        {
            SelectionController._SelectionController.Subtract(this.gameObject, other.gameObject);
        }
    }



}