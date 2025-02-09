using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Linq;
//s
public class SelectiveScreenShader : ScriptableRendererFeature
{
    class SelectiveRenderPass : ScriptableRenderPass
    {


        int LayerBitMask;
        Material m_objectMaterial;
        Material m_screenMaterial;
        float FOV;
        float baseFOV;
        int uniquepassTag;
        //s
        public void Setup(Material object_mat, Material screen_mat, int _layerBitMask, float _FOV, int tag, float _baseFOV)
        {
            m_objectMaterial = object_mat;
            m_screenMaterial = screen_mat;
            requiresIntermediateTexture = true;
            LayerBitMask = _layerBitMask;
            FOV = _FOV;
            uniquepassTag = tag;
            baseFOV = _baseFOV;
        }

        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.
        private class PassData
        {
            // Create a field to store the list of objects to draw
            public RendererListHandle rendererListHandle;

            public TextureHandle intermediateColorTexture;

            public TextureHandle sourceTexture;

            public Material screenMaterial;

            public List<ShaderTagId> ShaderTags;

            public float fov;

            public UniversalCameraData cameraData;

            public int uniquepassTag;
        }


        public class CustomData : ContextItem
        {
            //public TextureHandle crossTexture;
            public Dictionary<int, TextureHandle> textureHandles = new Dictionary<int, TextureHandle>();
            public override void Reset()
            {
                //crossTexture = TextureHandle.nullHandle;

            }
        }


        //
        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            //if (data.uniquepassTag != 0) return;
                    /*            CommandBuffer _cmd = CommandBufferPool.Get();

                                _cmd.SetRenderTarget(data.intermediateColorTexture);

                                if (data.fov != 0)
                                {
                                    Camera camera = data.cameraData.camera;

                                    Rect pixelRect = data.cameraData.camera.rect;
                                    float cameraAspect = (float)pixelRect.width / (float)pixelRect.height;

                                    Matrix4x4 projectionMatrix = Matrix4x4.Perspective(data.fov, cameraAspect, camera.nearClipPlane, camera.farClipPlane);
                                    projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.sourceTexture));

                    *//*                *//*Matrix4x4 viewMatrix = data.cameraData.GetViewMatrix();
                                    Vector4 cameraTranslation = viewMatrix.GetColumn(3);
                                    viewMatrix.SetColumn(3, cameraTranslation + data.cameraSettings.offset); *//**//*

                                    //RenderingUtils.SetViewAndProjectionMatrices(context.cmd, data.cameraData.GetViewMatrix(), projectionMatrix, false);

                                    _cmd.SetViewProjectionMatrices(data.cameraData.GetViewMatrix(), projectionMatrix);
                                }

                                // Clear the render target to black
                                _cmd.ClearRenderTarget(true, true, Color.clear);

                                // Draw the objects in the list
                                //context.cmd.DrawRendererList(data.rendererListHandle);
                                _cmd.DrawRendererList(data.rendererListHandle);

                                data.screenMaterial.SetTexture("_Test", data.sourceTexture);


                    *//*            if (data.fov != 0)
                                {
                                    RenderingUtils.SetViewAndProjectionMatrices(context.cmd, data.cameraData.GetViewMatrix(), GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrix(0), data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.sourceTexture)), false);
                                }*/


            if (data.fov != 0)
            {
                Camera camera = data.cameraData.camera;
                //Debug.Log(data.fov);
                //Rect pixelRect = data.cameraData.camera.rect;
                //Debug.Log(pixelRect);
                //float cameraAspect = (float)pixelRect.width / (float)pixelRect.height;
                //
                Matrix4x4 projectionMatrix = Matrix4x4.Perspective(data.fov, data.cameraData.camera.aspect, camera.nearClipPlane, camera.farClipPlane);
                projectionMatrix = GL.GetGPUProjectionMatrix(projectionMatrix, !data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.sourceTexture));

                Matrix4x4 viewMatrix = data.cameraData.GetViewMatrix();
                Vector4 cameraTranslation = viewMatrix.GetColumn(3);
                viewMatrix.SetColumn(3, cameraTranslation); //+ data.cameraSettings.offset

                //RenderingUtils.SetViewAndProjectionMatrices(context.cmd, viewMatrix, projectionMatrix, false);

                context.cmd.SetViewProjectionMatrices(data.cameraData.GetViewMatrix(), projectionMatrix);
            }

            // Clear the render target to black
            context.cmd.ClearRenderTarget(true, true, Color.clear);

            // Draw the objects in the list
            //context.cmd.DrawRendererList(data.rendererListHandle);
            context.cmd.DrawRendererList(data.rendererListHandle);

            //if (data.uniquepassTag == 0) data.screenMaterial.SetTexture("_Test", data.sourceTexture);

            if (data.fov != 0)
            {
                //RenderingUtils.SetViewAndProjectionMatrices(context.cmd, data.cameraData.GetViewMatrix(), GL.GetGPUProjectionMatrix(data.cameraData.GetProjectionMatrix(0), data.cameraData.IsRenderTargetProjectionMatrixFlipped(data.sourceTexture)), false);
            }

            /*            Blitter.BlitCameraTexture(_cmd, data.intermediateColorTexture, data.destination);

                        Blitter.BlitCameraTexture(_cmd, data.destination, )*/

            //Debug.Log("render");
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            string passName = "Copy To Temp Texture";

            // Add a raster render pass to the render graph. The PassData type parameter determines
            // the type of the passData output variable.
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                builder.AllowGlobalStateModification(true);
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();
                // Create a destination texture for the copy operation based on the settings,
                // such as dimensions, of the textures that the camera uses.
                // Set msaaSamples to 1 to get a non-multisampled destination texture.
                // Set depthBufferBits to 0 to ensure that the CreateRenderGraphTexture method
                // creates a color texture and not a depth texture.
                //UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                //Debug.Log(cameraData.camera.fieldOfView);
                passData.screenMaterial = m_screenMaterial;

                passData.sourceTexture = resourceData.activeColorTexture;
                //Debug.Log(cameraData.camera.fieldOfView - baseFOV);
                passData.fov = FOV; //FOV + cameraData.camera.fieldOfView - baseFOV;

                passData.cameraData = cameraData;

                passData.uniquepassTag = uniquepassTag;
                //var textureDesc = resourceData.cameraDepthTexture.GetDescriptor(renderGraph);

/*                textureDesc.msaaSamples = MSAASamples.None;
                textureDesc.depthBufferBits = 0;*/
                // Populate passData with the data needed by the rendering function
                // of the render pass.
                // Use the camera's active color texture
                // as the source texture for the copy operation.

                //passData.intermediateColorTexture = renderGraph.CreateTexture(passData.sourceTexture);

                CustomData customData;
                if (frameData.Contains<CustomData>()) { customData = frameData.Get<CustomData>(); }
                else customData = frameData.Create<CustomData>();


                //Debug.Log(customData.textureHandles.Count);
                if (!customData.textureHandles.ContainsKey(uniquepassTag)) customData.textureHandles.Add(uniquepassTag, renderGraph.CreateTexture(passData.sourceTexture));
                else customData.textureHandles[uniquepassTag] = renderGraph.CreateTexture(passData.sourceTexture);

                //customData.crossTexture = passData.intermediateColorTexture;

                // For demonstrative purposes, this sample creates a temporary destination texture.
                // UniversalRenderer.CreateRenderGraphTexture is a helper method
                // that calls the RenderGraph.CreateTexture method.
                // Using a RenderTextureDescriptor instance instead of a TextureDesc instance
                // simplifies your code.

                // Declare that this render pass uses the temporary destination texture
                // as its color render target.
                // This is similar to cmd.SetRenderTarget prior to the RenderGraph API.
                builder.SetRenderAttachment(customData.textureHandles[uniquepassTag], 0);

                // RenderGraph automatically determines that it can remove this render pass
                // because its results, which are stored in the temporary destination texture,
                // are not used by other passes.
                // For demonstrative purposes, this sample turns off this behavior to make sure
                // that render graph executes the render pass. 
                builder.AllowPassCulling(false);

                // Get the data needed to create the list of objects to draw

                SortingCriteria sortFlags = cameraData.defaultOpaqueSortFlags;
                RenderQueueRange renderQueueRange = RenderQueueRange.all;
                FilteringSettings filterSettings = new FilteringSettings(renderQueueRange, LayerBitMask); //00000110
                // Redraw only objects that have their LightMode tag set to UniversalForward 
                ShaderTagId shadersToOverride = new ShaderTagId("UniversalForward");
               
                //
                // Create drawing settings
                DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shadersToOverride, renderingData, cameraData, lightData, sortFlags);

                // Add the override material to the drawing settings
                drawSettings.overrideMaterial = m_objectMaterial;

                // Create the list of objects to draw
                var rendererListParameters = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);

                // Convert the list to a list handle that the render graph system can use
                passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParameters);
                // UniversalResourceData contains all the texture references used by URP,
                // including the active color and depth textures of the camera.

                //Debug.Log(((RendererList)passData.rendererListHandle));


                // Set the ExecutePass method as the rendering function that render graph calls
                // for the render pass. 
                // This sample uses a lambda expression to avoid memory allocations.
                builder.UseRendererList(passData.rendererListHandle);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));


                
                //builder.e
                //Debug.Log("wahhh");
                //RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(passData.intermediateColorTexture, destination, m_screenMaterial, 0);

                /*renderGraph.AddBlitPass(para, passName: m_PassName);

                resourceData.cameraColor = destination;*/
            }
            //s
            /*var stack = VolumeManager.instance.stack;
            var resourceData = frameData.Get<UniversalResourceData>();

            

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Redraw objects", out var passData))
            {
                // Get the data needed to create the list of objects to draw
                UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();
                SortingCriteria sortFlags = cameraData.defaultOpaqueSortFlags;
                RenderQueueRange renderQueueRange = RenderQueueRange.opaque;
                FilteringSettings filterSettings = new FilteringSettings(renderQueueRange, 00000110);
                // Redraw only objects that have their LightMode tag set to UniversalForward 
                ShaderTagId shadersToOverride = new ShaderTagId("UniversalForward");
                // Create drawing settings
                DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shadersToOverride, renderingData, cameraData, lightData, sortFlags);

                // Add the override material to the drawing settings
                drawSettings.overrideMaterial = m_objectMaterial;

                // Create the list of objects to draw
                var rendererListParameters = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);

                // Convert the list to a list handle that the render graph system can use
                passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParameters);

                var textureDesc = resourceData.cameraDepthTexture.GetDescriptor(renderGraph);

                textureDesc.msaaSamples = MSAASamples.None;
                textureDesc.depthBufferBits = 0;
                // Populate passData with the data needed by the rendering function
                // of the render pass.
                // Use the camera's active color texture
                // as the source texture for the copy operation.

                passData.intermediateColorTexture = renderGraph.CreateTexture(textureDesc);

                builder.UseTexture(passData.intermediateColorTexture, AccessFlags.ReadWrite);

                builder.UseRendererList(passData.rendererListHandle);
                //builder.SetRenderAttachment(passData.intermediateColorTexture, 0, AccessFlags.ReadWrite);

                //builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Write);

                builder.AllowPassCulling(false);

                //builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));


                *//*var source = resourceData.activeColorTexture;

                var destination_desc = renderGraph.GetTextureDesc(source);

                destination_desc.name = $"CameraColor-{m_PassName}";

                destination_desc.clearBuffer = false;

                TextureHandle destination = renderGraph.CreateTexture(destination_desc);


                RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(passData.intermediateColorTexture, destination, m_screenMaterial, 0);

                renderGraph.AddBlitPass(para, passName: m_PassName);

                resourceData.cameraColor = destination;*/


            /*                if (resourceData.isActiveTargetBackBuffer)
                            {
                                Debug.LogAssertion("can't use backbuffer as texture input");
                                return;
                            }*/

            /*var source = resourceData.activeColorTexture;

            var destination_desc = renderGraph.GetTextureDesc(source);

            destination_desc.name = $"CameraColor-{m_PassName}";

            destination_desc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destination_desc);





            RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(intermediateColorTexture, destination, m_screenMaterial, 0);

            renderGraph.AddBlitPass(para, passName: m_PassName);

            resourceData.cameraColor = destination;*//*
        }*/




            //var intermediate_textureDesc = resourceData.cameraDepthTexture.GetDescriptor(renderGraph);












            //const string passName = "Render Custom Pass";
            /*
                        // This adds a raster render pass to the graph, specifying the name and the data type that will be passed to the ExecutePass function.
                        using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
                        {
                            // Use this scope to set the required inputs and outputs of the pass and to
                            // setup the passData with the required properties needed at pass execution time.

                            // Make use of frameData to access resources and camera data through the dedicated containers.
                            // Eg:
                            // UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                            // Setup pass inputs and outputs through the builder interface.
                            // Eg:
                            // builder.UseTexture(sourceTexture);
                            // TextureHandle destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraData.cameraTargetDescriptor, "Destination Texture", false);

                            // This sets the render target of the pass to the active color texture. Change it to your own render target as needed.
                            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

                            // Assigns the ExecutePass function to the render pass delegate. This will be called by the render graph when executing the pass.
                            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
                        }*/
        }


       
    /*        public override void FrameCleanup(CommandBuffer cmd)
            {
                base.FrameCleanup(cmd);
            }*/
    }

    class SelectiveShaderClass : ScriptableRenderPass
    {
        Material m_screenMaterial;
        Material m_material;
        string m_PassName = "SpecialOutlinePass";
        int uniquepassTag;
        SelectiveScreenShader screenPass;
        public void Setup(Material screen_Mat, int tag, Material mat)
        {
            m_screenMaterial = screen_Mat;
            m_material = mat;
            uniquepassTag = tag;
            requiresIntermediateTexture = true;
        }

        /*        public class CustomData : ContextItem
                {
                    public TextureHandle crossTexture;

                    public override void Reset()
                    {
                        crossTexture = TextureHandle.nullHandle;
                    }
                }*/

        class PassData
        {
            public Material screenMaterial;
            public TextureHandle cameraTexture;
            public TextureHandle colorTexture;
        }
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            //x
            

            //m_PassName = "SpecialOutlinePass" + uniquepassTag.ToString();

            using var builder = renderGraph.AddRasterRenderPass( passName, out PassData passData );
           





            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            passData.cameraTexture = resourceData.activeColorTexture;

            builder.AllowPassCulling(false);
            //builder.AllowGlobalStateModification(true);

            passData.screenMaterial = m_screenMaterial;
            //




            /*
                            var destinationDesc = renderGraph.GetTextureDesc(passData.cameraTexture);

                            destinationDesc.clearBuffer = false;



                            //TextureHandle test = renderGraph.CreateTexture(destinationDesc);
                            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);*/

            var customData = frameData.Get<SelectiveRenderPass.CustomData>();

            //Debug.Log();

            //TextureHandle color_Texture;
            if (customData.textureHandles.ContainsKey(uniquepassTag)) passData.colorTexture = customData.textureHandles[uniquepassTag];//renderGraph.CreateTexture(customData.textureHandles[uniquepassTag].GetDescriptor(renderGraph));
            else return;


            builder.UseTexture(passData.cameraTexture);

            // resourceData.cameraColor = passData.colorTexture;
            builder.UseTexture(passData.colorTexture);


            //resourceData.cameraColor = passData.colorTexture;
            /*                if (uniquepassTag == 1) Debug.Log("blahashsjajaks");
                            if (uniquepassTag == 0)
                            {
                                for (int i = 0; i < customData.textureHandles.Count; i++)
                                {

                                    if (customData.textureHandles.ContainsKey(customData.textureHandles.Keys.ToList()[i])) customData.textureHandles.Remove(customData.textureHandles.Keys.ToList()[i]);
                                    //Debug.Log((Texture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]]);//(RenderTexture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]]);
                                    //if ((RenderTexture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]] == (RenderTexture)TextureHandle.nullHandle) customData.textureHandles.Remove(customData.textureHandles.Keys.ToList()[i]);
                                    //customData.textureHandles[] = TextureHandle.nullHandle;
                                }
                            }*/
            //RenderTexture texture = new RenderTexture(passData.colorTexture);


            //////resourceData.cameraColor = passData.cameraTexture;
            //resourceData.cameraColor = passData.cameraTexture;


            /*                RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(passData.colorTexture, destination, m_screenMaterial, 0);

                            renderGraph.AddBlitPass(para, passName: m_PassName);*/

            //resourceData.cameraColor = passData.colorTexture;

            //Debug.LogWarning((new RenderTexture(passData.colorTexture)).height);

            //resourceData.cameraColor = passData.colorTexture;

            builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
                // builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));


            














            // return;


            /*            var resourceData = frameData.Get<UniversalResourceData>();




                        if (resourceData.isActiveTargetBackBuffer)
                        {
                            Debug.LogError("can't render");
                            return;
                        }

                        var source = resourceData.activeColorTexture;
                        var destinationDesc = renderGraph.GetTextureDesc(source);
                        destinationDesc.name = $"CameraColor-{m_PassName}";
                        destinationDesc.clearBuffer = false;


                        TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

                        RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(source, destination, m_screenMaterial, 0);

                        renderGraph.AddBlitPass(para, passName: m_PassName);

                        resourceData.cameraColor = destination;*/

            //string passName = "Apply Shader to Temp Texture";

            //using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            //{


            //builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));


            /* return; 
             if (resourceData.isActiveTargetBackBuffer)
             {
                 Debug.LogAssertion("can't use backbuffer as texture input");
                 return;
             }

             var customData = frameData.Get<SelectiveRenderPass.CustomData>();

             TextureHandle color_Texture;
             //TextureHandle color_Texture = customData.crossTexture;
             if (customData.textureHandles.ContainsKey(uniquepassTag)) color_Texture = customData.textureHandles[uniquepassTag];
             else return;
             if (uniquepassTag == 1) Debug.Log("blahashsjajaks");
             if (uniquepassTag == 0)
             {
                 for (int i = 0; i < customData.textureHandles.Count; i++)
                 {

                     if (customData.textureHandles.ContainsKey(customData.textureHandles.Keys.ToList()[i])) customData.textureHandles.Remove(customData.textureHandles.Keys.ToList()[i]);
                     //Debug.Log((Texture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]]);//(RenderTexture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]]);
                     //if ((RenderTexture)customData.textureHandles[customData.textureHandles.Keys.ToList()[i]] == (RenderTexture)TextureHandle.nullHandle) customData.textureHandles.Remove(customData.textureHandles.Keys.ToList()[i]);
                     //customData.textureHandles[] = TextureHandle.nullHandle;
                 }
             }

             //m_screenMaterial.SetTexture("_Test", color_Texture);

             var source = resourceData.activeColorTexture;

             var destinationDesc = renderGraph.GetTextureDesc(source);

             destinationDesc.clearBuffer = false;



             //TextureHandle test = renderGraph.CreateTexture(destinationDesc);
             TextureHandle destination = renderGraph.CreateTexture(destinationDesc);


 *//*
             var textureDesc = resourceData.cameraDepthTexture.GetDescriptor(renderGraph);

             textureDesc.msaaSamples = MSAASamples.None;
             textureDesc.depthBufferBits = 0;
             // Populate passData with the data needed by the rendering function
             // of the render pass.
             // Use the camera's active color texture
             // as the source texture for the copy operation.

             TextureHandle destination = renderGraph.CreateTexture(textureDesc);*//*

             if (uniquepassTag == 0)
             {
 *//*                RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(color_Texture, destination, m_screenMaterial, 0);

                 renderGraph.AddBlitPass(para, passName: m_PassName);*//*




                 resourceData.cameraColor = destination;
             }
             else
             {
                *//* RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(color_Texture, destination, m_screenMaterial, 0);

                 renderGraph.AddBlitPass(para, passName: m_PassName);*//*

                 m_screenMaterial.SetTexture("_Test", resourceData.activeColorTexture);

                 resourceData.cameraColor = destination;
             }

 *//*            RenderGraphUtils.BlitMaterialParameters para = new RenderGraphUtils.BlitMaterialParameters(color_Texture, destination, m_screenMaterial, 0);

             renderGraph.AddBlitPass(para, passName: m_PassName);



             if (uniquepassTag == 0) resourceData.cameraColor = destination;
             else resourceData.cameraColor = resourceData.activeColorTexture;*//*

             //}*/

           
        }
        void ExecutePass(PassData data, RasterGraphContext context)
        {
            //s
            //RenderTexture texture = new RenderTexture(data.colorTexture);

            //Debug.Log(texture.isReadable);
            //data.screenMaterial.SetTexture("_Test", data.cameraTexture);
            Blitter.BlitTexture(context.cmd, data.colorTexture, new Vector4(1, 1, 0, 0), data.screenMaterial, 0);



        }

    }

    class WriteToOutlineShaderPass : ScriptableRenderPass
    {
        Material m_screenMaterial;
        public void Setup(Material screenMat)
        {
            m_screenMaterial = screenMat;
        }
        class PassData
        {
            public TextureHandle cameraTexture;
            public Material screenMat;
        }
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData))
            {
                UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
                passData.cameraTexture = resourceData.cameraColor;
                passData.screenMat = m_screenMaterial;
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
        }

        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            data.screenMat.SetTexture("_Test", data.cameraTexture);
        }
    }

    [Header("Only One")]
    public LayerMask selectiveMask;

    [Header("0 is no FOV change")]
    //public bool specialFov;

    public RenderObjects rendererOpaque;
    float renderFOV;

    public int uniqueFeatureTag;

    SelectiveRenderPass m_SelectiveRenderPass;

    SelectiveShaderClass m_SelectiveShaderPass;

    WriteToOutlineShaderPass m_WriteToOutlineShaderPass;

    public RenderPassEvent selective_injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public RenderPassEvent shader_injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public RenderPassEvent writing_injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

    public Material screen_material;

    public Material object_material;

    public Material testMaterial;
    
    //s
    /// <inheritdoc/>
    public override void Create()
    {
        m_SelectiveRenderPass = new SelectiveRenderPass();

        // Configures where the render pass should be injected.
        m_SelectiveRenderPass.renderPassEvent = selective_injectionPoint;


       // m_WriteToOutlineShaderPass = new WriteToOutlineShaderPass();
       // m_WriteToOutlineShaderPass.renderPassEvent = writing_injectionPoint;

        m_SelectiveShaderPass = new SelectiveShaderClass();

        m_SelectiveShaderPass.renderPassEvent = shader_injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //Debug.Log(uniqueFeatureTag.ToString() + m_SelectiveShaderPass);
        if (screen_material == null | object_material == null) return;

        //Debug.Log(Camera.main);
        m_SelectiveRenderPass.Setup(object_material, screen_material, selectiveMask.value, rendererOpaque.settings.cameraSettings.cameraFieldOfView, uniqueFeatureTag, 60);
        renderer.EnqueuePass(m_SelectiveRenderPass);

       // m_WriteToOutlineShaderPass.Setup(screen_material);
        //renderer.EnqueuePass(m_WriteToOutlineShaderPass);

        m_SelectiveShaderPass.Setup(screen_material, uniqueFeatureTag, testMaterial);
        renderer.EnqueuePass(m_SelectiveShaderPass);
    }
}
