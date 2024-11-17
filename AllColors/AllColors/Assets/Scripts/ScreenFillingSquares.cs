using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ScreenFillingSquares : MonoBehaviour
{
    public GameObject squarePrefab;  // Префаб квадрата
    public Color[] colors;           // Массив цветов
    public ColorPicker2D _colorPicker2D;

    [SerializeField] private TMP_Text colorRule;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject overScreen;

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
        float screenWidth = 2 * Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = 2 * Camera.main.orthographicSize;
        float squareSize = Mathf.Min(screenWidth, screenHeight) / 10;

        int squaresX = Mathf.CeilToInt(screenWidth / squareSize);
        int squaresY = Mathf.CeilToInt(screenHeight / squareSize);

        for (int x = 0; x < squaresX; x++)
        {
            for (int y = 1; y < squaresY - 1; y++)
            {
                Vector2 position = new Vector2(-screenWidth / 2 + x * squareSize + squareSize / 2, -screenHeight / 2 + y * squareSize + squareSize / 2);
                GameObject newSquare = Instantiate(squarePrefab, position, Quaternion.identity);
                newSquare.transform.localScale = new Vector3(squareSize, squareSize, 1);
                squares.Add(newSquare);

                SpriteRenderer spriteRenderer = newSquare.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null && colors.Length > 0)
                {
                    spriteRenderer.color = colors[Random.Range(0, colors.Length)];
                }
            }
        }
    }

    IEnumerator KeepOneColor()
    {
        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            elapsedTime += Time.deltaTime;
            timeSlider.value = elapsedTime / waitTime;
            yield return null;
        }

        foreach (GameObject square in squares)
        {
            if (currentColor != square.GetComponent<SpriteRenderer>().color)
            {
                Destroy(square);
            }
        }
        squares.RemoveAll(s => s == null || s.GetComponent<SpriteRenderer>().color != currentColor);
        Debug.Log(_colorPicker2D.pointedColor);
        if (currentColor == _colorPicker2D.pointedColor)
        {
            score.text = (int.Parse(score.text) + 1).ToString();
            if (waitTime > 0.1f)
                waitTime -= 0.1f;
            else
                waitTime = 0.1f;
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        overScreen.SetActive(true);
    }

    IEnumerator CycleSquares()
    {
        while (true)
        {
            GenerateSquares();

            int currentScore = int.Parse(score.text);
            currentColor = colors[Random.Range(0, colors.Length)];

            if (currentScore < 5)
            {
                colorRule.color = currentColor;
                colorRule.text = ColorToText.ColorToName(currentColor);
            }
            else
            {
                var randomColorName = ColorToText.GetRandomColorName();
                colorRule.color = currentColor;
                colorRule.text = randomColorName;
            }

            yield return StartCoroutine(KeepOneColor());

            yield return new WaitForSeconds(regenerateTime);

            foreach (GameObject square in squares)
            {
                if (square != null)
                    Destroy(square);
            }
            squares.Clear();
        }
    }
}
