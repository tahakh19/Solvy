using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Tooltip("The input field where the user types their name.")]
    public InputField usernameInput;

    [Tooltip("The button the user clicks to log in.")]
    public Button loginButton;

    [Tooltip("The name of your main puzzle scene to load after login.")]
    public string sceneToLoad = "GameScene"; 

    void Start()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(Login);
        }
    }

    public void Login()
    {
        if (!string.IsNullOrEmpty(usernameInput.text))
        {
            GameSession.Username = usernameInput.text;

            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Username is empty. Please enter a username.");
        }
    }
}
