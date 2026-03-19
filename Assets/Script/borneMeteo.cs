using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System;

public class BorneMeteo : MonoBehaviour
{
    public Light sceneLight;
    public TextMeshProUGUI txtVille;
    public TextMeshProUGUI txtDate;
    public TextMeshProUGUI txtTemp;
    public TextMeshProUGUI txtMeteo;
    public TextMeshProUGUI txtVent;
    public ParticleSystem rain;
    public ParticleSystem snow;
    public AudioSource rainAudio;
    public AudioSource button;

    [HideInInspector] public int jourSelectionne = 0;
    float latitude = 48.42f;
    float longitude = -71.06f;
    string nomVille = "Chicoutimi (Canada)";

    public void AujourdHui()
    {
        jourSelectionne = 0;
        StartCoroutine(AppelAPI());
    }

    public void JourSuivant()
    {
        jourSelectionne++;
        if (jourSelectionne > 6)
            jourSelectionne = 0;
        StartCoroutine(AppelAPI());
    }

    public void JourPrecedent()
    {
        jourSelectionne--;
        if (jourSelectionne < 0)
            jourSelectionne = 6;
        StartCoroutine(AppelAPI());
    }

    public void VilleChicoutimi()
    {
        latitude = 48.42f;
        longitude = -71.06f;
        nomVille = "Chicoutimi (Canada)";
        jourSelectionne = 0;
        StartCoroutine(AppelAPI());
    }

    public void VilleDax()
    {
        latitude = 43.71f;
        longitude = -1.05f;
        nomVille = "Dax (France)";
        jourSelectionne = 0;
        StartCoroutine(AppelAPI());
    }

    public void VilleRambouillet()
    {
        latitude = 48.65f;
        longitude = 1.83f;
        nomVille = "Rambouillet (France)";
        jourSelectionne = 0;
        StartCoroutine(AppelAPI());
    }

    public IEnumerator AppelAPI()
    {
        button.Play();

        string url = "https://api.open-meteo.com/v1/forecast?latitude="
                        + latitude +
                        "&longitude=" + longitude +
                        "&hourly=temperature_2m,weathercode,windspeed_10m&timezone=auto";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            WeatherData data = JsonUtility.FromJson<WeatherData>(request.downloadHandler.text);

            int heureActuelle = System.DateTime.Now.Hour;
            int index = jourSelectionne * 24 + heureActuelle;

            float temp = data.hourly.temperature_2m[index];
            float wind = data.hourly.windspeed_10m[index];
            int code = data.hourly.weathercode[index];
            DateTime dateHeure = DateTime.Parse(data.hourly.time[index]);
            string[] jours = { "Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi" };
            string[] mois = { "janvier", "février", "mars", "avril", "mai", "juin", "juillet", "août", "septembre", "octobre", "novembre", "décembre" };
            string date = jours[(int)dateHeure.DayOfWeek] + " " + dateHeure.Day + " " + mois[dateHeure.Month - 1] + " " + dateHeure.Year + " - " + dateHeure.Hour + "h00";

            UpdateLight(code);
            UpdateParticle(code);

            string meteo = GetWeatherName(code);

            txtVille.text = nomVille;
            txtDate.text = date;
            txtTemp.text = temp + "°C";
            txtMeteo.text = meteo;
            txtVent.text = "Vent : " + wind + " km/h";
        }
        else
        {
            txtVille.text = "Erreur météo";
            txtDate.text = "";
            txtTemp.text = "";
            txtMeteo.text = "";
            txtVent.text = "";
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
        if (code >= 80 && code <= 82) return "Averses de pluie";
        if (code >= 85 && code <= 86) return "Averses de neige";
        if (code >= 95) return "Orage";
        return "Inconnu";
    }

    void UpdateLight(int code)
    {
        if (sceneLight == null) return;
        if (code == 0) sceneLight.intensity = 1.5f;
        else if (code <= 3) sceneLight.intensity = 1.0f;
        else if (code <= 60) sceneLight.intensity = 0.6f;
        else if (code <= 80) sceneLight.intensity = 0.4f;
        else sceneLight.intensity = 0.2f;
    }

    void UpdateParticle(int code)
    {
        if (rain != null) rain.Stop();
        if (snow != null) snow.Stop();

        if (code >= 51 && code <= 67) { rain.Play(); rainAudio.Play(); }
        else if (code >= 71 && code <= 77) { snow.Play(); rainAudio.Stop(); }
        else if (code >= 80 && code <= 82) { rain.Play(); rainAudio.Play(); }
        else if (code >= 85 && code <= 86) { snow.Play(); rainAudio.Stop(); }
        else if (code >= 95) { rain.Play(); rainAudio.Play(); }
        else { rainAudio.Stop(); }
    }
}

[System.Serializable]
public class WeatherData
{
    public Hourly hourly;
}

[System.Serializable]
public class Hourly
{
    public string[] time;
    public float[] temperature_2m;
    public int[] weathercode;
    public float[] windspeed_10m;
}