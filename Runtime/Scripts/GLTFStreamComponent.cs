using GLTF;
using GLTF.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityGLTF.Loader;
using UnityGLTF.Plugins;

namespace UnityGLTF
{
	public class GLTFStreamLoader : IDataLoader
	{
		private Stream _stream;

		public GLTFStreamLoader(Stream stream)
		{
			_stream = stream;
		}
		public async Task<Stream> LoadStreamAsync(string relativeFilePath)
		{
			return _stream;
		}
	}
	/// <summary>
	/// Component to load a GLTF scene with
	/// </summary>
	public class GLTFStreamComponent : MonoBehaviour
	{
		public System.IO.MemoryStream GLTFStream = null;
		public bool Multithreaded = true;
		public bool UseStream = false;
		public bool AppendStreamingAssets = true;
		public bool PlayAnimationOnLoad = true;
		[Tooltip("Hide the scene object during load, then activate it when complete")]
		public bool HideSceneObjDuringLoad = false;
        public ImporterFactory Factory = null;
        public UnityAction onLoadComplete;

		public Action<GameObject, ExceptionDispatchInfo> OnImportComplete;

#if UNITY_ANIMATION
		public IEnumerable<Animation> Animations { get; private set; }
#endif

		[SerializeField]
		private bool loadOnStart = true;

		[SerializeField] private int RetryCount = 10;
		[SerializeField] private float RetryTimeout = 2.0f;
		private int numRetries = 0;


		public int MaximumLod = 300;
		public int Timeout = 8;
		public GLTFSceneImporter.ColliderType Collider = GLTFSceneImporter.ColliderType.None;
		public GameObject LastLoadedScene { get; private set; } = null;

		[SerializeField]
		private Shader shaderOverride = null;

		[Header("Import Settings")]
		public GLTFImporterNormals ImportNormals = GLTFImporterNormals.Import;
		public GLTFImporterNormals ImportTangents = GLTFImporterNormals.Import;
		public bool SwapUVs = false;
		[Tooltip("Blend shape frame weight import multiplier. Default is 1. For compatibility with some FBX animations you may need to use 100.")]
		public BlendShapeFrameWeightSetting blendShapeFrameWeight = new BlendShapeFrameWeightSetting(BlendShapeFrameWeightSetting.MultiplierOption.Multiplier1);

		[Tooltip("When false, the index of the BlendShape is used as name.")]
		[SerializeField] public bool importBlendShapeNames = true;
		
		public AnimationMethod AnimationMethod;
		public bool AnimationLoopPose = true;

		public async Task Load()
		{
			var importOptions = new ImportOptions
			{
				AsyncCoroutineHelper = gameObject.GetComponent<AsyncCoroutineHelper>() ?? gameObject.AddComponent<AsyncCoroutineHelper>(),
				ImportNormals = ImportNormals,
				ImportTangents = ImportTangents,
				SwapUVs = SwapUVs,
				AnimationMethod = AnimationMethod,
				AnimationLoopPose = AnimationLoopPose,
				ImportBlendShapeNames = importBlendShapeNames,
				BlendShapeFrameWeight = blendShapeFrameWeight
			};
			
			var settings = GLTFSettings.GetOrCreateSettings();
			importOptions.ImportContext = new GLTFImportContext(settings);

			var plugins = importOptions.ImportContext.Plugins;
			
			GLTFSceneImporter sceneImporter = null;
			try
			{
				if (!Factory) Factory = ScriptableObject.CreateInstance<DefaultImporterFactory>();

				importOptions.DataLoader = new GLTFStreamLoader(GLTFStream);

				GLTFRoot gLTFRoot;
				GLTFParser.ParseJson(GLTFStream, out gLTFRoot);
				sceneImporter = new UnityGLTF.GLTFSceneImporter(gLTFRoot, GLTFStream, importOptions);

				//sceneImporter.SceneParent = null;
				sceneImporter.Collider = Collider;
				sceneImporter.MaximumLod = MaximumLod;
				sceneImporter.Timeout = Timeout;
				sceneImporter.IsMultithreaded = Multithreaded;
				sceneImporter.CustomShaderName = shaderOverride ? shaderOverride.name : null;

				GLTFStream.Position = 0; // Make sure the read position is changed back to the beginning of the file

									 // for logging progress
				await sceneImporter.LoadSceneAsync(
					showSceneObj:!HideSceneObjDuringLoad,
					onLoadComplete:LoadCompleteAction
					// ,progress: new Progress<ImportProgress>(
					// 	p =>
					// 	{
					// 		Debug.Log("Progress: " + p);
					// 	})
				);


				// Override the shaders on all materials if a shader is provided
				if (shaderOverride != null)
				{
					Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
					foreach (Renderer renderer in renderers)
					{
						renderer.sharedMaterial.shader = shaderOverride;
					}
				}

				LastLoadedScene = sceneImporter.LastLoadedScene;
				
				if (HideSceneObjDuringLoad)
					LastLoadedScene.SetActive(true);

#if UNITY_ANIMATION
				Animations = sceneImporter.LastLoadedScene.GetComponents<Animation>();

				if (PlayAnimationOnLoad && Animations.Any())
				{
					Animations.First().Play();
				}
#endif
			}
			finally
			{
				if(importOptions.DataLoader != null)
				{
					sceneImporter?.Dispose();
					sceneImporter = null;
					importOptions.DataLoader = null;
				}
			}
		}

		private void LoadCompleteAction(GameObject obj, ExceptionDispatchInfo exceptionDispatchInfo)
		{
			onLoadComplete?.Invoke();
			OnImportComplete?.Invoke(obj, exceptionDispatchInfo);
		}
	}
}
