using UnityEngine;
using TMPro;

public class BorneMeteo : MonoBehaviour
{
    public TextMeshProUGUI texte;

    public void BoutonPresse()
    {
        texte.text = "Météo en cours...";
        Debug.Log("Bouton pressé");
    }
}
