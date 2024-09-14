using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEngine.Profiling;
partial class CameraRenderer {
#if UNITY_EDITOR
    static Material errorMaterial; //错误材质

    static ShaderTagId[] legacyShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    partial void DrawGizmos ();
    partial void PrepareForSceneWindow ();
    partial void DrawUnsupportedShaders (bool useDynamicBatching, bool useGPUInstancing);
    partial void PrepareBuffer ();
    
    #if UNITY_EDITOR
        string SampleName { get; set; }
        partial void PrepareBuffer () {
            Profiler.BeginSample("Editor Only");
            buffer.name = SampleName = camera.name;
            Profiler.EndSample();
        }
        partial void PrepareForSceneWindow () {  //添加UI显示到世界空间
            if (camera.cameraType == CameraType.SceneView) {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
            }
        }
        partial void DrawGizmos () {
            if (Handles.ShouldRenderGizmos()) {
                context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
                context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
            }
        }
        partial void DrawUnsupportedShaders (bool useDynamicBatching, bool useGPUInstancing) {
            if (errorMaterial == null) {
                errorMaterial =
                    new Material(Shader.Find("Hidden/InternalErrorShader"));
            }
            var drawingSettings = new DrawingSettings(
                legacyShaderTagIds[0], new SortingSettings(camera)
            ) {
                overrideMaterial = errorMaterial,
                enableDynamicBatching = useDynamicBatching,
                enableInstancing = useGPUInstancing
            };
            for (int i = 1; i < legacyShaderTagIds.Length; i++) {
                drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
            }
            var filteringSettings = FilteringSettings.defaultValue;
            context.DrawRenderers(
                cullingResults, ref drawingSettings, ref filteringSettings
            );
        }
    #else
        string SampleName => bufferName;
        const string SampleName = bufferName;
    #endif
    
    
#endif
}