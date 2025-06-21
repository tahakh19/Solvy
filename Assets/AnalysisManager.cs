using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic; // Required for Dictionary
using UnityEngine.UI; // Required for UI elements

// Attach this to a manager GameObject in your Analysis Scene.
public class AnalysisManager : MonoBehaviour
{
    [Header("Server Settings")]
    [Tooltip("The URL of your Python server's analysis endpoint.")]
    public string serverURL = "http://localhost:5000/api/analyze";

    [Header("UI References")]
    [Tooltip("The parent object where the level analysis grids will be created.")]
    public Transform analysisPanelContainer;

    [Tooltip("A prefab for a single UI grid cell. Should have an Image component.")]
    public GameObject gridCellPrefab;

    [Tooltip("A prefab for the title of each level analysis.")]
    public GameObject levelTitlePrefab;

    void Start()
    {
        StartCoroutine(GetAnalysisFromServer());
    }

    private IEnumerator GetAnalysisFromServer()
    {
        // --- 1. Find and Read the User's JSON File ---
        string username = GameSession.Username ?? "DefaultUser";
        string filePath = Path.Combine(@"C:\Users\lenovo\Adhd_game_kharazmi_teh\data_json", $"{username}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Analysis Error: Save file not found for user '{username}' at path: {filePath}");
            yield break;
        }

        string jsonData = File.ReadAllText(filePath);

        // --- 2. Send the JSON Data to the Server ---
        using (UnityWebRequest request = new UnityWebRequest(serverURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"Sending data for user '{username}' to server...");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Server Error: {request.error}");
                yield break;
            }
            
            Debug.Log("Server Response Received!");
            // --- 3. Process the Server's Response ---
            // We need a helper class to parse the nested JSON from the server.
            AnalysisResponse response = JsonUtility.FromJson<AnalysisResponse>(WrapJson(request.downloadHandler.text));
            DisplayAnalysis(response);
        }
    }

    /// <summary>
    /// Creates the UI grids to display the analysis data.
    /// </summary>
    private void DisplayAnalysis(AnalysisResponse response)
    {
        foreach (var levelAnalysis in response.levels)
        {
            // Create a title for the level
            GameObject titleObj = Instantiate(levelTitlePrefab, analysisPanelContainer);
            titleObj.GetComponent<Text>().text = $"Level {levelAnalysis.grid_size - 2} Analysis";

            // Create a panel to hold the grid
            GameObject gridPanel = new GameObject("GridPanel", typeof(RectTransform));
            gridPanel.transform.SetParent(analysisPanelContainer, false);
            GridLayoutGroup gridLayout = gridPanel.AddComponent<GridLayoutGroup>();
            
            // Configure the grid layout
            int gridSize = levelAnalysis.grid_size;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = gridSize;
            gridLayout.cellSize = new Vector2(50, 50); // Adjust cell size as needed
            gridLayout.spacing = new Vector2(5, 5);

            // Create the grid cells
            for (int i = 1; i <= gridSize * gridSize; i++)
            {
                GameObject cell = Instantiate(gridCellPrefab, gridPanel.transform);
                Image cellImage = cell.GetComponent<Image>();
                cellImage.color = Color.white; // Default color

                // Check if this cell is part of the analyzed path
                if (levelAnalysis.colors.ContainsKey(i.ToString()))
                {
                    Color pathColor;
                    if (ColorUtility.TryParseHtmlString(levelAnalysis.colors[i.ToString()], out pathColor))
                    {
                        cellImage.color = pathColor;
                    }
                }
            }
        }
    }
    
    // Helper function to make the server's JSON parsable by Unity's simple JsonUtility
    private string WrapJson(string json)
    {
        return "{\"levels\":" + json.Replace("\"level_1\":", "").Replace("\"level_2\":", "").Replace("\"level_3\":", "").Replace("\"level_4\":", "").Replace("\"level_5\":", "").Trim('{').Trim('}') + "}";
    }
}

// --- Helper classes for parsing server JSON response ---
[System.Serializable]
public class LevelAnalysis
{
    public int grid_size;
    public SerializableDictionary<string, int> path_order;
    public SerializableDictionary<string, string> colors;
}

[System.Serializable]
public class AnalysisResponse
{
    public List<LevelAnalysis> levels;
}

// Unity's JsonUtility can't serialize dictionaries directly, so we need a custom wrapper.
[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
            throw new System.Exception("There are unequal numbers of keys and values");

        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}
