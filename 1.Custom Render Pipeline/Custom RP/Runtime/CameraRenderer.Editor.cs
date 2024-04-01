using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer {
	CullingResults cullingResults;
	ScriptableRenderContext context;


	Camera camera;
	const string bufferName = "Render Camera";

	CommandBuffer buffer = new CommandBuffer {name = bufferName};


	public void Render (ScriptableRenderContext context, Camera camera) {
		this.context = context;
		this.camera = camera;

	PrepareBuffer();
	PrepareForSceneWindow();
	
	if (!Cull()) {
			return;
		}


		Setup();
		DrawVisibleGeometry();
		DrawUnsupportedShaders();

		DrawGizmos();//在其他一切渲染之后渲染小空间控件
		Submit();
	}
void DrawVisibleGeometry () {

		var sortingSettings = new SortingSettings(camera) {criteria = SortingCriteria.CommonOpaque};
		//设置可渲染的Shader
		var drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);
		//根据渲染队列过滤物体
		var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
		
		context.DrawSkybox(camera);
		//渲染顺序：先不透明，再透明
		 sortingSettings = new SortingSettings(camera) {criteria = SortingCriteria.CommonTransparent};

		//设置可渲染的Shader
		 drawingSettings = new DrawingSettings(unlitShaderTagId, sortingSettings);

		//根据渲染队列过滤物体
		filteringSettings = new FilteringSettings(RenderQueueRange.transparent);
		//渲染命令
		context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
		

		

	}
	void Setup () {
		context.SetupCameraProperties(camera);

		CameraClearFlags flags = camera.clearFlags;

		buffer.ClearRenderTarget(
			flags <= CameraClearFlags.Depth,
			flags <= CameraClearFlags.Color,
			flags == CameraClearFlags.Color ?
				camera.backgroundColor.linear : Color.clear
		);
		buffer.BeginSample(SampleName);
		ExecuteBuffer();
		//context.SetupCameraProperties(camera);
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

		if (camera.TryGetCullingParameters(out ScriptableCullingParameters p)) {
			
			cullingResults = context.Cull(ref p);
			return true;
		}
		return false;
	}


}