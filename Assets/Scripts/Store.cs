using UnityEngine;

public class Store : MonoBehaviour
{
    public static Store Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoggingService.LogStartGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
