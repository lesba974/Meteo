using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System;
using System.Globalization;

public class BorneMeteo : MonoBehaviour
{
    public Light sceneLight;
    public Light lightningLight;
    public TextMeshProUGUI txtVille;
    public TextMeshProUGUI txtDate;
    public TextMeshProUGUI txtTemp;
    public TextMeshProUGUI txtMeteo;
    public TextMeshProUGUI txtVent;
    public ParticleSystem rain;
    public ParticleSystem snow;
    public ParticleSystem clouds;
    public AudioSource rainAudio;
    public AudioSource thunderAudio;
    public AudioSource button;

    [HideInInspector] public int jourSelectionne = 0;
    float latitude = 48.42f;
    float longitude = -71.06f;
    string nomVille = "Chicoutimi (Canada)";

    bool eclairActif = false; 

    void Start()
    {
        if (clouds != null) clouds.Stop();
        if (rain != null) rain.Stop();
        if (snow != null) snow.Stop();
        if (lightningLight != null) lightningLight.enabled = false;
        RenderSettings.fog = false;
    }

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

    IEnumerator Eclair()
    {
        eclairActif = true;
        while (eclairActif)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 8f));
            int flashs = UnityEngine.Random.Range(2, 4);
            for (int i = 0; i < flashs; i++)
            {
                if (lightningLight != null) lightningLight.enabled = true;
                yield return new WaitForSeconds(0.3f);
                if (lightningLight != null) lightningLight.enabled = false;
                yield return new WaitForSeconds(0.25f);
            }
        }
    }

    void ArreterEclair()
    {
        eclairActif = false;
        if (lightningLight != null) lightningLight.enabled = false;
        StopCoroutine("Eclair");
    }

    public IEnumerator AppelAPI()
    {
        button.Play();

        string url = "https://api.open-meteo.com/v1/forecast?latitude="
                        + latitude.ToString(CultureInfo.InvariantCulture) +
                        "&longitude=" + longitude.ToString(CultureInfo.InvariantCulture) +
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
            UpdateFog(code);

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
        if (code == 0) { sceneLight.intensity = 1.5f; sceneLight.color = new Color(1f, 0.95f, 0.8f); }
        else if (code >= 1 && code <= 3) { sceneLight.intensity = 1.0f; sceneLight.color = new Color(0.7f, 0.75f, 0.85f); }
        else if (code >= 45 && code <= 48) { sceneLight.intensity = 0.5f; sceneLight.color = new Color(0.6f, 0.65f, 0.7f); }
        else if (code >= 51 && code <= 67) { sceneLight.intensity = 0.6f; sceneLight.color = new Color(0.6f, 0.65f, 0.75f); }
        else if (code >= 71 && code <= 77) { sceneLight.intensity = 0.6f; sceneLight.color = new Color(0.8f, 0.88f, 1f); }
        else if (code >= 80 && code <= 82) { sceneLight.intensity = 0.4f; sceneLight.color = new Color(0.5f, 0.55f, 0.65f); }
        else if (code >= 85 && code <= 86) { sceneLight.intensity = 0.4f; sceneLight.color = new Color(0.7f, 0.78f, 0.9f); }
        else if (code >= 95) { sceneLight.intensity = 0.2f; sceneLight.color = new Color(0.4f, 0.4f, 0.5f); }
    }

    void UpdateParticle(int code)
    {
        if (rain != null) rain.Stop();
        if (snow != null) snow.Stop();
        if (clouds != null) clouds.Stop();
        ArreterEclair(); 

        if (code >= 1 && code <= 3) { clouds.Play(); }
        else if (code >= 51 && code <= 67) { rain.Play(); rainAudio.Play(); thunderAudio.Stop(); }
        else if (code >= 71 && code <= 77) { snow.Play(); rainAudio.Stop(); thunderAudio.Stop(); }
        else if (code >= 80 && code <= 82) { rain.Play(); rainAudio.Play(); thunderAudio.Stop(); }
        else if (code >= 85 && code <= 86) { snow.Play(); rainAudio.Stop(); thunderAudio.Stop(); }
        else if (code >= 95)
        {
            rain.Play();
            rainAudio.Stop();
            thunderAudio.Play();
            StartCoroutine("Eclair"); 
        }
        else { rainAudio.Stop(); thunderAudio.Stop(); }
    }

    void UpdateFog(int code)
    {
        if (code >= 45 && code <= 48)
        {
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.15f;
        }
        else
        {
            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0f;
        }
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