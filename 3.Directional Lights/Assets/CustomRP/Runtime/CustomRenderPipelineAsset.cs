using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")] //在Project 可以右键创建的特性
public class CustomRenderPipelineAsset : RenderPipelineAsset {
    [SerializeField]
    bool useDynamicBatching = true, useGPUInstancing = true, useSRPBatcher = true; //是否使用动态批处理，是否使用GPU实例化，是否使用SRP批处理
    //创建实例
    protected override RenderPipeline CreatePipeline ()
    {
        return new CustomRenderPipeline(useDynamicBatching, useGPUInstancing, useSRPBatcher);
    }
}