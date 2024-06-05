using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public List<Renderer> renderers;
    public Color color = Color.white;
    private List<Material> materials;
    private void Awake()
    {
        materials = new List<Material>();
        foreach(var r in renderers)
        {
            materials.AddRange(new List<Material>(r.materials));
        }
    }
    public void ToggleHighlight(bool val)
    {
        if(val)
        {
            foreach (var material in materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color);
            }
        }
        else
        {
            foreach(var material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}
