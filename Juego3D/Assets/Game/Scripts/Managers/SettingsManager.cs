using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Paneles")]
    public GameObject panelBotones;
    public GameObject panelAudio;
    public GameObject panelPantalla;
    public GameObject panelGameplay;

    [Header("Botones Tab")]
    public Button btnAudio;
    public Button btnPantalla;
    public Button btnGameplay;

    [Header("Audio")]
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderSFX;

    [Header("Pantalla")]
    public TMP_Dropdown dropdownResolucion;
    public TMP_Dropdown dropdownCalidad;
    public Toggle togglePantallaCompleta;
    public Toggle toggleVSync;

    [Header("Gameplay")]
    public Slider sliderSensibilidad;

    private Resolution[] resoluciones;

    // Colores para tabs activo/inactivo
    private Color colorActivo = new Color(1f, 1f, 1f, 1f);
    private Color colorInactivo = new Color(0.6f, 0.6f, 0.6f, 1f);

    void Start()
    {
        InicializarResoluciones();
        InicializarCalidad();
        CargarConfiguracion();
        MostrarPanel("Inicial"); 
    }

    // ─── TABS ─────────────────────────────────────────────

    public void MostrarPanel(string nombre)
    {
        panelBotones.SetActive(true);
        panelAudio.SetActive(nombre == "Audio");
        panelPantalla.SetActive(nombre == "Pantalla");
        panelGameplay.SetActive(nombre == "Gameplay");

        // Resalta botones
        btnAudio.image.color = nombre == "Audio" ? colorActivo : colorInactivo;
        btnPantalla.image.color = nombre == "Pantalla" ? colorActivo : colorInactivo;
        btnGameplay.image.color = nombre == "Gameplay" ? colorActivo : colorInactivo;
    }

    // ─── AUDIO ────────────────────────────────────────────

    public void CambiarMaster(float valor)
    {
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20 : -80f;
        if (audioMixer != null) audioMixer.SetFloat("VolumenMaster", db);
        PlayerPrefs.SetFloat("master", valor);
    }

    public void CambiarMusica(float valor)
    {
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20 : -80f;
        if (audioMixer != null) audioMixer.SetFloat("VolumenMusica", db);
        PlayerPrefs.SetFloat("musica", valor);
    }

    public void CambiarSFX(float valor)
    {
        float db = valor > 0.001f ? Mathf.Log10(valor) * 20 : -80f;
        if (audioMixer != null) audioMixer.SetFloat("VolumenSFX", db);
        PlayerPrefs.SetFloat("sfx", valor);
    }

    // ─── PANTALLA ─────────────────────────────────────────

    void InicializarResoluciones()
    {
        dropdownResolucion.ClearOptions();

        int[] anchosPermitidos = { 1280, 1366, 1600, 1920, 2560, 3840 };
        var resolucionesComunes = new System.Collections.Generic.List<Resolution>();

        foreach (var res in Screen.resolutions)
            foreach (int ancho in anchosPermitidos)
                if (res.width == ancho) { resolucionesComunes.Add(res); break; }

        if (resolucionesComunes.Count == 0)
            resolucionesComunes.AddRange(Screen.resolutions);

        resoluciones = resolucionesComunes.ToArray();

        int indiceActual = 0;
        var opciones = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resoluciones.Length; i++)
        {
            opciones.Add(resoluciones[i].width + " x " + resoluciones[i].height);
            if (resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
                indiceActual = i;
        }

        dropdownResolucion.AddOptions(opciones);
        dropdownResolucion.value = PlayerPrefs.GetInt("resolucion", indiceActual);
        dropdownResolucion.RefreshShownValue();
    }

    public void CambiarResolucion(int indice)
    {
        Resolution res = resoluciones[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolucion", indice);
    }

    void InicializarCalidad()
    {
        dropdownCalidad.ClearOptions();
        dropdownCalidad.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
        dropdownCalidad.value = PlayerPrefs.GetInt("calidad", QualitySettings.GetQualityLevel());
        dropdownCalidad.RefreshShownValue();
    }

    public void CambiarCalidad(int indice)
    {
        QualitySettings.SetQualityLevel(indice);
        PlayerPrefs.SetInt("calidad", indice);
    }

    public void CambiarPantallaCompleta(bool activo)
    {
        Screen.fullScreen = activo;
        PlayerPrefs.SetInt("pantallaCompleta", activo ? 1 : 0);
    }

    public void CambiarVSync(bool activo)
    {
        QualitySettings.vSyncCount = activo ? 1 : 0;
        PlayerPrefs.SetInt("vsync", activo ? 1 : 0);
    }

    // ─── GAMEPLAY ─────────────────────────────────────────

    public void CambiarSensibilidad(float valor)
    {
        PlayerPrefs.SetFloat("sensibilidad", valor);
    }

    // ─── GUARDAR / CARGAR / RESTAURAR ────────────────────

    public void GuardarConfiguracion()
    {
        PlayerPrefs.Save();
        Debug.Log("Configuración guardada.");
    }

    public void RestaurarDefaults()
    {
        // Audio
        sliderMaster.value = 1f;
        sliderMusica.value = 0.8f;
        sliderSFX.value = 1f;

        // Pantalla
        togglePantallaCompleta.isOn = true;
        toggleVSync.isOn = false;
        dropdownCalidad.value = QualitySettings.GetQualityLevel();

        // Gameplay
        sliderSensibilidad.value = 2f;

        GuardarConfiguracion();
        Debug.Log("Configuración restaurada a valores por defecto.");
    }

    void CargarConfiguracion()
    {
        // Audio
        float master = PlayerPrefs.GetFloat("master", 1f);
        float musica = PlayerPrefs.GetFloat("musica", 0.8f);
        float sfx = PlayerPrefs.GetFloat("sfx", 1f);

        if (sliderMaster != null) sliderMaster.value = master;
        if (sliderMusica != null) sliderMusica.value = musica;
        if (sliderSFX != null) sliderSFX.value = sfx;

        if (audioMixer != null)
        {
            CambiarMaster(master);
            CambiarMusica(musica);
            CambiarSFX(sfx);
        }

        // Pantalla
        bool pantallaCompleta = PlayerPrefs.GetInt("pantallaCompleta", 1) == 1;
        bool vsync = PlayerPrefs.GetInt("vsync", 0) == 1;

        if (togglePantallaCompleta != null) togglePantallaCompleta.isOn = pantallaCompleta;
        if (toggleVSync != null) toggleVSync.isOn = vsync;

        Screen.fullScreen = pantallaCompleta;
        QualitySettings.vSyncCount = vsync ? 1 : 0;

        // Gameplay
        float sensibilidad = PlayerPrefs.GetFloat("sensibilidad", 2f);
        if (sliderSensibilidad != null) sliderSensibilidad.value = sensibilidad;
    }
}