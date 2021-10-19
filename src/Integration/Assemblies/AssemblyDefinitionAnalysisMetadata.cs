using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Analysis;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.FileSystem;
using Appalachia.CI.Integration.Repositories;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    public class AssemblyDefinitionAnalysisMetadata : IAnalysisColorable
    {
        static AssemblyDefinitionAnalysisMetadata()
        {
            RepositoryDirectoryMetadata.PrepareAll();
            AssemblyDefinitionMetadata.PrepareAll();
        }

        public AssemblyDefinitionAnalysisMetadata(AssemblyDefinitionMetadata assembly)
        {
            _assembly = assembly;

            AllIssues = new List<AnalysisResult>();

            NameAssembly = new AnalysisResult(
                "Assembly Naming",
                AnalysisType.NameAssembly,
                CheckNameAssembly,
                FixNameAssembly
            );

            NameFile = new AnalysisResult("File Naming", AnalysisType.NameFile, CheckNameFile, FixNameFile);

            Namespace = new AnalysisResult("Namespace", AnalysisType.Namespace, CheckNamespace, FixNamespace);

            NamespaceFolders = new AnalysisResult(
                "Namespace Folders",
                AnalysisType.NamespaceFolders,
                CheckNamespaceFolders,
                FixNamespaceFolders
            );

            ReferenceValidity = new AnalysisResult(
                "Reference Validity",
                AnalysisType.ReferenceValidity,
                CheckReferenceValidity,
                FixReferenceValidity
            );

            ReferenceStyle = new AnalysisResult(
                "Reference Style",
                AnalysisType.ReferenceStyle,
                CheckReferenceStyle,
                FixReferenceStyle
            );

            DependencyPresence = new AnalysisResult(
                "Dependency Presence",
                AnalysisType.DependencyPresence,
                CheckDependencyPresence,
                FixDependencyPresence
            );

            DependencyValidity = new AnalysisResult(
                "Dependency Validity",
                AnalysisType.DependencyPresence,
                CheckDependencyValidity,
                FixDependencyValidity
            );

            DependencyLevel = new AnalysisResult(
                "Dependency Level",
                AnalysisType.DependencyLevel,
                CheckDependencyLevel,
                FixDependencyLevel
            );

            DependencyVersions = new AnalysisResult(
                "Dependency Versions",
                AnalysisType.DependencyVersions,
                CheckDependencyVersions,
                FixDependencyVersions
            );

            DependencyOpportunity = new AnalysisResult(
                "Dependency Opportunity",
                AnalysisType.DependencyOpportunity,
                CheckDependencyOpportunity,
                FixDependencyOpportunity
            );

            Sorting = new AnalysisResult("Sorting", AnalysisType.Sorting, CheckSorting, FixSorting);

            ReferenceDuplicates = new AnalysisResult(
                "Reference Duplicates",
                AnalysisType.ReferenceDuplicates,
                CheckReferenceDuplicates,
                FixReferenceDuplicates
            );

            AllIssues.Add(NameAssembly);
            AllIssues.Add(NameFile);
            AllIssues.Add(Namespace);
            AllIssues.Add(NamespaceFolders);
            AllIssues.Add(ReferenceValidity);
            AllIssues.Add(ReferenceDuplicates);
            AllIssues.Add(ReferenceStyle);
            AllIssues.Add(DependencyValidity);
            AllIssues.Add(DependencyPresence);
            AllIssues.Add(DependencyLevel);
            AllIssues.Add(DependencyVersions);
            AllIssues.Add(DependencyOpportunity);
            AllIssues.Add(Sorting);

            var colors = ColorPalette.Default.GetBadMultiple(AllIssues.Count);

            for (var index = 0; index < AllIssues.Count; index++)
            {
                var issue = AllIssues[index];
                issue.color = colors[index];
            }
            
            Analyze();
        }

        public readonly AnalysisResult DependencyLevel;
        public readonly AnalysisResult DependencyOpportunity;
        public readonly AnalysisResult DependencyPresence;
        public readonly AnalysisResult DependencyValidity;
        public readonly AnalysisResult DependencyVersions;
        public readonly AnalysisResult NameAssembly;
        public readonly AnalysisResult NameFile;
        public readonly AnalysisResult Namespace;
        public readonly AnalysisResult NamespaceFolders;
        public readonly AnalysisResult ReferenceDuplicates;
        public readonly AnalysisResult ReferenceStyle;
        public readonly AnalysisResult ReferenceValidity;
        public readonly AnalysisResult Sorting;

        public List<AnalysisResult> AllIssues;

        private AssemblyDefinitionMetadata _assembly;

        [NonSerialized] private bool _dependenciesAnalyzed;
        [NonSerialized] private bool _dependenciesAnalyzing;

        public Color IssueColor { get; set; }

        public bool AnyIssues => AllIssues.Any(a => a.HasIssue);
        public int IssueDisplayColumns => 4;

        public AnalysisResult IssueByType(AnalysisType type)
        {
            switch (type)
            {
                case AnalysisType.DependencyLevel:
                    return DependencyLevel;
                case AnalysisType.DependencyOpportunity:
                    return DependencyOpportunity;
                case AnalysisType.DependencyPresence:
                    return DependencyPresence;
                case AnalysisType.DependencyVersions:
                    return DependencyVersions;
                case AnalysisType.NameAssembly:
                    return NameAssembly;
                case AnalysisType.NameFile:
                    return NameFile;
                case AnalysisType.Namespace:
                    return Namespace;
                case AnalysisType.ReferenceStyle:
                    return ReferenceStyle;
                case AnalysisType.ReferenceValidity:
                    return ReferenceValidity;
                case AnalysisType.Sorting:
                    return Sorting;
                case AnalysisType.ReferenceDuplicates:
                    return ReferenceDuplicates;
                case AnalysisType.DependencyValidity:
                    return DependencyValidity;
                case AnalysisType.NamespaceFolders:
                    return NamespaceFolders;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private bool CheckDependencyLevel()
        {
            return _assembly.references.Any(d => d.isLevelIssue);
        }

        private bool CheckDependencyOpportunity()
        {
            return _assembly.opportunities.Any();
        }

        private bool CheckDependencyPresence()
        {
            return _assembly.dependencies.Any(d => d.IsMissing);
        }

        private bool CheckDependencyValidity()
        {
            return _assembly.dependencies.Any(d => d.IsValid);
        }

        private bool CheckDependencyVersions()
        {
            return _assembly.dependencies.Any(d => d.IsOutOfDate);
        }

        private bool CheckNameAssembly()
        {
            return _assembly.assembly_current != _assembly.assembly_ideal;
        }

        private bool CheckNameFile()
        {
            return _assembly.filename_current != _assembly.filename_ideal;
        }

        private bool CheckNamespace()
        {
            var ns1 = _assembly.root_namespace_current;
            var ns2 = _assembly.root_namespace_ideal;

            var bothNull = string.IsNullOrWhiteSpace(ns1) && string.IsNullOrWhiteSpace(ns2);

            return !bothNull && (ns1 != ns2);
        }

        private bool CheckNamespaceFolders()
        {
            return _assembly.dotSettings?.missingNamspaceFolderExclusions ?? false;
        }

        private bool CheckReferenceDuplicates()
        {
            return _assembly.references?.Any(r => r.isDuplicate) ?? false;
        }

        private bool CheckReferenceStyle()
        {
            return _assembly.references?.Any(s => !s.IsGuidReference) ?? false;
        }

        private bool CheckReferenceValidity()
        {
            return _assembly.references?.Any(r => r.assembly == null) ?? false;
        }

        private bool CheckSorting()
        {
            return _assembly.references.Any(d => d.outOfSorts);
        }

        private void Analyze()
        {
            if (_dependenciesAnalyzed || _dependenciesAnalyzing)
            {
                return;
            }

            _dependenciesAnalyzing = true;

            if (_assembly == null)
            {
                return;
            }

            var uniqueDependencies = new Dictionary<string, RepositoryDependencyMetadata>();
            var uniqueReferences = new HashSet<AssemblyDefinitionMetadata>();
            var uniqueReferenceStrings = new HashSet<string>();
            var uniqueOpportunities = new HashSet<AssemblyDefinitionMetadata>();
            
            InitializeAnalysis(this, _assembly);
            
            AnalyzeProject(this, _assembly, NameAssembly, NameFile, Namespace, NamespaceFolders);
            
            AnalzyeRepository(_assembly.repository, uniqueDependencies);
            
            AnalyzeReferences(
                this,
                _assembly,
                uniqueDependencies,
                uniqueReferences,
                uniqueReferenceStrings,
                ReferenceDuplicates,
                ReferenceValidity,
                DependencyLevel,
                ReferenceStyle,
                Sorting
            );

            AnalyzeDependencies(
                this,
                _assembly,
                uniqueDependencies,
                uniqueReferences,
                uniqueReferenceStrings,
                uniqueOpportunities,
                DependencyValidity,
                DependencyPresence,
                DependencyVersions,
                DependencyOpportunity
            );

            _dependenciesAnalyzed = true;
            _dependenciesAnalyzing = false;
        }

        private static void InitializeAnalysis(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly)
        {
            if (assembly.dependencies == null)
            {
                assembly.dependencies = new List<RepositoryDependencyMetadata>();
            }
            else
            {
                assembly.dependencies.Clear();
            }

            if (assembly.opportunities == null)
            {
                assembly.opportunities = new List<AssemblyDefinitionReferenceMetadata>();
            }
            else
            {
                assembly.opportunities.Clear();
            }
        }

        private static void AnalyzeProject(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            AnalysisResult nameAssembly,
            AnalysisResult nameFile,
            AnalysisResult ns,
            AnalysisResult namespaceFolders)
        {
            if (nameAssembly.HasIssue)
            {
                SetColor(analysis, assembly, nameAssembly);
            }

            if (nameFile.HasIssue)
            {
                SetColor(analysis, assembly, nameFile);
            }

            if (ns.HasIssue)
            {
                SetColor(analysis, assembly, ns);
            }

            if (namespaceFolders.HasIssue)
            {
                SetColor(analysis, assembly, namespaceFolders);
            }

            var exclusions = new HashSet<string>
            {
                "assets",
                "thirdparty",
                "third-party",
                "assemblies",
                "runtime",
                "editor",
                "test",
                "tests",
                "src",
                "dist",
                "asset",
                "data"
            };

            var dir = assembly.directory;
            var first = true;

            do
            {
                if (dir.Parent == null)
                {
                    throw new NotSupportedException(assembly.ToString());
                }

                if (!first)
                {
                    dir = dir.Parent;
                }

                first = false;

                if (exclusions.Contains(dir.Name.ToLowerInvariant()))
                {
                    assembly.dotSettings.CheckIfExcludingFolder(dir.RelativePath);
                }
            } while ((dir != null) &&
                     (dir.Name != "Assets") &&
                     (dir.Name != "Packages") &&
                     (dir.Name != "Library"));
        }

        private static void AnalzyeRepository(RepositoryDirectoryMetadata repository, Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies)
        {
            var npmPackage = repository.npmPackage;

            if (npmPackage == null)
            {
                return;
            }

            if (npmPackage.Dependencies == null)
            {
                return;
            }

            foreach (var dependency in npmPackage.Dependencies)
            {
                var newDep = new RepositoryDependencyMetadata(dependency.Key, dependency.Value);
                uniqueDependencies.Add(dependency.Key, newDep);

                var repoMatch = RepositoryDirectoryMetadata.Find(dependency.Key);

                if (repoMatch != null)
                {
                    newDep.repository = repoMatch;
                }
            }
        }

        private static void AnalyzeDependencies(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies,
            HashSet<AssemblyDefinitionMetadata> uniqueReferences,
            HashSet<string> uniqueReferenceStrings,
            HashSet<AssemblyDefinitionMetadata> uniqueOpportunities,
            AnalysisResult dependencyValidity,
            AnalysisResult dependencyPresence,
            AnalysisResult dependencyVersions,
            AnalysisResult dependencyOpportunity)
        {
            assembly.dependencies.AddRange(uniqueDependencies.Values);

            for (var index = 0; index < assembly.dependencies.Count; index++)
            {
                var dependency = assembly.dependencies[index];

                if (!dependency.IsValid)
                {
                    SetColor(analysis, assembly, dependency, dependencyValidity);
                }

                if (dependency.IsMissing)
                {
                    SetColor(analysis, assembly, dependency, dependencyPresence);
                }

                if (dependency.IsOutOfDate)
                {
                    SetColor(analysis, assembly, dependency, dependencyVersions);
                }
            }

            var thisLevel = assembly.GetAssemblyDependencyLevel();

            foreach (var instance in AssemblyDefinitionMetadata.Instances)
            {
                if (uniqueReferences.Contains(instance) || uniqueOpportunities.Contains(instance))
                {
                    continue;
                }

                var instanceLevel = instance.GetAssemblyDependencyLevel();

                if ((instanceLevel < 50) && (instanceLevel < thisLevel))
                {
                    uniqueOpportunities.Add(instance);
                    var oppReff = new AssemblyDefinitionReferenceMetadata(instance);
                    assembly.opportunities.Add(oppReff);

                    SetColor(analysis, assembly, oppReff, dependencyOpportunity);
                }
            }
        }

        private static void AnalyzeReferences(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies,
            HashSet<AssemblyDefinitionMetadata> uniqueReferences,
            HashSet<string> uniqueReferenceStringss,
            //HashSet<AssemblyDefinitionMetadata> uniqueOpportunities,
            AnalysisResult referenceDuplicates,
            AnalysisResult referenceValidity,
            AnalysisResult dependencyLevel,
            AnalysisResult referenceStyle,
            AnalysisResult sorting)
        {
            assembly.SetReferences();
            
            for (var index = 0; index < assembly.references.Count; index++)
            {
                var reference = assembly.references[index];
                if (uniqueReferenceStringss.Contains(reference.guid))
                {
                    SetColor(analysis, assembly, reference, referenceDuplicates);
                    reference.isDuplicate = true;
                }
                else
                {
                    uniqueReferenceStringss.Add(reference.guid);
                }

                if (reference.assembly == null)
                {
                    SetColor(analysis, assembly, reference, referenceValidity);
                    continue;
                }

                uniqueReferences.Add(reference.assembly);

                var refRepo = reference.assembly.repository;

                if (refRepo == assembly.repository)
                {
                    continue;
                }

                var packageName = refRepo.PackageName;

                if (uniqueDependencies.ContainsKey(packageName))
                {
                    uniqueDependencies[packageName].repository = refRepo;
                }
                else
                {
                    var newDep = new RepositoryDependencyMetadata(refRepo);
                    uniqueDependencies.Add(packageName, newDep);
                }
            }

            var thisLevel = assembly.GetAssemblyDependencyLevel();

            for (var index = 0; index < assembly.references.Count; index++)
            {
                var reference = assembly.references[index];
                if (reference.assembly == null)
                {
                    continue;
                }

                var refLevel = reference.assembly.GetAssemblyDependencyLevel();

                if (refLevel > thisLevel)
                {
                    reference.isLevelIssue = true;
                    SetColor(analysis, assembly, reference, dependencyLevel);
                }

                if (!reference.IsGuidReference)
                {
                    SetColor(analysis, assembly, reference, referenceStyle);
                }
            }

            for (var i = 0; i < (assembly.references.Count - 1); i++)
            {
                var ref1 = assembly.references[i];
                var ref2 = assembly.references[i + 1];

                if (ref1 > ref2)
                {
                    ref1.outOfSorts = true;
                    ref2.outOfSorts = true;
                    SetColor(analysis, assembly, ref1, ref2, sorting);
                }
            }
        }

        private void FixDependencyLevel(bool useTestFiles, bool reimport)
        {
            for (var index = _assembly.references.Count - 1; index >= 0; index--)
            {
                var reference = _assembly.references[index];

                if (reference.isLevelIssue)
                {
                    _assembly.references.RemoveAt(index);
                }
            }

            WriteReferences();

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixDependencyOpportunity(bool useTestFiles, bool reimport)
        {
            var referenceLookup = _assembly.references.Select(r => r.guid).ToHashSet();

            for (var index = 0; index < _assembly.opportunities.Count; index++)
            {
                var opportunity = _assembly.opportunities[index];
                if (!referenceLookup.Contains(opportunity.guid))
                {
                    _assembly.references.Add(opportunity);
                }
            }

            WriteReferences();

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixDependencyPresence(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = _assembly.repository.npmPackage;

            for (var index = _assembly.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = _assembly.dependencies[index];
                var refRepo = dependency.repository;

                if (refRepo != null)
                {
                    var packageName = refRepo.PackageName;
                    var packageVersion = refRepo.PackageVersion;

                    if (!npmPackage.Dependencies.ContainsKey(packageName))
                    {
                        changed = true;
                        npmPackage.Dependencies.Add(packageName, packageVersion);
                    }
                }
            }

            if (changed)
            {
                _assembly.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixDependencyValidity(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = _assembly.repository.npmPackage;

            for (var index = _assembly.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = _assembly.dependencies[index];

                if (!dependency.IsValid)
                {
                    changed = true;
                    _assembly.dependencies.RemoveAt(index);
                    npmPackage.Dependencies.Remove(dependency.name);
                }
            }

            if (changed)
            {
                _assembly.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixDependencyVersions(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = _assembly.repository.npmPackage;

            for (var index = _assembly.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = _assembly.dependencies[index];
                var refRepo = dependency.repository;

                if (refRepo != null)
                {
                    var packageName = refRepo.PackageName;
                    var packageVersion = refRepo.PackageVersion;

                    if (npmPackage.Dependencies.ContainsKey(packageName))
                    {
                        var currentVersion = npmPackage.Dependencies[packageName];

                        if (currentVersion != packageVersion)
                        {
                            changed = true;
                            npmPackage.Dependencies[packageName] = packageVersion;
                        }
                    }
                }
            }

            if (changed)
            {
                _assembly.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixNameAssembly(bool useTestFiles, bool reimport)
        {
            _assembly.assetModel.name = _assembly.assembly_ideal;

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixNameFile(bool useTestFiles, bool reimport)
        {
            AssetDatabaseManager.RenameAsset(_assembly.path, _assembly.filename_ideal);
        }

        private void FixNamespace(bool useTestFiles, bool reimport)
        {
            _assembly.assetModel.rootNamespace = _assembly.root_namespace_ideal;

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixNamespaceFolders(bool useTestFiles, bool reimport)
        {
            _assembly.dotSettings.AddMissingFolders();
            _assembly.SaveDotSettingsFile(useTestFiles);
        }

        private void FixReferenceDuplicates(bool useTestFiles, bool reimport)
        {
            WriteReferences();

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixReferenceStyle(bool useTestFiles, bool reimport)
        {
            var allAssemblies = AssemblyDefinitionMetadata.Instances.ToList();

            var changed = false;

            _assembly.referenceStrings.Clear();

            for (var i = 0; i < _assembly.references.Count; i++)
            {
                var reference = _assembly.references[i];

                if (reference.IsGuidReference)
                {
                    continue;
                }

                changed = true;

                if (reference.assembly == null)
                {
                    for (var index = 0; index < allAssemblies.Count; index++)
                    {
                        var assemblyToCheck = allAssemblies[index];

                        if (assemblyToCheck.assembly_current == reference.guid)
                        {
                            reference.assembly = assemblyToCheck;
                            break;
                        }
                    }
                }

                reference.guid = reference.assembly?.guid;
                _assembly.referenceStrings.Add(reference.assembly?.guid);
            }

            WriteReferences();

            if (changed)
            {
                _assembly.SaveFile(useTestFiles, reimport);
            }
        }

        private void FixReferenceValidity(bool useTestFiles, bool reimport)
        {
            for (var i = _assembly.references.Count - 1; i >= 0; i--)
            {
                var reference = _assembly.references[i];

                if (reference.assembly == null)
                {
                    _assembly.references.RemoveAt(i);
                }
            }

            WriteReferences();

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private void FixSorting(bool useTestFiles, bool reimport)
        {
            _assembly.references.Sort();

            WriteReferences();

            _assembly.SaveFile(useTestFiles, reimport);
        }

        private static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            IAnalysisColorable colorable3,
            IAnalysisColorable colorable4,
            AnalysisResult analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
            SetColor(colorable3, analysis, overwrite);
            SetColor(colorable4, analysis, overwrite);
        }

        private static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            IAnalysisColorable colorable3,
            AnalysisResult analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
            SetColor(colorable3, analysis, overwrite);
        }

        private static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            AnalysisResult analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
        }

        private static void SetColor(IAnalysisColorable colorable, AnalysisResult analysis, bool overwrite = false)
        {
            if (overwrite || (colorable.IssueColor == default))
            {
                colorable.IssueColor = analysis.color;
            }
        }

        private void WriteReferences()
        {
            _assembly.referenceStrings.Clear();
            _assembly.references.Sort();
            _assembly.references = _assembly.references.Distinct().ToList();

            for (var i = 0; i < _assembly.references.Count; i++)
            {
                var reference = _assembly.references[i];

                _assembly.referenceStrings.Add(reference.guid);
            }

            _assembly.assetModel.references = _assembly.referenceStrings.ToArray();
        }
    }
}
