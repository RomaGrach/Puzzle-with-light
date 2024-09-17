using UnityEngine;

public class playerRotation : MonoBehaviour
{
    public Camera mainCamera; // Камера, из которой будет исходить луч
    public string[] selectableTags; // Список тегов, которые можно выбирать
    private GameObject selectedObject; // Выбранный объект
    private Vector2 lastTouchPosition; // Последняя позиция касания

    void Update()
    {
        // Проверка на нажатие мыши или касание экрана
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Проверка, есть ли тег объекта в списке тегов
                foreach (string tag in selectableTags)
                {
                    //Debug.Log(hit.collider.tag);
                    if (hit.collider.tag == tag)
                    {
                        //Debug.Log("1");
                        selectedObject = hit.collider.gameObject;
                        lastTouchPosition = Input.mousePosition;
                        break;
                    }
                }
            }
        }

        // Проверка на движение мыши или пальца
        if (Input.GetMouseButton(0) && selectedObject != null)
        {
            Vector2 currentTouchPosition = Input.mousePosition;
            Vector2 delta = currentTouchPosition - lastTouchPosition;

            // Поворот объекта в зависимости от направления движения пальца
            if (delta.x != 0)
            {
                float rotationAmount = delta.x * 0.1f; // Коэффициент для регулировки скорости поворота
                selectedObject.transform.Rotate(Vector3.up, -rotationAmount);
            }

            lastTouchPosition = currentTouchPosition;
        }

        // Сброс выбранного объекта при отпускании мыши или пальца
        if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
        }
    }
}