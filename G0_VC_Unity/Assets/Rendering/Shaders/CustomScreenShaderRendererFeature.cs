using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.XR.XRDisplaySubsystem;
using System;
///using UnityEditor.Rendering;
using static Unity.Burst.Intrinsics.X86.Avx;

public class CustomScreenShaderRendererFeature : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private RTHandle rtHandle;
        RTHandle tempRT;
        private int layerMask;
        private Material colorMaterial;
        private Material fullscreenMaterial;

        public CustomRenderPass(int layerMask, Material color_material, Material fullscreen_material)
        {
            this.layerMask = layerMask;
            this.colorMaterial = color_material;
            this.fullscreenMaterial = fullscreen_material;
        }
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            rtHandle = RTHandles.Alloc(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 1, DepthBits.None, GraphicsFormat.R8G8B8A8_UNorm, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_LayerRenderTexture");
            tempRT = RTHandles.Alloc(cameraTextureDescriptor.width, cameraTextureDescriptor.height, 1, DepthBits.None, GraphicsFormat.R8G8B8A8_UNorm, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TempTexture");
            ConfigureTarget(rtHandle);
            cmd.SetRenderTarget(rtHandle);
            //ConfigureClear(ClearFlag.All, Color.black);
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //context.Submit();
            /*            // Create and schedule a command to clear the current render target
                        var cmd = CommandBufferPool.Get();
                        //cmd.ClearRenderTarget(true, true, Color.black);
                        context.ExecuteCommandBuffer(cmd);*/
            //cmd.Release();

            //Debug.Log(Camera.current);

            var cmd = CommandBufferPool.Get();
            Camera camera = renderingData.cameraData.camera;
/*            float OG_fov = camera.fieldOfView;
            camera.fieldOfView = 40;*/
            // Get the culling parameters from the current Camera
            camera.TryGetCullingParameters(out var cullingParameters);

            // Use the culling parameters to perform a cull operation, and store the results
            

            // Update the value of built-in shader variables, based on the current Camera
            context.SetupCameraProperties(camera);

            // Tell Unity which geometry to draw, based on its LightMode Pass tag value
            ShaderTagId shaderTagId = new ShaderTagId("Always");

            // Tell Unity how to sort the geometry, based on the current Camera
            var sortingSettings = new SortingSettings(camera);

            // Create a DrawingSettings struct that describes which geometry to draw and how to draw it
            DrawingSettings drawingSettings = new DrawingSettings(shaderTagId, sortingSettings);

            // Tell Unity how to filter the culling results, to further specify which geometry to draw
            // Use FilteringSettings.defaultValue to specify no filtering
            FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.opaque, 00000110);

            var cullingResults = context.Cull(ref cullingParameters);

            // Schedule a command to draw the geometry, based on the settings you have defined
            context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);



            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            //context.Submit();
            //camera.fieldOfView = OG_fov;

            // Schedule a command to draw the Skybox if required
            /*            if (camera.clearFlags == CameraClearFlags.Skybox && RenderSettings.skybox != null)
                        {
                            //context.DrawSkybox(camera);
                        }*/

            // Instruct the graphics API to perform all scheduled commands
            /*context.Submit();
            return; */
            /*CommandBuffer cmd = CommandBufferPool.Get();
            //cmd.ClearRenderTarget(true, true, Color.black);
            cmd.SetRenderTarget(rtHandle);
            
            context.ExecuteCommandBuffer(cmd);
            //cmd.Release();
            
            //Debug.Log());//BuiltinRenderTextureType.CurrentActive);
            var drawingSettings = CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, ~0);
            //context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            cmd.ClearRenderTarget(false, true, Color.red);
            //cmd.Clear();//
            context.Submit();
            context.ExecuteCommandBuffer(cmd);
            //rtHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            //Debug.Log(cmd == null | rtHandle == null | tempRT == null | colorMaterial == null);
            tempRT = renderingData.cameraData.renderer.cameraColorTargetHandle;
            //return;
            Blitter.BlitCameraTexture(cmd, rtHandle, tempRT, colorMaterial, 0);
            Shader.SetGlobalTexture("_ObjectRenderTexture", tempRT.rt);
            context.ExecuteCommandBuffer(cmd);
            //tempRT = renderingData.cameraData.renderer.cameraColorTargetHandle;



            cmd.Clear();
            cmd.Release();


            return;*/

            /*Blit(cmd, rtHandle, renderingData.cameraData.renderer.cameraColorTargetHandle, colorMaterial);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            //Debug.Log(rtHandle == null);
            //colorMaterial.SetGlobalTexture("_ObjectRenderTexture", rtHandle.rt);
            // Instruct the graphics API to perform all scheduled commands
            context.Submit();*/

            /*CommandBuffer cmd = new CommandBuffer(); //CommandBufferPool.Get("Render Layer Command Buffer");



            //cmd.SetGlobalTexture("_MyRenderTexture", rtHandle.nameID);
            // Get the culling parameters from the current Camera
            //Camera.current.TryGetCullingParameters(out var cullingParameters);

            // Use the culling parameters to perform a cull operation, and store the results
            //var cullingResults = context.Cull(ref cullingParameters);

            //context.SetupCameraProperties(Camera.current);
            //Debug.Log(cmd);
            *//*            using (new ProfilingScope(cmd, new ProfilingSampler("Render Layer Profiling Scope")))
                        {*//*
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            var drawingSettings = new DrawingSettings(new ShaderTagId("UniversalForward"), new SortingSettings(Camera.current));
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque, 0);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            //Blitter.BlitTexture(cmd, rtHandle.nameID, BuiltinRenderTextureType.CameraTarget, colorMaterial, 0);
            /////////messed with this for testing
            // Apply the white color shader to the render texture
            //Blit(cmd, rtHandle, rtHandle, colorMaterial); 
            // Apply the fullscreen shader to the render texture and render to the screen
            //Blit(cmd, rtHandle, renderingData.cameraData.renderer.cameraColorTargetHandle, colorMaterial);

            
            //}
            context.ExecuteCommandBuffer(cmd); 
            CommandBufferPool.Release(cmd);

            context.Submit();*/
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            // Unset the active render texture
            if (RenderTexture.active == rtHandle.rt | RenderTexture.active == tempRT.rt) { RenderTexture.active = null; }
            RTHandles.Release(rtHandle);
            RTHandles.Release(tempRT);
        }
    }
    [SerializeField] private LayerMask layerMask;

    CustomRenderPass m_ScriptablePass;

    [SerializeField] private Material colorMaterial; //= new Material(Shader.Find("Shader Graphs/SpecialOutlines"));
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] RenderPassEvent renderTime;
    //[Flags]
/*    public enum LightModeTags
    {
        None = 0,
        SRPDefaultUnlit = 1 << 0,
        UniversalForward = 1 << 1,
        UniversalForwardOnly = 1 << 2,
        LightweightForward = 1 << 3,
        DepthNormals = 1 << 4,
        DepthOnly = 1 << 5,
        Standard = SRPDefaultUnlit | UniversalForward | UniversalForwardOnly | LightweightForward,
    }*/
    public override void Create()
    {
        //Debug.Log(int.Parse(Convert.ToString(layerMask.value, 2)));
        m_ScriptablePass = new CustomRenderPass(layerMask.value, colorMaterial, fullscreenMaterial);

        m_ScriptablePass.renderPassEvent = renderTime;
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);

    }
}


