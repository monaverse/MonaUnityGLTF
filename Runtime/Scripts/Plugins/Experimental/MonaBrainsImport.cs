using GLTF.Schema;
using Mona.SDK.Brains.Core.Brain;
using Mona.SDK.Core.Assets.Behaviours;
using Mona.SDK.Brains.Core.ScriptableObjects;
using System;
using System.Text;
using System.Collections.Generic;
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

        private Dictionary<string, MonaBrainGraph> _graphInstances = new Dictionary<string, MonaBrainGraph>();

        public override void OnAfterImportNode(Node node, int nodeIndex, GameObject nodeObject)
        {
            if (node.Extensions == null) return;
            if (!node.Extensions.ContainsKey(MONA_BrainsFactory.EXTENSION_NAME)) return;

            var extension = (MONA_Brains)node.Extensions[MONA_BrainsFactory.EXTENSION_NAME];

            if (extension.brains != null && extension.brains.Count > 0)
            {
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
                    else
                    {
                        var json = Base64Decode(brain);
#if UNITY_EDITOR
                        if (!AssetDatabase.IsValidFolder("Assets/Brains"))
                            AssetDatabase.CreateFolder("Assets", "Brains");

                        if (!AssetDatabase.IsValidFolder("Assets/Brains/Imported"))
                            AssetDatabase.CreateFolder("Assets/Brains", "Imported");
#endif
                        MonaBrainGraph graph;
                        if (!_graphInstances.ContainsKey(json))
                        {
                            graph = MonaBrainGraph.CreateFromJson(json);
                            _graphInstances.Add(json, graph);

                            var parent = nodeObject.transform;
                            var pathBuilder = new StringBuilder();
                            while (parent != null)
                            {
                                pathBuilder.Append(parent.name + "_");
                                parent = parent.parent;
                            }
                            var path = pathBuilder.ToString();

                            var name = graph.Name;
                            if (string.IsNullOrEmpty(name))
                                name = graph.name;

#if UNITY_EDITOR
                            AssetDatabase.CreateAsset(graph, "Assets/Brains/Imported/" + path + name + ".asset");
#endif
                        }
                        else
                        {
                            graph = _graphInstances[json];
                        }

                        runner.AddBrainGraph(graph);
                    }
                }
            }

            var id = nodeObject.AddComponent<MonaBodyId>();
            id.ChainId = extension.chain_id;
            id.Contract = extension.contract;
            id.TokenId = extension.token_id;
            id.PackageName = extension.package_name;
            id.Version = extension.version;

            var author = nodeObject.AddComponent<MonaBodyAuthor>();
            author.Author = extension.author;
            author.AuthorAddress = extension.author_address;
            author.License = extension.license;

            if(!string.IsNullOrEmpty(extension.asset))
            {
                switch(extension.asset)
                {
                    //case MONA_BrainsFactory.ANIMATION_ASSETS: nodeObject.AddComponent<MonaAnimationAssetProviderBehaviour>(); break;
                    //case MONA_BrainsFactory.AUDIO_ASSETS: nodeObject.AddComponent<MonaAudioAssetProviderBehaviour>(); break;
                    //case MONA_BrainsFactory.AVATAR_ASSETS: nodeObject.AddComponent<MonaAvatarAssetProviderBehaviour>(); break;
                    case MONA_BrainsFactory.BODY_ASSETS: nodeObject.AddComponent<MonaBodyAssetProviderBehaviour>(); break;
                    //case MONA_BrainsFactory.MATERIAL_ASSETS: nodeObject.AddComponent<MonaMateriralAssetProviderBehaviour>(); break;
                    //case MONA_BrainsFactory.TEXTURE_ASSETS: nodeObject.AddComponent<MonaTextureAssetProviderBehaviour>(); break;
                    //case MONA_BrainsFactory.WEARABLE_ASSETS: nodeObject.AddComponent<MonaWearableAssetProviderBehaviour>(); break;
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