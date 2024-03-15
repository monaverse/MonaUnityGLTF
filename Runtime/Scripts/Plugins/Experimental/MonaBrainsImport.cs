using GLTF.Schema;
using Mona.SDK.Brains.Core.Brain;
using Mona.SDK.Brains.Core.ScriptableObjects;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UnityGLTF.Plugins
{
    public class MonaBrainsImport : GLTFImportPlugin
    {
        public override string DisplayName => "MONA_brains";
        public override string Description => "Imports Mona Brains";
        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
        {
            return new MonaBrainsImportContext();
        }
    }
    
    public class MonaBrainsImportContext : GLTFImportPluginContext
    {
        public override void OnAfterImportNode(Node node, int nodeIndex, GameObject nodeObject)
        {
            Debug.Log($"{nameof(OnAfterImportNode)} {nodeObject}", nodeObject);

            if (node.Extensions == null) return;
            if (!node.Extensions.ContainsKey(MONA_BrainsFactory.EXTENSION_NAME)) return;

            var extension = (MONA_Brains)node.Extensions[MONA_BrainsFactory.EXTENSION_NAME];

            var runner = nodeObject.GetComponent<IMonaBrainRunner>();
            if (runner == null)
                runner = (IMonaBrainRunner)nodeObject.AddComponent<MonaBrainRunner>();
            for (var i = 0; i < extension.brains.Count; i++)
            {
                var brain = extension.brains[i];
                if (brain.IndexOf("ipfs:") == 0 || brain.IndexOf("http:") == 0)
                {
                    runner.BrainUrls.Add(brain.Replace("ipfs://", ""));
                }
                else { 
                    var json = Base64Decode(brain);
                    var graph = MonaBrainGraph.CreateFromJson(json);
#if UNITY_EDITOR
                    if (!AssetDatabase.IsValidFolder("Assets/Brains"))
                        AssetDatabase.CreateFolder("Assets", "Brains");

                    if (!AssetDatabase.IsValidFolder("Assets/Brains/Imported"))
                        AssetDatabase.CreateFolder("Assets/Brains", "Imported");
                    
                    var name = graph.Name;
                    if (string.IsNullOrEmpty(name))
                        name = graph.name;
                    /*
                    string[] guids = AssetDatabase.FindAssets("t:MonaBrainGraph", null);
                    var count = 0;
                    foreach (string guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        if (path.IndexOf("Assets/Brains/Imported/" + nodeObject.name + "_" + name) == 0)
                            count++;
                    }
                    var suffix = count > 0 ? count.ToString("000") : "";
                    */
                    AssetDatabase.CreateAsset(graph, "Assets/Brains/Imported/" + nodeObject.name + "_" + name + ".asset");
#endif
                    runner.BrainGraphs.Add(graph);
                }
            }
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}