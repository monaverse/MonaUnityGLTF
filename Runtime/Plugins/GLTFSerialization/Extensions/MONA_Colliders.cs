using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GLTF.Schema
{
	public enum MONAColliderType { Box, Sphere, Capsule, Mesh }

	public class MONACollider
    {
		public MONAColliderType ColliderType;
		public bool IsTrigger;
		public bool IsConvex;
		public Vector3 Center;
		public Vector3 Size;
		public float Radius;
		public float Height;
		public int Direction;

		public static MONACollider CreateFromJson(string json)
		{
			var collider = new MONACollider();
			JsonUtility.FromJsonOverwrite(json, collider);
			return collider;
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}
	}

	public class MONA_Colliders : IExtension
	{
		public List<MONACollider> colliders;

		public JProperty Serialize()
		{
			var arr = new JArray();
			if (colliders != null)
			{
				for (var i = 0; i < colliders.Count; i++)
					arr.Add(colliders[i].ToJson());
			}

			var jobj = new JObject();
			jobj.Add(new JProperty("colliders", arr));
			return new JProperty(MONA_CollidersFactory.EXTENSION_NAME, jobj);
		}

		public IExtension Clone(GLTFRoot root)
		{
			return new MONA_Colliders
			{
				colliders = colliders,
			};
		}
	}

	public class MONA_CollidersFactory : ExtensionFactory
	{
		public const string EXTENSION_NAME = "MONA_colliders";

		public MONA_CollidersFactory()
		{
			ExtensionName = EXTENSION_NAME;
		}

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			if (extensionToken != null)
			{
				var extension = new MONA_Colliders();
				var children = extensionToken.Value[nameof(MONA_Colliders.colliders)]?.Children();
				var colliders = new List<MONACollider>();

				foreach (var child in children)
					colliders.Add(MONACollider.CreateFromJson(child.ToString()));

				extension.colliders = colliders;
				return extension;
			}

			return null;
		}
	}

}
