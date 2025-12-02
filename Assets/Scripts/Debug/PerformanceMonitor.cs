using System;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class PerformanceStats : MonoBehaviour
{
    // FPS
    public float currentFPS { get; private set; }
    public float averageFPS { get; private set; }
    public float lowestFPS { get; private set; } = float.MaxValue;
    public float highestFPS { get; private set; } = 0f;

    // Frame time
    public float frameTimeMS { get; private set; }

    // Memory (RAM)
    public float allocatedMemoryMB { get; private set; }
    public float reservedMemoryMB { get; private set; }
    public float unusedReservedMemoryMB { get; private set; }

    // Scene & Player info
    public string sceneName { get; private set; }
    public Vector3 playerPosition { get; private set; }

    // Draw Calls & Geometry
    public int drawCalls { get; private set; }
    public int triangles { get; private set; }
    public int vertices { get; private set; }

    // Optional references
    [SerializeField] private Transform player;

    [Header("TMP")]
    [SerializeField] private TMP_Text sceneText;
    [SerializeField] private TMP_Text positionText;

    [SerializeField] private TMP_Text currentFPSText;
    [SerializeField] private TMP_Text avgFPSText;
    [SerializeField] private TMP_Text lowestFPSText;
    [SerializeField] private TMP_Text highestFPSText;
    [SerializeField] private TMP_Text frameTimeMSText;

    [SerializeField] private TMP_Text allocatedMemoryText;
    [SerializeField] private TMP_Text reservedMemoryText;
    [SerializeField] private TMP_Text unUsedReservedMemoryText;

    [SerializeField] private TMP_Text drawCallsText;
    [SerializeField] private TMP_Text trianglesText;
    [SerializeField] private TMP_Text verticesText;

    // Internal
    private float deltaTime = 0f;
    private int frameCount = 0;
    private float totalFPS = 0f;

    // Reset timer
    private float resetTimer = 0f;
    private const float RESET_INTERVAL = 60f; // co 60 sekund

    void LateUpdate()
    {
        // Licznik resetu
        resetTimer += Time.unscaledDeltaTime;
        if (resetTimer >= RESET_INTERVAL)
        {
            ResetStats();
            resetTimer = 0f;
        }

        // FPS & frame time
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        frameTimeMS = deltaTime * 1000f;
        currentFPS = 1f / deltaTime;

        // FPS tracking
        frameCount++;
        totalFPS += currentFPS;
        averageFPS = totalFPS / frameCount;

        if (currentFPS < lowestFPS) lowestFPS = currentFPS;
        if (currentFPS > highestFPS) highestFPS = currentFPS;

        // Memory
        allocatedMemoryMB = Profiler.GetTotalAllocatedMemoryLong() / (1024f * 1024f);
        reservedMemoryMB = Profiler.GetTotalReservedMemoryLong() / (1024f * 1024f);
        unusedReservedMemoryMB = Profiler.GetTotalUnusedReservedMemoryLong() / (1024f * 1024f);

        // Scene info
        sceneName = SceneManager.GetActiveScene().name;

        if (player != null)
            playerPosition = player.position;

        // Render stats
#if UNITY_EDITOR
        drawCalls = UnityEditor.UnityStats.batches;
        triangles = UnityEditor.UnityStats.triangles;
        vertices = UnityEditor.UnityStats.vertices;
#else
        drawCalls = -1;
        triangles = -1;
        vertices = -1;
#endif

        // Aktualizacja TMP
        sceneText.text = sceneName;
        positionText.text = $"X:{MathF.Round(playerPosition.x)} Y:{MathF.Round(playerPosition.y)} Z:{MathF.Round(playerPosition.z)}";
        currentFPSText.text = $"FPS: {MathF.Round(currentFPS)}";
        avgFPSText.text = $"AVG: {MathF.Round(averageFPS)}";
        lowestFPSText.text = $"Low: {MathF.Round(lowestFPS)}";
        highestFPSText.text = $"High: {MathF.Round(highestFPS)}";
        frameTimeMSText.text = $"MS: {MathF.Round(frameTimeMS)}";
        allocatedMemoryText.text = $"M_Al: {MathF.Round(allocatedMemoryMB)}";
        reservedMemoryText.text = $"M_Res: {MathF.Round(reservedMemoryMB)}";
        unUsedReservedMemoryText.text = $"M_Un: {MathF.Round(unusedReservedMemoryMB)}";
        drawCallsText.text = $"Draw: {drawCalls}";
        trianglesText.text = $"Tria: {triangles}";
        verticesText.text = $"Ver: {vertices}";
    }

    private void ResetStats()
    {
        frameCount = 0;
        totalFPS = 0f;
        averageFPS = 0f;
        lowestFPS = float.MaxValue;
        highestFPS = 0f;
    }
}
