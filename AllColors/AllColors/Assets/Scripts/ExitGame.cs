using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Публичный метод, который будет вызываться при нажатии на кнопку
    public void OnExitButtonPress()
    {
        // Выход из игры
        Application.Quit();
        
        // Если вы тестируете в редакторе Unity, добавьте следующую строку для остановки режима игры
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
