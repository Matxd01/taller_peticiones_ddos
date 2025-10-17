using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ApiClient : MonoBehaviour
{
    [Tooltip("Set this in the inspector (e.g. http://192.168.42.21:5005/server)")]
    public string baseUrl = "http://localhost:5005/server";

    // Kept because GameManager subscribes to this event.
    public event Action<int, ServerData> OnDataReceived;

    [Header("Auto Send Settings")]
    public string autoGameId = "defaultGame";
    public string autoPlayerId = "0";
    [Tooltip("Optional; assign for stable behavior. If null, a PlayerController will be searched once.")]
    public Transform playerTransform;
    [Tooltip("Seconds between automatic POSTs.")]
    public float autoInterval = 1f;

    private Coroutine _autoSendCoroutine;
    private PlayerController _cachedPlayerController;
    private WaitForSeconds _waitAuto;

    private void Start()
    {
        if (autoInterval <= 0f) autoInterval = 1f; // safety clamp
        _waitAuto = new WaitForSeconds(autoInterval);
        CachePlayerControllerOnce();
        _autoSendCoroutine = StartCoroutine(AutoSendRoutine());
    }

    private void OnDisable()
    {
        if (_autoSendCoroutine != null)
        {
            StopCoroutine(_autoSendCoroutine);
            _autoSendCoroutine = null;
        }
    }

    // Automatic loop: builds data and posts every autoInterval seconds.
    private IEnumerator AutoSendRoutine()
    {
        while (true)
        {
            Vector3 pos = ResolvePlayerPosition();

            var data = new ServerData
            {
                posX = pos.x,
                posY = pos.y,
                posZ = pos.z
            };

            StartCoroutine(PostPlayerData(autoGameId, autoPlayerId, data));
            Debug.Log($"Auto POST → {ComposeUrl(autoGameId, autoPlayerId)}");

            yield return _waitAuto;
        }
    }

    public IEnumerator GetPlayerData(string gameId, string playerId)
    {
        string url = ComposeUrl(gameId, playerId);

        using (var webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"GET Error: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"GET Success: {webRequest.downloadHandler.text}");
                var data = JsonUtility.FromJson<ServerData>(webRequest.downloadHandler.text);

                if (int.TryParse(playerId, out int pid))
                    OnDataReceived?.Invoke(pid, data);
                else
                    Debug.LogWarning($"Invalid playerId '{playerId}' for OnDataReceived.");
            }
        }
    }

    // Single source of truth for POST behavior.
    public IEnumerator PostPlayerData(string gameId, string playerId, ServerData data)
    {
        string url = ComposeUrl(gameId, playerId);
        string jsonData = JsonUtility.ToJson(data);

        using (var webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"POST Error: {webRequest.error}");
                Debug.LogError($"Response: {webRequest.downloadHandler.text}");
            }
            else
            {
                Debug.Log($"POST Success: {webRequest.downloadHandler.text}");
            }
        }
    }

    // --- Helpers ---

    // Compose endpoint once in a consistent way.
    private string ComposeUrl(string gameId, string playerId)
    {
        return $"{baseUrl}/{gameId}/{playerId}";
    }

    // Resolve player position with minimal lookups.
    private Vector3 ResolvePlayerPosition()
    {
        if (playerTransform != null)
            return playerTransform.position;

        if (_cachedPlayerController != null)
            return _cachedPlayerController.GetPosition();

        CachePlayerControllerOnce();
        return _cachedPlayerController != null ? _cachedPlayerController.GetPosition() : Vector3.zero;
    }

    // Cache PlayerController once to avoid repeated scene searches.
    private void CachePlayerControllerOnce()
    {
        if (_cachedPlayerController == null)
            _cachedPlayerController = FindAnyObjectByType<PlayerController>();
    }
}
