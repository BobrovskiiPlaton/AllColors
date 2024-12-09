using UnityEngine;
using System.Collections.Generic;

public class PersistentObjectsManager : MonoBehaviour
{
    public List<GameObject> persistentObjects;

    private void Awake()
    {
        foreach (GameObject obj in persistentObjects)
        {
            DontDestroyOnLoad(obj);
        }
    }
}