using System.IO;
using UnityEngine;

[System.Serializable]
public class DatosJugador
{
    public int energia;
    public int escena;
    public string[] cristales;
    public string[] piezas;
}

public class JSONManager : MonoBehaviour
{
    public static JSONManager Instance;
    private string rutaGuardado;

    void Awake()
    {
        Instance = this;
        rutaGuardado = Application.persistentDataPath + "/guardado.json";
    }

    public void GuardarProgreso()
    {
        var gm = GameManager.Instance;
        DatosJugador datos = new DatosJugador();
        datos.energia = gm.energiaTotal;
        datos.escena = gm.escenaActual;
        datos.cristales = gm.cristalesRecogidos.ToArray();

        var piezasGuardadas = new System.Collections.Generic.List<string>();
        foreach (var p in gm.piezasNave)
            if (p.Value) piezasGuardadas.Add(p.Key);
        datos.piezas = piezasGuardadas.ToArray();

        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaGuardado, json);
        Debug.Log("Progreso guardado en: " + rutaGuardado);
    }

    public void CargarProgreso()
    {
        if (!File.Exists(rutaGuardado))
        {
            Debug.LogWarning("No existe archivo de guardado.");
            return;
        }

        string json = File.ReadAllText(rutaGuardado);
        DatosJugador datos = JsonUtility.FromJson<DatosJugador>(json);

        var gm = GameManager.Instance;
        gm.energiaTotal = datos.energia;
        gm.escenaActual = datos.escena;

        gm.cristalesRecogidos.Clear();
        if (datos.cristales != null)
            gm.cristalesRecogidos.AddRange(datos.cristales);

        if (datos.piezas != null)
            foreach (var p in datos.piezas)
                if (gm.piezasNave.ContainsKey(p))
                    gm.piezasNave[p] = true;

        Debug.Log("Progreso cargado.");
    }
}