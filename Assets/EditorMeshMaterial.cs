using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class EditorMeshMaterial : MonoBehaviour
{
    public new Renderer renderer;
    public Color color;

    List<Material> sharedMaterials = new List<Material>();
    List<Material> tempMaterials = new List<Material>();

    void OnEnable()
    {
        RenderPipelineManager.beginContextRendering += OnBeginCameraRendering;
        RenderPipelineManager.endContextRendering += OnEndCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginContextRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endContextRendering -= OnEndCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (renderer == null) { return; }

        renderer.GetSharedMaterials(sharedMaterials);

        // Reduce the "new Material()" to prevent GOU errors.
        for (int i = tempMaterials.Count; i < sharedMaterials.Count; i++)
        {
            tempMaterials.Add(new Material(sharedMaterials[i]));
        }

        if (tempMaterials.Count > sharedMaterials.Count)
        {
            tempMaterials.RemoveRange(sharedMaterials.Count, tempMaterials.Count - sharedMaterials.Count);
        }

        for (int i = 0; i < tempMaterials.Count; i++)
        {
            tempMaterials[i].CopyPropertiesFromMaterial(sharedMaterials[i]);
            tempMaterials[i].color = color;
        }

        renderer.sharedMaterials = tempMaterials.ToArray();
    }

    void OnEndCameraRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        if (renderer == null) { return; }
        renderer.sharedMaterials = sharedMaterials.ToArray();
    }
}
