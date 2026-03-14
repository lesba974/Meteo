using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class BorneMeteo : MonoBehaviour
{
    public TextMeshProUGUI texte;

    public void BoutonPresse()
    {
        StartCoroutine(AppelAPI());
    }

    IEnumerator AppelAPI()
    {
        string url = "https://api.open-meteo.com/v1/forecast?latitude=48.42&longitude=-71.06&current_weather=true";

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

            float temp = data.current_weather.temperature;
            float wind = data.current_weather.windspeed;
            int code = data.current_weather.weathercode;

            string meteo = GetWeatherName(code);

            texte.text =
                "Chicoutimi\n" +
                "Température : " + temp + "°C\n" +
                "Météo : " + meteo + "\n" +
                "Vent : " + wind + "km/h\n";
        }
        else
        {
            texte.text = "Erreur météo";
            Debug.LogError(request.error);
        }
    }

    string GetWeatherName(int code)
    {
        if (code == 0) return "Ciel clair";

        if (code >= 1 && code <= 3) return "Nuageux";

        if (code >= 45 && code <= 48) return "Brouillard";

        if (code >= 51 && code <= 67) return "Pluie";

        if (code >= 71 && code <= 77) return "Neige";

        if (code >= 80 && code <= 82) return "Averses";

        if (code >= 95) return "Orage";

        return "Inconnu";
    }

}

[System.Serializable]
public class WeatherData
{
    public CurrentWeather current_weather;
}

[System.Serializable]
public class CurrentWeather
{
    public float temperature;
    public float windspeed;
    public int weathercode;
}
