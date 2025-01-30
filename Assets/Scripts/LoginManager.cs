using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    private string firebaseAPIKey = "AIzaSyCzrN8rWsQEgcix6ebKayhk9w38wBDZyUE"; // Firebase API Anahtarı

    [Header("UI Elements")]
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public TMP_Text statusText;

    private string idToken; // Firebase JWT Token

    // Kullanıcı Kaydı
    public void RegisterUser()
    {
        StartCoroutine(RegisterCoroutine(emailInputField.text, passwordInputField.text));
    }

    // Kullanıcı Girişi
    public void LoginUser()
    {
        StartCoroutine(LoginCoroutine(emailInputField.text, passwordInputField.text));
    }

    // Kullanıcı Kaydı Coroutine
    private IEnumerator RegisterCoroutine(string email, string password)
    {
        string registerURL = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={firebaseAPIKey}";
        string jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        using (UnityWebRequest request = new UnityWebRequest(registerURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Registration Successful: " + request.downloadHandler.text);
                statusText.text = "Registration Successful!";
            }
            else
            {
                Debug.LogError("Registration Error: " + request.error);
                statusText.text = "Registration failed!";
            }
        }
    }

    // Kullanıcı Girişi Coroutine
    private IEnumerator LoginCoroutine(string email, string password)
    {
        string loginURL = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseAPIKey}";

        string jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        using (UnityWebRequest request = new UnityWebRequest(loginURL, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("Login Successful: " + responseText);
                statusText.text = "Login Successful!";
                
                // JSON verisinden idToken almak için
                var response = JsonUtility.FromJson<FirebaseAuthResponse>(responseText);
                idToken = response.idToken;

                // Kullanıcı oturumunu kaydet (Oyun kapansa bile hatırlamak için)
                PlayerPrefs.SetString("user_id", response.localId);
                PlayerPrefs.SetString("id_token", response.idToken);
                PlayerPrefs.SetString("email", email);
                PlayerPrefs.Save(); // Veriyi kalıcı yap
                
                // Oyun sahnesine geç
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
            else
            {
                Debug.LogError("Login Error: " + request.error);
                statusText.text = "Login Error!";
            }
        }
    }

    // Firebase JSON Yanıtını almak için model sınıfı
    [System.Serializable]
    private class FirebaseAuthResponse
    {
        public string idToken;
        public string localId;
    }

}
