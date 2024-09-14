using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")] //在Project 可以右键创建的特性
public class CustomRenderPipelineAsset : RenderPipelineAsset {
    
    //创建实例
    protected override RenderPipeline CreatePipeline ()
    {
        return new CustomRenderPipeline();
    }
}