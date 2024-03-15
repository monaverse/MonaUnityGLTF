using GLTF.Schema;
using Mona.SDK.Brains.Core.Brain;
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
            var runner = transform.GetComponent<IMonaBrainRunner>();
            if (runner == null) return;

            if (runner.BrainGraphs.Count > 0)
            {
                var ext = new MONA_Brains();
                ext.brains = new List<string>();

                for(var i = 0;i < runner.BrainGraphs.Count; i++)
                {
                    var brain = runner.BrainGraphs[i].ToJson();
                    var brainJson = Base64Encode(brain);
                    ext.brains.Add(brainJson);
                }

                for(var i = 0;i < runner.BrainUrls.Count; i++)
                {
                    ext.brains.Add(runner.BrainUrls[i]);
                }

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