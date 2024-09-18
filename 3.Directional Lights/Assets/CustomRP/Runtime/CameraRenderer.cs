using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {

    ScriptableRenderContext context; //渲染上下文

    Camera camera; //对应相机
    const string bufferName = "Render Camera"; //命令缓冲名称
    CullingResults cullingResults; //用于存储不被剔除，可见物体的字段
    CommandBuffer buffer = new CommandBuffer {
        name = bufferName
    };
    Lighting lighting = new Lighting();

    private static ShaderTagId 
        unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
        litShaderTagId = new ShaderTagId("CustomLit");//shader标签

    public void Render (ScriptableRenderContext context, Camera camera, bool useDynamicBatching, bool useGPUInstancing) {
        this.context = context;
        this.camera = camera;
        PrepareBuffer();
        PrepareForSceneWindow();//UI显示,因为UI会新加集合体，所以放在裁剪之前
        if (!Cull()) {
            return;
        }

        Setup(); //设置相机属性
        lighting.Setup(context, cullingResults); //设置光照;
        DrawVisibleGeometry(useDynamicBatching, useGPUInstancing); //绘制可见物体
        
        DrawUnsupportedShaders(useDynamicBatching, useGPUInstancing); //绘制不支持的shader
        DrawGizmos(); //渲染小组件
        Submit();  //提交所有渲染命令缓冲
    }
    
    void DrawVisibleGeometry (bool useDynamicBatching, bool useGPUInstancing) {
        
        var sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };//从前到后排序，先不透明，再天空盒子，再透明
        var drawingSettings = new DrawingSettings(
            unlitShaderTagId, sortingSettings
        ) {
            enableDynamicBatching = useDynamicBatching,
            enableInstancing = useGPUInstancing
        }; //渲染设置, 用于指定支持可以被渲染的shader标签
        drawingSettings.SetShaderPassName(1, litShaderTagId);//把litShaderTagId添加到渲染通道中
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);; //过滤设置 : 哪些队列可以被选入
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings); //根据传进来的3个参数，绘制可见物体
        context.DrawSkybox(camera);
        
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
        
        //context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }
    
    /*
    void DrawUnsupportedShaders () {
        if (errorMaterial == null) {
            errorMaterial =
                new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        var drawingSettings = new DrawingSettings(
            legacyShaderTagIds[0], new SortingSettings(camera)
        ) {
            overrideMaterial = errorMaterial
        };
        for (int i = 1; i < legacyShaderTagIds.Length; i++) {
            drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        context.DrawRenderers(
            cullingResults, ref drawingSettings, ref filteringSettings
        );
    }*/
    void Setup () {
        context.SetupCameraProperties(camera);//设置相机属性
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(
            flags <= CameraClearFlags.Depth,
            flags == CameraClearFlags.Color,
            flags == CameraClearFlags.Color ?
                camera.backgroundColor.linear : Color.clear
        );
        //buffer.ClearRenderTarget(true, true, Color.clear);//清除渲染目标
        buffer.BeginSample(SampleName);//采样点开始
        ExecuteBuffer();
        
        
    }   
    void Submit () {
        buffer.EndSample(SampleName);
        ExecuteBuffer();
        context.Submit();
    }
    void ExecuteBuffer () {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }
    
    bool Cull () {
        //ScriptableCullingParameters p
        if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
            cullingResults = context.Cull(ref p); //可见物体被存在了cullingResults中
            return true;
        }
        return false;
    }
}