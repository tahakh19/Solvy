// WebViewManager.cs
using UnityEngine;

// This script uses the free unity-webview asset.
// Make sure you have downloaded and imported the .unitypackage into your project first.
public class WebViewManager : MonoBehaviour
{
    // The URL of your running Flask application.
    // Make sure your Flask server is running and accessible at this address.
    public string Url = "http://127.0.0.1:5000";

    void Start()
    {
        // Create a new WebViewObject and attach it to this GameObject.
        // The `WebViewObject` comes from the 'unity-webview' asset.
        var webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        
        // Initialize the web view. The callback function is useful for debugging
        // web view events in the Unity console.
        webViewObject.Init((msg) => {
            Debug.Log($"WebView Message: {msg}");
        });

        // Set the margins to 0 pixels from each edge (left, top, right, bottom)
        // to make the web view appear full screen.
        webViewObject.SetMargins(0, 0, 0, 0);

        // Make the web view visible.
        webViewObject.SetVisibility(true);

        // Load the URL from your running Flask server.
        // The Replace function is a safeguard for any accidental spaces.
        webViewObject.LoadURL(Url.Replace(" ", "%20"));
    }
}