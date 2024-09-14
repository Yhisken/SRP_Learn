using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();

    protected override void Render (ScriptableRenderContext context, Camera[] cameras) 
    {
        foreach (Camera camera in cameras) 
        {
            renderer.Render(context, camera);
        }
    }
    
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        foreach (Camera camera in cameras) 
        {
            renderer.Render(context, camera);
        }
    } 
    
}