using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ApiClient api;

    private void Awake()
    {
        if (api == null) api = FindAnyObjectByType<ApiClient>();
    }

    private void OnEnable()
    {
        if (api != null) api.OnDataReceived += OnData;
    }

    private void OnDisable()
    {
        if (api != null) api.OnDataReceived -= OnData;
    }

    private void OnData(int playerId, ServerData data)
    {
        Debug.Log($"Data received → playerId={playerId} pos={data}");
    }
}
