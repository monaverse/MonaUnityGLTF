using GLTF.Schema;
using Mona.SDK.Brains.Core.Brain;
using Mona.SDK.Core.Assets.Interfaces;
using Mona.SDK.Core.Body;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGLTF.Plugins
{
    public class MonaCollidersExport: GLTFExportPlugin
    {
        public override string DisplayName => "MONA_Colliders";
        public override string Description => "Exports attached mona colliders";
        public override GLTFExportPluginContext CreateInstance(ExportContext context)
        {
            return new MonaCollidersExportContext();
        }
    }
    
    public class MonaCollidersExportContext : GLTFExportPluginContext
    {
        public override void AfterNodeExport(GLTFSceneExporter exporter, GLTFRoot gltfRoot, Transform transform, Node node)
        {
            var colliders = transform.GetComponents<Collider>();

            if (colliders.Length > 0)
            {
                var ext = new MONA_Colliders();
                ext.colliders = new List<MONACollider>();

                for (var i = 0; i < colliders.Length; i++)
                {
                    var collider = colliders[i];
                    if (collider is BoxCollider)
                    {
                        var mc = new MONACollider();
                        mc.ColliderType = MONAColliderType.Box;
                        mc.IsTrigger = collider.isTrigger;
                        mc.Center = ((BoxCollider)collider).center;
                        mc.Size = ((BoxCollider)collider).size;
                        ext.colliders.Add(mc);
                    }
                    else if (collider is SphereCollider)
                    {
                        var mc = new MONACollider();
                        mc.ColliderType = MONAColliderType.Sphere;
                        mc.IsTrigger = collider.isTrigger;
                        mc.Center = ((SphereCollider)collider).center;
                        mc.Radius = ((SphereCollider)collider).radius;
                        ext.colliders.Add(mc);
                    }
                    else if (collider is CapsuleCollider)
                    {
                        var mc = new MONACollider();
                        mc.ColliderType = MONAColliderType.Capsule;
                        mc.IsTrigger = collider.isTrigger;
                        mc.Center = ((CapsuleCollider)collider).center;
                        mc.Radius = ((CapsuleCollider)collider).radius;
                        mc.Height = ((CapsuleCollider)collider).height;
                        mc.Direction = ((CapsuleCollider)collider).direction;
                        ext.colliders.Add(mc);
                    }
                    else if (collider is MeshCollider)
                    {
                        var mc = new MONACollider();
                        mc.ColliderType = MONAColliderType.Mesh;
                        mc.IsTrigger = collider.isTrigger;
                        mc.IsConvex = ((MeshCollider)collider).convex;
                        ext.colliders.Add(mc);
                    }
                }

                if (node.Extensions == null) node.Extensions = new Dictionary<string, IExtension>();
                node.Extensions.Add(MONA_CollidersFactory.EXTENSION_NAME, ext);
                exporter.DeclareExtensionUsage(MONA_CollidersFactory.EXTENSION_NAME, false);
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}