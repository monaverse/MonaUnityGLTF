using GLTF.Schema;
using Mona.SDK.Brains.Core.Brain;
using Mona.SDK.Core.Assets.Interfaces;
using Mona.SDK.Core.Body;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Plugins
{
    public class MonaBrainsExport: GLTFExportPlugin
    {
        public override string DisplayName => "MONA_brains";
        public override string Description => "Exports attached mona brains";
        public override GLTFExportPluginContext CreateInstance(ExportContext context)
        {
            return new MonaBrainsExportContext();
        }
    }
    
    public class MonaBrainsExportContext : GLTFExportPluginContext
    {
        public override void AfterNodeExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Transform transform, Node node)
        {
            var ext = new MONA_Brains();
            var found = false;
            var runner = transform.GetComponent<IMonaBrainRunner>();
            if (runner != null)
            {
                if (runner.BrainGraphs.Count > 0)
                {
                    ext.brains = new List<string>();

                    for (var i = 0; i < runner.BrainGraphs.Count; i++)
                    {
                        var brain = runner.BrainGraphs[i].ToJson();
                        var brainJson = Base64Encode(brain);
                        ext.brains.Add(brainJson);
                    }

                    for (var i = 0; i < runner.BrainUrls.Count; i++)
                    {
                        ext.brains.Add(runner.BrainUrls[i]);
                    }
                }
                found = true;
            }

            var assetBehaviour = transform.GetComponent<IMonaAssetProviderBehaviour>();
            if(assetBehaviour != null)
            {
                /*if (assetBehaviour is IMonaAnimationAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.ANIMATION_ASSETS;
                else if (assetBehaviour is IMonaAudioAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.AUDIO_ASSETS;
                else if (assetBehaviour is IMonaAvatarAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.AVATAR_ASSETS;
                else
                */
                if (assetBehaviour is IMonaBodyAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.BODY_ASSETS;
                /*
                else if (assetBehaviour is IMonaMaterialAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.MATERIAL_ASSETS;
                else if (assetBehaviour is IMonaTextureAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.TEXTURE_ASSETS;
                else if (assetBehaviour is IMonaWearableAssetProviderBehaviour)
                    ext.asset = MONA_BrainsFactory.WEARABLE_ASSETS;
                */
                found = true;
            }

            var body = transform.GetComponent<IMonaBody>();
            if (body != null)
            {
                ext.body = true;
                found = true;
            }
            else
                ext.body = false;

            if (found)
            {
                if (node.Extensions == null) node.Extensions = new Dictionary<string, IExtension>();
                node.Extensions.Add(MONA_BrainsFactory.EXTENSION_NAME, ext);
                exporter.DeclareExtensionUsage(MONA_BrainsFactory.EXTENSION_NAME, false);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}