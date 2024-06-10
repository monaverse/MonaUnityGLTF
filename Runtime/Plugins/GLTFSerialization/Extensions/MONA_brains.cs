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
		public int sync_type;
		public bool disable_on_load;

		public string chain_id;
		public string token_id;
		public string contract;

		public string author;
		public string author_address;
		public string package_name;
		public string version;
		public string license;

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
			jobj.Add(new JProperty("sync_type", sync_type));
			jobj.Add(new JProperty("disable_on_load", disable_on_load));

			jobj.Add(new JProperty("chain_id", chain_id));
			jobj.Add(new JProperty("token_id", token_id));
			jobj.Add(new JProperty("contract", contract));

			jobj.Add(new JProperty("author", author));
			jobj.Add(new JProperty("author_address", author_address));
			jobj.Add(new JProperty("package_name", package_name));
			jobj.Add(new JProperty("version", version));
			jobj.Add(new JProperty("license", license));

			return new JProperty(MONA_BrainsFactory.EXTENSION_NAME, jobj);
		}

		public IExtension Clone(GLTFRoot root)
		{
			return new MONA_Brains
			{
				brains = brains,
				asset = asset,
				body = body,
				sync_type = sync_type,
				disable_on_load = disable_on_load,
				chain_id = chain_id,
				token_id = token_id,
				contract = contract,
				author = author,
				author_address = author_address,
				package_name = package_name,
				version = version,
				license = license
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

				var sync_type = extensionToken.Value[nameof(MONA_Brains.sync_type)] != null ? int.Parse(extensionToken.Value[nameof(MONA_Brains.sync_type)].ToString()) : 0;
				var disable_on_load = extensionToken.Value[nameof(MONA_Brains.disable_on_load)] != null ? bool.Parse(extensionToken.Value[nameof(MONA_Brains.disable_on_load)].ToString()) : false;

				var chain_id = extensionToken.Value[nameof(MONA_Brains.chain_id)] != null ? extensionToken.Value[nameof(MONA_Brains.chain_id)].ToString() : "";
				var contract = extensionToken.Value[nameof(MONA_Brains.contract)] != null ? extensionToken.Value[nameof(MONA_Brains.contract)].ToString() : "";
				var token_id = extensionToken.Value[nameof(MONA_Brains.token_id)] != null ? extensionToken.Value[nameof(MONA_Brains.token_id)].ToString() : "";
				var package_name = extensionToken.Value[nameof(MONA_Brains.package_name)] != null ? extensionToken.Value[nameof(MONA_Brains.package_name)].ToString() : "";
				var version = extensionToken.Value[nameof(MONA_Brains.version)] != null ? extensionToken.Value[nameof(MONA_Brains.version)].ToString() : "";

				var author = extensionToken.Value[nameof(MONA_Brains.author)] != null ? extensionToken.Value[nameof(MONA_Brains.author)].ToString() : "";
				var author_address = extensionToken.Value[nameof(MONA_Brains.author_address)] != null ? extensionToken.Value[nameof(MONA_Brains.author_address)].ToString() : "";
				var license = extensionToken.Value[nameof(MONA_Brains.license)] != null ? extensionToken.Value[nameof(MONA_Brains.license)].ToString() : "";

				var brains = new List<string>();

				foreach (var child in children)
					brains.Add(child.ToString());

				extension.brains = brains;
				extension.asset = asset;
				extension.body = body;
				extension.sync_type = sync_type;
				extension.disable_on_load = disable_on_load;

				extension.chain_id = chain_id;
				extension.contract = contract;
				extension.token_id = token_id;
				extension.package_name = package_name;
				extension.version = version;

				extension.author = author;
				extension.author_address = author_address;
				extension.license = license;

				return extension;
			}

			return null;
		}
	}

}
