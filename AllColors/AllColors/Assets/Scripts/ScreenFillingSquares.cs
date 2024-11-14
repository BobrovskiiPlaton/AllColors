using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenFillingSquares : MonoBehaviour
{
    public GameObject squarePrefab;  // Префаб квадрата
    public Color[] colors;           // Массив цветов
    
    private float waitTime = 2f;
    private float regenerateTime = 2f;
    private List<GameObject> squares = new List<GameObject>();
    private Color currentColor;

    void Start()
    {
        StartCoroutine(CycleSquares());

    }



    void GenerateSquares()
    {
        // Получаем размеры экрана в мировых координатах
        float screenWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = 2 * Camera.main.orthographicSize;

        // Размер квадрата (высота и ширина должны быть одинаковыми)
        float squareSize = Mathf.Min(screenWidth, screenHeight) / 10;  // Количество квадратов в ряд/столбец

        // Определяем количество квадратов по ширине и высоте
        int squaresX = Mathf.CeilToInt(screenWidth / squareSize);
        int squaresY = Mathf.CeilToInt(screenHeight / squareSize);

        for (int x = 0; x < squaresX; x++)
        {
            for (int y = 0; y < squaresY; y++)
            {
                // Позиция для квадрата
                Vector2 position = new Vector2(-screenWidth / 2 + x * squareSize + squareSize / 2, -screenHeight / 2 + y * squareSize + squareSize / 2);

                // Создание квадрата
                GameObject newSquare = Instantiate(squarePrefab, position, Quaternion.identity);
                newSquare.transform.localScale = new Vector3(squareSize, squareSize, 1);  // Установка размера квадрата
                squares.Add(newSquare);

                // Назначение случайного цвета из массива
                SpriteRenderer spriteRenderer = newSquare.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && colors.Length > 0)
                {
                    spriteRenderer.color = colors[Random.Range(0, colors.Length)];
                }
            }
        }
    }
    IEnumerator KeepOneColor( )
    {
        yield return new WaitForSeconds(waitTime);
        currentColor = colors[Random.Range(0, colors.Length)];
        foreach (GameObject square in squares)
        {
            if (currentColor != square.GetComponent<SpriteRenderer>().color)
            {
                Destroy(square);
            }
        }
        squares.RemoveAll(s => s == null || s.GetComponent<SpriteRenderer>().color != currentColor);
        yield return new WaitForSeconds(2f);
    }
    IEnumerator CycleSquares()
    {
        while (true)
        {
            GenerateSquares();

            currentColor = colors[Random.Range(0, colors.Length)];
            yield return StartCoroutine(KeepOneColor());

            yield return new WaitForSeconds(regenerateTime);

            foreach (GameObject square in squares)
            {
                if(square != null)
                    Destroy(square);
            }
            squares.Clear();
        }
    }
}