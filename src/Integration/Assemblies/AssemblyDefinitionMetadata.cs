using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Appalachia.CI.Integration.Analysis;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using Appalachia.CI.Integration.Rider;
using Newtonsoft.Json;
using Unity.Profiling;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    public class AssemblyDefinitionMetadata : IComparable<AssemblyDefinitionMetadata>,
                                              IComparable,
                                              IEquatable<AssemblyDefinitionMetadata>,
                                              IAnalysisTarget
    {
        private const string _PRF_PFX = nameof(AssemblyDefinitionMetadata) + ".";
        private static Dictionary<string, AssemblyDefinitionMetadata> _instances;
        private static Dictionary<string, AssemblyDefinitionMetadata> _instancesByGuid;
        private static string[] _partExclusions;
        private static StringBuilder _asmdefNameBuilder;
        private static readonly ProfilerMarker _PRF_ToString = new(_PRF_PFX + nameof(ToString));

        public static IEnumerable<AssemblyDefinitionMetadata> Instances => _instances.Values;

        private AssemblyDefinitionMetadata()
        {
        }

        private static bool _allPrepared;

        public AppaDirectoryInfo directory;

        public AssemblyDefinitionAnalysisMetadata analysis;
        public AssemblyDefinitionAsset asset;
        public AssemblyDefinitionImporter importer;
        public AssemblyDefinitionModel assetModel;

        public bool readOnly;
        public DotSettings dotSettings;
        public List<AssemblyDefinitionReferenceMetadata> opportunities;
        public List<AssemblyDefinitionReferenceMetadata> references;
        public List<RepositoryDependencyMetadata> dependencies;
        public List<string> referenceStrings;

        public RepositoryDirectoryMetadata repository;
        public string assembly_current;
        public string assembly_ideal;
        public string csProjPath;
        public string filename_current;
        public string guid;
        public string path;
        public string root_namespace_current;
        public string root_namespace_ideal;

        public Color IssueColor { get; set; }

        public bool IsAppalachia => filename_current.StartsWith("Appalachia");
        public bool IsAsset => path.StartsWith("Asset");
        public bool IsLibrary => path.StartsWith("Library");

        public bool IsPackage => path.StartsWith("Package");
        public string dotSettingsPath => $"{csProjPath}.dotSettings";
        public string filename_ideal => assembly_current + ".asmdef";
        public string Name => assembly_current;

        public int GetAssemblyDependencyLevel()
        {
            var name = assembly_current;
            var result = 0;

            if (name.EndsWith("Examples") ||
                name.EndsWith("Example") ||
                name.EndsWith("Tests") ||
                name.EndsWith("Tests.Playmode") ||
                name.EndsWith("CodeGen"))
            {
                result += 10000;
            }

            if ((name.StartsWith("Appalachia.Editor") || name.EndsWith(".Editor")) &&
                (name != "Unity.EditorCoroutines.Editor") &&
                (name != "Amplify.Shader.Editor"))
            {
                result += 100;
            }

            if (name.StartsWith("Unity."))
            {
                result += 0;
                return result;
            }

            if (name.StartsWith("UnityEditor."))
            {
                result += 50;
                return result;
            }

            if (name.StartsWith("Appalachia."))
            {
                if (name.StartsWith("Appalachia.Utility"))
                {
                    result += 0;
                    return result;
                }

                if (name.StartsWith("Appalachia.CI.Integration"))
                {
                    result += 1;
                    return result;
                }

                if (name.StartsWith("Appalachia.CI"))
                {
                    result += 10;
                    return result;
                }

                if (name.StartsWith("Appalachia.Core"))
                {
                    result += 20;
                    return result;
                }

                if (name.StartsWith("Appalachia.Editing"))
                {
                    result += 50;
                    return result;
                }

                if (name.StartsWith("Appalachia.Jobs"))
                {
                    result += 70;
                    return result;
                }

                if (name.StartsWith("Appalachia.Audio"))
                {
                    result += 90;
                    return result;
                }

                if (name.StartsWith("Appalachia.Globals"))
                {
                    result += 110;
                    return result;
                }

                if (name.StartsWith("Appalachia.Spatial") || name.StartsWith("Appalachia.Simulation"))
                {
                    result += 130;
                    return result;
                }

                if (name.StartsWith("Appalachia.Rendering"))
                {
                    result += 150;
                    return result;
                }

                if (name.StartsWith("Appalachia.UI"))
                {
                    result += 170;
                    return result;
                }

                if (name.StartsWith("Appalachia.KOC") || name.StartsWith("Appalachia.Editor"))
                {
                    result += 200;
                    return result;
                }

                result += 250;
                return result;
            }

            result += 2;
            return result;
        }

        public int GetOpportunityCutoffLevel()
        {
            return 50;
        }

        public void Initialize(string assemblyDefinitionPath)
        {
            if (_partExclusions == null)
            {
                _partExclusions = new[] {"src", "dist", "asmdef", "Runtime", "Scripts"};
            }

            IssueColor = Color.clear;
            readOnly = !assemblyDefinitionPath.StartsWith("Assets");

            path = assemblyDefinitionPath;
            guid = AssemblyDefinitionModel.GUID_PREFIX + AssetDatabaseManager.AssetPathToGUID(path);
            filename_current = AppaPath.GetFileName(assemblyDefinitionPath);

            importer = (AssemblyDefinitionImporter) AssetImporter.GetAtPath(assemblyDefinitionPath);
            asset = AssetDatabaseManager.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);
            var directoryPath = AppaPath.GetDirectoryName(assemblyDefinitionPath);
            directory = new AppaDirectoryInfo(directoryPath);
            repository = ProjectLocations.GetAssetRepository(path);
            references = new List<AssemblyDefinitionReferenceMetadata>();
            referenceStrings = new List<string>();

            if (_asmdefNameBuilder == null)
            {
                _asmdefNameBuilder = new StringBuilder();
            }

            _asmdefNameBuilder.Clear();

            var asmDefDirectory = AppaPath.GetDirectoryName(assemblyDefinitionPath).Replace("\\", "/");

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

            assembly_ideal = _asmdefNameBuilder.ToString().Trim('.');
            root_namespace_ideal = null;

            assetModel = JsonConvert.DeserializeObject<AssemblyDefinitionModel>(asset.text);

            csProjPath = $"{assetModel.name}.csproj";

            LoadDotSettings();

            assembly_current = assetModel.name;
            root_namespace_current = assetModel.rootNamespace;

            referenceStrings = assetModel.references?.ToList() ?? new List<string>();
        }

        public void InitializeAnalysis()
        {
            // need to finish initialize method before this is called.
            analysis = new AssemblyDefinitionAnalysisMetadata(this);
        }

        public void Reanalyze()
        {
            ClearAnalysisResults();
            InitializeAnalysis();
        }

        public void SaveDotSettingsFile(bool testFile)
        {
            var outputPath = GetFilePath(dotSettingsPath, testFile);

            var newContent = dotSettings.ToXml();

            AppaFile.WriteAllText(outputPath, newContent);
        }

        public void SaveFile(bool testFile, bool reimport)
        {
            var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            settings.NullValueHandling = NullValueHandling.Include;

            var outputPath = GetFilePath(path, testFile);

            assetModel.CheckBeforeWrite();

            var text = JsonConvert.SerializeObject(assetModel, settings);

            AppaFile.WriteAllText(outputPath, text);

            EditorUtility.SetDirty(asset);

            if (reimport)
            {
                AssetDatabaseManager.ImportAsset(outputPath);
            }
        }

        public void SetReferences()
        {
            references.Clear();

            foreach (var referenceString in referenceStrings)
            {
                AssemblyDefinitionReferenceMetadata reference;

                if (!_instancesByGuid.ContainsKey(referenceString))
                {
                    reference = new AssemblyDefinitionReferenceMetadata(referenceString);
                }
                else
                {
                    reference = new AssemblyDefinitionReferenceMetadata(
                        _instancesByGuid[referenceString],
                        referenceString
                    );
                }

                references.Add(reference);
            }
        }

        public void ClearAnalysisResults()
        {
            IssueColor = Color.clear;
            LoadDotSettings();

            repository.ClearAnalysisResults();

            references.Clear();
            dependencies.Clear();
            opportunities?.Clear();
        }

        public int CompareTo(object obj)
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

        public int CompareTo(AssemblyDefinitionMetadata other)
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

            GetMatches(assembly_current,       out var appalachiaMatch,      out var unityMatch);
            GetMatches(other.assembly_current, out var otherAppalachiaMatch, out var otherUnityMatch);

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
                assembly_current,
                other.assembly_current,
                StringComparison.Ordinal
            );
            if (assemblyCurrentComparison != 0)
            {
                return assemblyCurrentComparison;
            }

            return string.Compare(path, other.path, StringComparison.Ordinal);
        }

        public bool Equals(AssemblyDefinitionMetadata other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return assembly_current == other.assembly_current;
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

        public override int GetHashCode()
        {
            return assembly_current != null ? assembly_current.GetHashCode() : 0;
        }

        public override string ToString()
        {
            using (_PRF_ToString.Auto())
            {
                return assembly_current;
            }
        }

        private void LoadDotSettings()
        {
            if (!AppaFile.Exists(dotSettingsPath))
            {
                dotSettings = new DotSettings(null);
            }
            else
            {
                var dotSettingsText = AppaFile.ReadAllLines(dotSettingsPath);
                dotSettings = new DotSettings(dotSettingsText);
            }
        }

        public static AssemblyDefinitionMetadata CreateNew(string path, bool preparing = false)
        {
            if (_instances == null)
            {
                _instances = new Dictionary<string, AssemblyDefinitionMetadata>();
            }

            if (_instancesByGuid == null)
            {
                _instancesByGuid = new Dictionary<string, AssemblyDefinitionMetadata>();
            }

            if (_instances.ContainsKey(path))
            {
                return _instances[path];
            }

            var newInstance = new AssemblyDefinitionMetadata();

            newInstance.Initialize(path);

            if (_instances.ContainsKey(path))
            {
                return _instances[path];
            }

            _instances.Add(path, newInstance);
            _instancesByGuid.Add(newInstance.guid,             newInstance);
            _instancesByGuid.Add(newInstance.assembly_current, newInstance);

            if (!preparing)
            {
                newInstance.InitializeAnalysis();
            }

            return newInstance;
        }

        public static IEnumerable<AssemblyDefinitionMetadata> FindAll()
        {
            PrepareAll();

            return _instances.Values;
        }

        public static void PrepareAll()
        {
            if (_allPrepared)
            {
                return;
            }

            var asmdefs = AssetDatabaseManager.FindAssetPathsByExtension(".asmdef");

            foreach (var asmdef in asmdefs)
            {
                CreateNew(asmdef, true);
            }

            foreach (var instance in _instances.Values)
            {
                instance.InitializeAnalysis();
            }

            _allPrepared = true;
        }

        private static string GetFilePath(string path, bool testFile)
        {
            if (testFile)
            {
                path += ".test";
            }

            return path;
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
    }
}
