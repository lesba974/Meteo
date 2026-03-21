# Météo VR

---

## Membres du projet
- **LESBARRERES Emma LESE02560400**  
- **DE JERPHANION Tanguy DEJT06040600**

---

## Description du projet

Application de météo en réalité virtuelle permettant de consulter les prévisions sur 7 jours pour plusieurs villes. L'environnement réagit dynamiquement aux conditions météo : effets visuels, sonores et lumineux s'adaptent en temps réel.

---

## Environnement technique

### Moteur de jeu
- **Unity** : `6000.3.3f1`

### Packages utilisés
- **XR Interaction Toolkit**
- **TextMeshPro**

### API utilisée
- **Open-Meteo** : `https://api.open-meteo.com` (données météo en temps réel, gratuite, sans clé API)

### Modalités de déplacements
- **Continuous Turn**
- **Continuous Move**

### Modalités d'interaction
- **Direct Interactor** : interaction par contact direct avec les boutons des bornes via les contrôleurs

---

## Fonctionnalités

### Bornes interactives
- **Borne Ville** : sélection de la ville parmi Chicoutimi, Dax et Rambouillet
- **Borne Date** : navigation sur 7 jours (jour précédent / aujourd'hui / jour suivant)

### Villes disponibles
| Ville | Pays | Coordonnées |
|---|---|---|
| Chicoutimi | Canada | 48.42, -71.06 |
| Dax | France | 43.71, -1.05 |
| Rambouillet | France | 48.65, 1.83 |

---

## Fenêtres (interfaces spatialisées)

- **Canvas météo** (World Space) : affiche en temps réel le nom de la ville, la date et l'heure, la température, la condition météo et la vitesse du vent
- **CanvasVille** : titre de la borne et labels sous chaque bouton (Chicoutimi, Rambouillet, Dax)
- **CanvasDate** : titre de la borne et labels sous chaque bouton (Précédent, Aujourd'hui, Suivant)

---

## Effets visuels et météo

| Condition | Effets |
|---|---|
| Ciel clair | Lumière chaude et intense |
| Nuageux | Lumière gris bleutée + particules de nuages |
| Brouillard | Fog global activé + lumière réduite |
| Pluie | Particules de pluie + son de pluie spatialisé |
| Neige | Particules de neige |
| Averses | Particules de pluie + son de pluie spatialisé |
| Orage | Pluie + son de tonnerre spatialisé + éclairs (LightningLight clignotante) |

---

## Sources de lumière

- **Directional Light** : lumière principale de la scène, intensité et couleur adaptées dynamiquement selon la météo
- **LightningLight** : lumière ponctuelle qui clignote aléatoirement lors des orages pour simuler des éclairs
- **button_highlighter** (x2) : lumières de mise en valeur des boutons des bornes

---

## Structure de la scène

```
SampleScene
├── Murs
│   ├── Mur 1
│   └── Mur 2
├── XR Interaction Manager
│   ├── XR Origin (VR)
│   │   ├── Left (XR Controller Left + Direct Interactor)
│   │   └── Right (XR Controller Right + Direct Interactor)
│   └── Locomotion
│       ├── Move
│       └── Turn
├── terrain
│   ├── herbe
│   └── route
├── Canvas                          → Affichage météo (World Space)
│   └── PanelFond
│       ├── NomVille
│       ├── Date
│       ├── Separateur
│       ├── Temperature
│       ├── Meteo
│       └── Vent
├── BorneMeteoDate                  → Borne de navigation des jours
│   ├── Aujourd'hui
│   ├── suivant
│   ├── precedent
│   └── CanvasDate
│       ├── TitreDate
│       ├── LabelAujourdhui
│       ├── LabelPrecedent
│       └── LabelSuivant
├── BorneMeteoVille                 → Borne de sélection de ville (Script BorneMeteo)
│   ├── Chicoutimi
│   ├── Rambouillet
│   ├── Dax
│   └── CanvasVille
│       ├── TitreVille
│       ├── LabelChicoutimi
│       ├── LabelRambouillet
│       └── LabelDax
├── WeatherParticle                 → Effets météo
│   ├── Rain
│   ├── snow
│   └── Clouds
├── audio_components                → Sons spatialisés
│   ├── audio_rain
│   ├── button activation
│   └── audio_thunder
└── lights
    ├── Directional Light
    ├── button_highlighter
    ├── button_highlighter (1)
    └── LightningLight              → Éclairs lors des orages
```

---