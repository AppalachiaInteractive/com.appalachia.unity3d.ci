using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Core;
using Appalachia.CI.Integration.Extensions;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using Appalachia.CI.Integration.Rider;
using Newtonsoft.Json;
using Unity.Profiling;
using UnityEditorInternal;
using UnityEngine;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class AssemblyDefinitionMetadata : IntegrationMetadata<AssemblyDefinitionMetadata>
    {
        #region Profiling And Tracing Markers

        private const string _PRF_PFX = nameof(AssemblyDefinitionMetadata) + ".";

        private static string[] _partExclusions;
        private static StringBuilder _asmdefNameBuilder;
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        private static readonly ProfilerMarker _PRF_FinalizeInternal =
            new(_PRF_PFX + nameof(FinalizeInternal));

        #endregion

        static AssemblyDefinitionMetadata()
        {
            IntegrationMetadataRegistry<AssemblyDefinitionMetadata>.Register(
                100,
                FindAllInternal,
                ProcessAll,
                FinalizeInternal
            );
        }

        private AssemblyDefinitionMetadata()
        {
        }

        public AssemblyDefinitionAsset asset;

        /*public AssemblyDefinitionImporter importer;*/
        public AssemblyDefinitionModel assetModel;
        public DotSettings dotSettings;
        public HashSet<AssemblyDefinitionReference> opportunities;
        public List<AssemblyDefinitionReference> references;
        public List<string> referenceStrings;
        public RepositoryMetadata repository;
        public string AssemblyCurrent;
        public string assemblyIdeal;
        public string csProjPath;
        public string filenameCurrent;
        public string rootNamespaceCurrent;
        public string rootNamespaceIdeal;

        private string _guid;
        private string _path;
        public override string Id => _guid;

        public override string Name => AssemblyCurrent;

        public override string Path => _path;

        public string DotSettingsPath => $"{csProjPath}.dotSettings";
        public string FilenameIdeal => AssemblyCurrent + ".asmdef";

        public static AssemblyDefinitionMetadata CreateNew(string path)
        {
            if (InstancesByPath.ContainsKey(path))
            {
                return InstancesByPath[path];
            }

            var newInstance = new AssemblyDefinitionMetadata();

            newInstance.Initialize(path);

            return newInstance;
        }

        public static bool operator ==(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator >(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return Comparer<AssemblyDefinitionMetadata>.Default.Compare(left, right) > 0;
        }

        public static bool operator >=(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return Comparer<AssemblyDefinitionMetadata>.Default.Compare(left, right) >= 0;
        }

        public static bool operator !=(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return !Equals(left, right);
        }

        public static bool operator <(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return Comparer<AssemblyDefinitionMetadata>.Default.Compare(left, right) < 0;
        }

        public static bool operator <=(AssemblyDefinitionMetadata left, AssemblyDefinitionMetadata right)
        {
            return Comparer<AssemblyDefinitionMetadata>.Default.Compare(left, right) <= 0;
        }

        private static IEnumerable<AssemblyDefinitionMetadata> FindAllInternal()
        {
            var asmdefs = AssetDatabaseManager.FindProjectPathsByExtension(".asmdef");

            foreach (var asmdef in asmdefs)
            {
                yield return CreateNew(asmdef);
            }
        }

        private static void FinalizeInternal()
        {
            using (_PRF_FinalizeInternal.Auto())
            {
                foreach (var instance in Instances)
                {
                    if (instance.repository == null)
                    {
                        instance.repository = RepositoryMetadata.FindByName(instance.Name);
                        instance.SetReferences();
                    }
                }
            }
        }

        public override bool Equals(AssemblyDefinitionMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return AssemblyCurrent == other.AssemblyCurrent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((AssemblyDefinitionMetadata) obj);
        }

        public override int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return 1;
            }

            if (ReferenceEquals(this, obj))
            {
                return 0;
            }

            return obj is AssemblyDefinitionMetadata other
                ? CompareTo(other)
                : throw new ArgumentException($"Object must be of type {nameof(AssemblyDefinitionMetadata)}");
        }

        public override int CompareTo(AssemblyDefinitionMetadata other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            void GetMatches(string name, out bool isAppalachiaMatch, out bool isUnityMatch)
            {
                isAppalachiaMatch = name.StartsWith("Appalachia");
                isUnityMatch = name.StartsWith("Unity") ||
                               name.StartsWith("com.unity") ||
                               name.StartsWith("TextMeshPro") ||
                               name.StartsWith("Cinemachine");
            }

            GetMatches(AssemblyCurrent,       out var appalachiaMatch,      out var unityMatch);
            GetMatches(other.AssemblyCurrent, out var otherAppalachiaMatch, out var otherUnityMatch);

            if (appalachiaMatch && !otherAppalachiaMatch)
            {
                return -1;
            }

            if (!appalachiaMatch && otherAppalachiaMatch)
            {
                return 1;
            }

            if (unityMatch && !otherUnityMatch)
            {
                return -1;
            }

            if (!unityMatch && otherUnityMatch)
            {
                return 1;
            }

            var assemblyCurrentComparison = string.Compare(
                AssemblyCurrent,
                other.AssemblyCurrent,
                StringComparison.Ordinal
            );
            if (assemblyCurrentComparison != 0)
            {
                return assemblyCurrentComparison;
            }

            return string.Compare(Path, other.Path, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return AssemblyCurrent != null ? AssemblyCurrent.GetHashCode() : 0;
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return AssemblyCurrent;
            }
        }

        public override void InitializeForAnalysis()
        {
            SetReferences();
        }

        public int GetAssemblyReferenceLevel()
        {
            var name = AssemblyCurrent.ToLowerInvariant();
            var result = 0;

            if (name.EndsWith("examples") ||
                name.EndsWith("example") ||
                name.EndsWith("samples") ||
                name.EndsWith("sample") ||
                name.EndsWith("demos") ||
                name.EndsWith("demo") ||
                name.EndsWith("tests") ||
                name.EndsWith("tests.playmode") ||
                name.EndsWith("codegen"))
            {
                result += 10000;
            }

            if ((name.StartsWith("appalachia.editor") || name.EndsWith(".editor")) &&
                (name != "unity.editorcoroutines.editor") &&
                (name != "amplify.shader.editor"))
            {
                result += 100;
            }

            if (name.StartsWith("unity."))
            {
                result += 0;
                return result;
            }

            if (name.StartsWith("unityeditor."))
            {
                result += 50;
                return result;
            }

            if (name.StartsWith("appalachia."))
            {
                if (name.StartsWith("appalachia.utility"))
                {
                    result += 3;
                    return result;
                }

                if (name.StartsWith("appalachia.ci.constants"))
                {
                    result += 4;
                    return result;
                }

                if (name.StartsWith("appalachia.ci.integration"))
                {
                    result += 5;
                    return result;
                }

                if (name.StartsWith("appalachia.ci"))
                {
                    result += 10;
                    return result;
                }

                if (name.StartsWith("appalachia.core"))
                {
                    result += 20;
                    return result;
                }

                if (name.StartsWith("appalachia.editing"))
                {
                    result += 50;
                    return result;
                }

                if (name.StartsWith("appalachia.jobs"))
                {
                    result += 70;
                    return result;
                }

                if (name.StartsWith("appalachia.audio"))
                {
                    result += 90;
                    return result;
                }

                if (name.StartsWith("appalachia.globals"))
                {
                    result += 110;
                    return result;
                }

                if (name.StartsWith("appalachia.spatial") || name.StartsWith("appalachia.simulation"))
                {
                    result += 130;
                    return result;
                }

                if (name.StartsWith("appalachia.rendering"))
                {
                    result += 150;
                    return result;
                }

                if (name.StartsWith("appalachia.ui"))
                {
                    result += 170;
                    return result;
                }

                if (name.StartsWith("appalachia.koc") || name.StartsWith("appalachia.editor"))
                {
                    result += 200;
                    return result;
                }

                result += 250;
                return result;
            }

            result += 0;
            return result;
        }

        public int GetOpportunityCutoffLevel()
        {
            return 50;
        }

        public void Initialize(string path)
        {
            if (_partExclusions == null)
            {
                _partExclusions = new[] {"src", "dist", "asmdef", "Runtime", "Scripts"};
            }

            _path = path.ToRelativePath();

            readOnly = !_path.StartsWith("Assets");

            _guid = AssemblyDefinitionModel.GUID_PREFIX + AssetDatabaseManager.AssetPathToGUID(_path);

            if (string.IsNullOrWhiteSpace(_guid))
            {
                throw new NotSupportedException(_path);
            }

            filenameCurrent = AppaPath.GetFileName(_path);

            LoadAsset(_path);

            var directoryPath = AppaPath.GetDirectoryName(_path);
            directory = new AppaDirectoryInfo(directoryPath);
            repository = ProjectLocations.GetAssetRepository(_path);
            references = new List<AssemblyDefinitionReference>();
            opportunities = new HashSet<AssemblyDefinitionReference>();
            referenceStrings = new List<string>();

            if (_asmdefNameBuilder == null)
            {
                _asmdefNameBuilder = new StringBuilder();
            }

            _asmdefNameBuilder.Clear();

            var asmDefDirectory = AppaPath.GetDirectoryName(_path).Replace("\\", "/");

            var parts = asmDefDirectory.Split('/');

            for (var partIndex = 1; partIndex < parts.Length; partIndex++)
            {
                var part = parts[partIndex];
                var exclude = false;
                for (var exclusionIndex = 0; exclusionIndex < _partExclusions.Length; exclusionIndex++)
                {
                    var partExclusion = _partExclusions[exclusionIndex];
                    if (partExclusion.ToLowerInvariant() == part.ToLowerInvariant())
                    {
                        exclude = true;
                    }
                }

                if (!exclude)
                {
                    _asmdefNameBuilder.Append(part + '.');
                }
            }

            assemblyIdeal = _asmdefNameBuilder.ToString().Trim('.');
            rootNamespaceIdeal = null;

            if (assetModel == null)
            {
                return;
            }

            csProjPath = $"{assetModel.name}.csproj";

            LoadDotSettings();

            AssemblyCurrent = assetModel.name;
            rootNamespaceCurrent = assetModel.rootNamespace;
        }

        public void SaveDotSettingsFile(bool testFile)
        {
            var outputPath = GetFilePath(DotSettingsPath, testFile);

            var newContent = dotSettings.ToXml();

            AppaFile.WriteAllText(outputPath, newContent);
        }

        public void SaveFile(bool testFile, bool reimport)
        {
            if (references.Count != referenceStrings.Count)
            {
                throw new NotSupportedException("Make sure you are not losing references!");
            }

            var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            settings.NullValueHandling = NullValueHandling.Include;

            var outputPath = GetFilePath(Path, testFile);

            assetModel.CheckBeforeWrite();

            var text = JsonConvert.SerializeObject(assetModel, settings);

            AppaFile.WriteAllText(outputPath, text);

            UnityEditor.EditorUtility.SetDirty(asset);

            if (reimport)
            {
                LoadAsset(_path);
                SetReferences();
            }
        }

        public void SetReferences()
        {
            references.Clear();

            if ((referenceStrings == null) || (referenceStrings.Count == 0))
            {
                LoadAsset(_path);
            }

            for (var index = 0; index < referenceStrings.Count; index++)
            {
                var referenceString = referenceStrings[index];
                if (referenceString == null)
                {
                    Debug.LogWarning($"Reference string [{index}] is null for [{Name}].");
                    continue;
                }

                AssemblyDefinitionReference reference;

                if (!InstancesByID.ContainsKey(referenceString))
                {
                    reference = new AssemblyDefinitionReference(referenceString);
                }
                else
                {
                    reference = new AssemblyDefinitionReference(
                        InstancesByID[referenceString],
                        referenceString
                    );
                }

                references.Add(reference);
            }
        }

        protected override IEnumerable<string> GetIds()
        {
            yield return Id;
            yield return AssemblyCurrent;
        }

        private void LoadAsset(string assemblyDefinitionPath)
        {
            asset = AssetDatabaseManager.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);

            if (asset == null)
            {
                return;
            }

            assetModel = JsonConvert.DeserializeObject<AssemblyDefinitionModel>(asset.text);
            referenceStrings = assetModel.references?.ToList() ?? new List<string>();
        }

        private void LoadDotSettings()
        {
            if (!AppaFile.Exists(DotSettingsPath))
            {
                dotSettings = new DotSettings(null);
            }
            else
            {
                var dotSettingsText = AppaFile.ReadAllLines(DotSettingsPath);
                dotSettings = new DotSettings(dotSettingsText);
            }
        }
    }
}
