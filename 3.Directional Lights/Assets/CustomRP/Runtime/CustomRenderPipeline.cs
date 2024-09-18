using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();
    bool useDynamicBatching, useGPUInstancing;
    public CustomRenderPipeline (bool useDynamicBatching, bool useGPUInstancing, bool useSRPBatcher) {//构造函数
        this.useDynamicBatching = useDynamicBatching;//是否使用动态批处理
        this.useGPUInstancing = useGPUInstancing;//是否使用GPU实例化
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher; //启用SRP Batch
        GraphicsSettings.lightsUseLinearIntensity = true; //启用线性光照
    }
    
    protected override void Render (ScriptableRenderContext context, Camera[] cameras) 
    {
        foreach (Camera camera in cameras) 
        {
            renderer.Render(context, camera, useDynamicBatching, useGPUInstancing);
        }
    }
    
    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        foreach (Camera camera in cameras) 
        {
            renderer.Render(context, camera, useDynamicBatching, useGPUInstancing);
        }
    } 
    
}