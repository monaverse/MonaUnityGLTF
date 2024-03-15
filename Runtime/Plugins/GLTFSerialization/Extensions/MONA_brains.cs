using GLTF.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GLTF.Schema
{

	public class MONA_Brains : IExtension
	{
		public List<string> brains;

		public JProperty Serialize()
		{
			var arr = new JArray();
			for (var i = 0; i < brains.Count; i++)
				arr.Add(brains[i]);

			return new JProperty(MONA_BrainsFactory.EXTENSION_NAME, new JObject(new JProperty("brains", arr)));
		}

		public IExtension Clone(GLTFRoot root)
		{
			return new MONA_Brains
			{
				brains = brains
			};
		}
	}

	public class MONA_BrainsFactory : ExtensionFactory
	{
		public const string EXTENSION_NAME = "MONA_brains";

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
				var brains = new List<string>();
				foreach (var child in children)
					brains.Add(child.ToString());
				extension.brains = brains;
				return extension;
			}

			return null;
		}
	}

}
