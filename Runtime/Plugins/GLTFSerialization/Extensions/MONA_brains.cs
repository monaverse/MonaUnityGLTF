using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GLTF.Schema
{

	public class MONA_Brains : IExtension
	{
		public List<string> brains;
		public string asset;
		public bool body;

		public JProperty Serialize()
		{
			var arr = new JArray();
			if (brains != null)
			{
				for (var i = 0; i < brains.Count; i++)
					arr.Add(brains[i]);
			}

			var jobj = new JObject();
			jobj.Add(new JProperty("brains", arr));
			jobj.Add(new JProperty("asset", asset));
			jobj.Add(new JProperty("body", body));
			return new JProperty(MONA_BrainsFactory.EXTENSION_NAME, jobj);
		}

		public IExtension Clone(GLTFRoot root)
		{
			return new MONA_Brains
			{
				brains = brains,
				asset = asset,
				body = body
			};
		}
	}

	public class MONA_BrainsFactory : ExtensionFactory
	{
		public const string EXTENSION_NAME = "MONA_brains";

		public const string ANIMATION_ASSETS = "animation";
		public const string AUDIO_ASSETS = "audio";
		public const string AVATAR_ASSETS = "avatar";
		public const string BODY_ASSETS = "body";
		public const string MATERIAL_ASSETS = "material";
		public const string TEXTURE_ASSETS = "texture";
		public const string WEARABLE_ASSETS = "wearables";

		public MONA_BrainsFactory()
		{
			ExtensionName = EXTENSION_NAME;
		}

		public override IExtension Deserialize(GLTFRoot root, JProperty extensionToken)
		{
			if (extensionToken != null)
			{
				var extension = new MONA_Brains();
				var children = extensionToken.Value[nameof(MONA_Brains.brains)]?.Children();
				var asset = extensionToken.Value[nameof(MONA_Brains.asset)].ToString();
				var body = bool.Parse(extensionToken.Value[nameof(MONA_Brains.body)].ToString());
				var brains = new List<string>();

				foreach (var child in children)
					brains.Add(child.ToString());

				extension.brains = brains;
				extension.asset = asset;
				extension.body = body;
				return extension;
			}

			return null;
		}
	}

}
