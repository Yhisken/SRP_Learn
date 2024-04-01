using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

partial  class CameraRenderer {
#if UNITY_EDITOR
	static ShaderTagId unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

	partial void DrawUnsupportedShaders ();
	partial void DrawGizmos ();
	partial void PrepareForSceneWindow ();
	partial void PrepareBuffer ();
	string SampleName { get; set; }

	static Material errorMaterial;
	static ShaderTagId[] legacyShaderTagIds = {//不支持渲染的错误ShaderTag
		new ShaderTagId("Always"),
		new ShaderTagId("ForwardBase"),
		new ShaderTagId("PrepassBase"),
		new ShaderTagId("Vertex"),
		new ShaderTagId("VertexLMRGBM"),
		new ShaderTagId("VertexLM")
	};
	partial void DrawGizmos () {//渲染小空控件
		if (Handles.ShouldRenderGizmos()) {
			context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
			context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
		}
	}

	
	partial void PrepareForSceneWindow () {
		if (camera.cameraType == CameraType.SceneView) {
			ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
		}
	}
	partial void DrawUnsupportedShaders () {
		//用bug红重载不受支持的Shader
		if (errorMaterial == null) {
			errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
		}
		var drawingSettings = new DrawingSettings(legacyShaderTagIds[0], new SortingSettings(camera)){
			overrideMaterial = errorMaterial
		};

		//
		for (int i = 1; i < legacyShaderTagIds.Length; i++) {
			drawingSettings.SetShaderPassName(i, legacyShaderTagIds[i]);
		}
		var filteringSettings = FilteringSettings.defaultValue;//反正是错的，直接用默认设置过滤
		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
	}

	partial void PrepareBuffer () {
		Profiler.BeginSample("Editor Only");
		buffer.name = SampleName = camera.name;
		Profiler.EndSample();
	}
	
#else

	string SampleName => bufferName;

#endif
}