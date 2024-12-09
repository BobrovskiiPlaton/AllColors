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
    public AudioSource scoreAudio;   // Ссылка на источник аудио

    [SerializeField] private TMP_Text colorRule;
    [SerializeField] private TMP_Text score;
    [SerializeField] private Slider timeSlider;
    [SerializeField] private GameObject overScreen;

    private float waitTime = 3f;
    private List<GameObject> squares = new List<GameObject>();
    private Color currentColor;
    private bool squaresFloating = false;

    void Start()
    {
        StartCoroutine(CycleSquares());
    }
    
    

    IEnumerator CycleSquares()
    {
        while (true)
        {
            GenerateField();
            UpdateRule();

            float elapsedTime = 0f;
            while (elapsedTime < waitTime)
            {
                elapsedTime += Time.deltaTime;
                timeSlider.value = elapsedTime / waitTime;
                yield return null;
            }
            
            if (currentColor != _colorPicker2D.pointedColor)
            {
                GameOver();
                yield break;
                
            }
            AddPoint();
            DecreaseTimer();
            DeleteField();
        }
    }

    private void AddPoint()
    {
        score.text = (int.Parse(score.text) + 1).ToString();
        scoreAudio.Play(); // Воспроизводим звук при наборе очков
    }

    private void UpdateRule()
    {
        int currentScore = int.Parse(score.text);
        currentColor = colors[Random.Range(0, colors.Length)];
        if (currentScore >= 10)
        {
            squaresFloating = true;
        }

        if (currentScore >= 5)
        {
            var randomColorName = ColorToText.GetRandomColorName();
            colorRule.color = currentColor;
            colorRule.text = randomColorName;
        }
        else
        {
            colorRule.color = currentColor;
            colorRule.text = ColorToText.ColorToName(currentColor);
        }
    }

    private void DecreaseTimer()
    {
        if (waitTime > 1f)
            waitTime -= 0.1f;
        else
            waitTime = 1f;
    }

    
    void GenerateField()
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
                
                if (newSquare.GetComponent<SpriteRenderer>() != null && colors.Length > 0)
                {
                    newSquare.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];
                }

                if (squaresFloating)
                {
                    Vector2 randomDirection = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
                    newSquare.GetComponent<Rigidbody2D>().velocity = randomDirection;
                }
            }
        }
    }
    private void DeleteField()
    {
        foreach (GameObject square in squares)
        {
            if (square != null)
                Destroy(square);
        }
        squares.Clear();
    }

    private void GameOver()
    {
        ShowCorrectColor();
        Time.timeScale = 0f;
        overScreen.SetActive(true);
    }

    private void ShowCorrectColor()
    {
        foreach (GameObject square in squares)
        {
            if (currentColor != square.GetComponent<SpriteRenderer>().color)
            {
                Destroy(square);
            }
        }
        squares.RemoveAll(s => s == null || s.GetComponent<SpriteRenderer>().color != currentColor);
    }
}
