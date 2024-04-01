using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
public class CustomRenderPipeline : RenderPipeline {
    CameraRenderer renderer = new CameraRenderer();
    	protected override void Render (
		ScriptableRenderContext context, Camera[] cameras
	) {}
    	protected override void Render (
		ScriptableRenderContext context, List<Camera> cameras
	) { 
        		for (int i = 0; i < cameras.Count; i++) {
			renderer.Render(context, cameras[i]);
		}
    }
}