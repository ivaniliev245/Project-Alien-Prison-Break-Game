using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;

    private void Awake()
    {
        instance = this;
    }

    public static CoroutineManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CoroutineManager");
                instance = go.AddComponent<CoroutineManager>();
            }

            return instance;
        }
    }

    public Coroutine StartRoutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }
}
