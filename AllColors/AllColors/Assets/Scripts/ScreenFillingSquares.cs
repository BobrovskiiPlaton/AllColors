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
    private float regenerateTime = 2f;
    private List<GameObject> squares = new List<GameObject>();
    private Color currentColor;
    private bool squaresFloating = false;

    void Start()
    {
        StartCoroutine(CycleSquares());
    }

    void Update()
    {
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

                if (squaresFloating)
                {
                    Rigidbody2D rb = newSquare.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0f;  // Убираем влияние гравитации
                    rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Улучшенная детекция столкновений
                    rb.constraints = RigidbodyConstraints2D.None; // Убираем все ограничения
                    BoxCollider2D collider = newSquare.GetComponent<BoxCollider2D>();
                    if (collider == null)
                    {
                        collider = newSquare.AddComponent<BoxCollider2D>();
                    }
                    collider.isTrigger = false; // Квадраты могут пересекаться и сталкиваться
                    Vector2 randomDirection = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                    rb.velocity = randomDirection;
                }
            }
        }
    }

    IEnumerator KeepOneColor()
    {
        ActivateColliders();

        float elapsedTime = 0f;
        while (elapsedTime < waitTime)
        {
            if(_colorPicker2D.pointedColor == Color.black)
                GameOver();
            elapsedTime += Time.deltaTime;
            timeSlider.value = elapsedTime / waitTime;
            yield return null;
        }

        DeactivateColliders();

        foreach (GameObject square in squares)
        {
            if (currentColor != square.GetComponent<SpriteRenderer>().color)
            {
                Destroy(square);
            }
        }
        squares.RemoveAll(s => s == null || s.GetComponent<SpriteRenderer>().color != currentColor);

    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        overScreen.SetActive(true);
    }

    private void ActivateColliders()
    {
        foreach (var square in squares)
        {
            var collider = square.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = square.AddComponent<BoxCollider2D>();
            }
            collider.isTrigger = true; // Активируем триггер
        }
    }

    private void DeactivateColliders()
    {
        foreach (var square in squares)
        {
            var collider = square.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                Destroy(collider); // Удаляем коллайдер
            }
        }
    }

    IEnumerator CycleSquares()
    {
        while (true)
        {
            int currentScore = int.Parse(score.text);
            if (currentScore >= 10)
            {
                squaresFloating = true;
            }

            GenerateSquares();

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
            if (currentColor == _colorPicker2D.pointedColor)
            {
                score.text = (int.Parse(score.text) + 1).ToString();
                scoreAudio.Play(); // Воспроизводим звук при наборе очков

                if (waitTime > 1f)
                    waitTime -= 0.1f;
                else
                    waitTime = 0.1f;
            }
            else
            {
                GameOver();
                yield break;
            }
            //yield return new WaitForSeconds(regenerateTime);

            foreach (GameObject square in squares)
            {
                if (square != null)
                    Destroy(square);
            }
            squares.Clear();
        }
    }
}
