using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class BorneMeteo : MonoBehaviour
{
    public Light sceneLight;
    public TextMeshProUGUI texte;
    public ParticleSystem rain;
    public ParticleSystem snow;
    public AudioSource rainAudio;

    public void BoutonPresse()
    {
        Debug.Log("Bouton pressé");
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

            UpdateLight(code);
            UpdateParticle(code);

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

    void UpdateLight(int code)
    {
        if (sceneLight == null) return;

        if (code == 0) sceneLight.intensity = 1.5f;

        if (code <= 3) sceneLight.intensity = 1.0f;

        if (code <= 60) sceneLight.intensity = 0.6f;

        if (code <= 80) sceneLight.intensity = 0.4f;

        else sceneLight.intensity = 0.2f;
    }

    void UpdateParticle(int code)
    {
        if (rain != null) rain.Stop();
        if (snow != null) snow.Stop();

        if (code >= 51 && code <= 67)
        {
            rain.Play();
            rainAudio.Play();
        }

        else if (code >= 71 && code <= 77)
        {
            snow.Play();
            rainAudio.Stop();
        }

        else if (code >= 80 && code <= 82)
        {
            rain.Play();//"Averses";
            rainAudio.Play();
        }

        else if (code >= 95)
        {
            rain.Play(); //"Orage";
            rainAudio.Play();
        }
        else
        {
            rainAudio.Stop();
        }
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
