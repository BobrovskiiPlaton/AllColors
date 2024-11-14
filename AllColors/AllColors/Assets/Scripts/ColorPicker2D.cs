using UnityEngine;

public class ColorPicker2D : MonoBehaviour
{
    public Camera renderCamera;

    void Start()
    {
        if (renderCamera == null)
        {
            renderCamera = Camera.main;
        }
    }

    void Update()
    {
        // Получаем позицию курсора мыши
        Vector3 mousePos = renderCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Проверяем, находится ли курсор над объектом
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // Получаем компонент SpriteRenderer
            SpriteRenderer spriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                Texture2D texture = spriteRenderer.sprite.texture;

                if (!texture.isReadable)
                {
                    Debug.LogError("Texture is not readable. Ensure 'Read/Write Enabled' is checked in the texture import settings.");
                    return;
                }

                // Преобразование мировых координат в координаты текстуры
                Vector2 localPos = hit.point - (Vector2)spriteRenderer.transform.position;
                float ppu = spriteRenderer.sprite.pixelsPerUnit;
                int x = Mathf.FloorToInt(localPos.x * ppu + texture.width / 2);
                int y = Mathf.FloorToInt(localPos.y * ppu + texture.height / 2);

                if (x >= 0 && x < texture.width && y >= 0 && y < texture.height)
                {
                    Color pixelColor = texture.GetPixel(x, y);
                    Color finalColor = pixelColor * spriteRenderer.color;
                    Debug.Log("Цвет под курсором: " + finalColor);
                }
                else
                {
                    Debug.Log("Координаты курсора вне границ текстуры");
                }
            }
        }
    }
}