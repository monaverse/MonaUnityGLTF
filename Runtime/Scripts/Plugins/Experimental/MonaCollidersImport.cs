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
    public class MonaCollidersImport : GLTFImportPlugin
    {
        public override string DisplayName => "MONA_Colliders";
        public override string Description => "Imports Mona Colliders";
        public override GLTFImportPluginContext CreateInstance(GLTFImportContext context)
        {
            return new MonaCollidersImportContext();
        }
    }

    public class MonaCollidersImportContext : GLTFImportPluginContext
    {
        public override void OnAfterImportNode(Node node, int nodeIndex, GameObject nodeObject)
        {
            //Debug.Log($"{nameof(OnAfterImportNode)} {nodeObject}", nodeObject);

            if (node.Extensions == null) return;
            if (!node.Extensions.ContainsKey(MONA_CollidersFactory.EXTENSION_NAME)) return;

            var extension = (MONA_Colliders)node.Extensions[MONA_CollidersFactory.EXTENSION_NAME];

            if (extension.colliders != null && extension.colliders.Count > 0)
            {

                for (var i = 0; i < extension.colliders.Count; i++)
                {
                    var collider = extension.colliders[i];
                    switch((MONAColliderType)collider.ColliderType)
                    {
                        case MONAColliderType.Box:
                            var box = nodeObject.AddComponent<BoxCollider>();
                            box.center = collider.Center;
                            box.size = collider.Size;
                            box.isTrigger = collider.IsTrigger;
                            break;
                        case MONAColliderType.Sphere:
                            var sphere = nodeObject.AddComponent<SphereCollider>();
                            sphere.center = collider.Center;
                            sphere.radius = collider.Radius;
                            sphere.isTrigger = collider.IsTrigger;
                            break;
                        case MONAColliderType.Capsule:
                            var capsule = nodeObject.AddComponent<CapsuleCollider>();
                            capsule.center = collider.Center;
                            capsule.radius = collider.Radius;
                            capsule.height = collider.Height;
                            capsule.direction = collider.Direction;
                            capsule.isTrigger = collider.IsTrigger;
                            break;
                        case MONAColliderType.Mesh:
                            var mesh = nodeObject.AddComponent<MeshCollider>();
                            mesh.isTrigger = collider.IsTrigger;
                            mesh.convex = collider.IsConvex;
                            break;
                    }    
                }
            }

        }

    }
}