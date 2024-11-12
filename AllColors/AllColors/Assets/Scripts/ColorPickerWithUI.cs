using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ColorPickerWithUI : MonoBehaviour
{
    public Camera renderCamera;
    public RenderTexture renderTexture;
    public GraphicRaycaster uiRaycaster;
    public Canvas canvas;

    void Start()
    {
        if (renderCamera == null)
        {
            renderCamera = Camera.main;
        }

        if (uiRaycaster == null)
        {
            if (canvas != null)
            {
                uiRaycaster = canvas.GetComponent<GraphicRaycaster>();
                if (uiRaycaster == null)
                {
                    Debug.LogError("GraphicRaycaster not found on the assigned Canvas.");
                }
            }
            else
            {
                Debug.LogError("Canvas is not assigned.");
            }
        }
    }

    void Update()
    {
        // Получаем координаты мыши
        Vector3 mousePos = Input.mousePosition;

        if (renderCamera == null || uiRaycaster == null)
        {
            Debug.LogError("RenderCamera or GraphicRaycaster is not assigned.");
            return;
        }

        // Проверяем UI элементы под курсором
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);

        if (results.Count > 0)
        {
            foreach (RaycastResult result in results)
            {
                Image image = result.gameObject.GetComponent<Image>();
                if (image != null)
                {
                    Texture2D texture = image.sprite.texture;
                    RectTransform rt = image.GetComponent<RectTransform>();
                    Vector2 localCursor;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, mousePos, renderCamera, out localCursor))
                    {
                        Rect rect = rt.rect;
                        float xImage = (localCursor.x - rect.x) / rect.width;
                        float yImage = (localCursor.y - rect.y) / rect.height;

                        int x = Mathf.FloorToInt(xImage * texture.width);
                        int y = Mathf.FloorToInt(yImage * texture.height);

                        if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                        {
                            Color color = texture.GetPixel(x, y);
                            Debug.Log("Цвет под курсором (UI): " + color);
                        }
                    }
                    return; // Выход из метода после нахождения UI элемента
                }
            }
        }

        // Если курсор не находится на UI элементе, проверяем сцену
        if (renderTexture == null)
        {
            renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            renderCamera.targetTexture = renderTexture;
        }

        renderCamera.Render();

        Texture2D sceneTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        sceneTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        sceneTexture.Apply();
        RenderTexture.active = null;

        int xScene = Mathf.FloorToInt(mousePos.x * sceneTexture.width / Screen.width);
        int yScene = Mathf.FloorToInt(mousePos.y * sceneTexture.height / Screen.height);

        if (xScene >= 0 && xScene < sceneTexture.width && yScene >= 0 && yScene < sceneTexture.height)
        {
            Color color = sceneTexture.GetPixel(xScene, yScene);
            Debug.Log("Цвет под курсором (сцена): " + color);
        }
        else
        {
            Debug.Log("Координаты курсора вне границ текстуры");
        }

        Destroy(sceneTexture);
    }
}
