using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.CI.Integration.Analysis;
using Appalachia.CI.Integration.Assets;
using Appalachia.CI.Integration.Repositories;

namespace Appalachia.CI.Integration.Assemblies
{
    [Serializable]
    public class
        AssemblyDefinitionAnalysisMetadata : AnalysisMetadata<AssemblyDefinitionMetadata,
            AssemblyAnalysisType>
    {
        static AssemblyDefinitionAnalysisMetadata()
        {
            RepositoryDirectoryMetadata.PrepareAll();
            AssemblyDefinitionMetadata.PrepareAll();
        }

        public AssemblyDefinitionAnalysisMetadata(AssemblyDefinitionMetadata assembly) : base(assembly)
        {
        }

        public AnalysisResult<AssemblyAnalysisType> DependencyLevel;
        public AnalysisResult<AssemblyAnalysisType> DependencyOpportunity;
        public AnalysisResult<AssemblyAnalysisType> DependencyPresence;
        public AnalysisResult<AssemblyAnalysisType> DependencyValidity;
        public AnalysisResult<AssemblyAnalysisType> DependencyVersions;
        public AnalysisResult<AssemblyAnalysisType> NameAssembly;
        public AnalysisResult<AssemblyAnalysisType> NameFile;
        public AnalysisResult<AssemblyAnalysisType> Namespace;
        public AnalysisResult<AssemblyAnalysisType> NamespaceFoldersExclusions;
        public AnalysisResult<AssemblyAnalysisType> NamespaceFoldersEncoding;
        public AnalysisResult<AssemblyAnalysisType> ReferenceDuplicates;
        public AnalysisResult<AssemblyAnalysisType> ReferenceStyle;
        public AnalysisResult<AssemblyAnalysisType> ReferenceValidity;
        public AnalysisResult<AssemblyAnalysisType> Sorting;

        protected override void OnAnalyze()
        {
            var uniqueDependencies = new Dictionary<string, RepositoryDependencyMetadata>();
            var uniqueReferences = new HashSet<AssemblyDefinitionMetadata>();
            var uniqueReferenceStrings = new HashSet<string>();
            var uniqueOpportunities = new HashSet<AssemblyDefinitionMetadata>();

            InitializeAnalysis(this, Target);

            AnalyzeProject(
                this,
                Target,
                NameAssembly,
                NameFile,
                Namespace,
                NamespaceFoldersExclusions,
                NamespaceFoldersEncoding
            );

            AnalzyeRepository(Target.repository, uniqueDependencies);

            AnalyzeReferences(
                this,
                Target,
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
                Target,
                uniqueDependencies,
                uniqueReferences,
                uniqueReferenceStrings,
                uniqueOpportunities,
                DependencyValidity,
                DependencyPresence,
                DependencyVersions,
                DependencyOpportunity
            );
        }

        protected override void RegisterAllAnalysis()
        {
            NameAssembly = RegisterAnalysis(
                "Assembly Naming",
                AssemblyAnalysisType.NameAssembly,
                CheckNameAssembly,
                FixNameAssembly
            );

            NameFile = RegisterAnalysis(
                "File Naming",
                AssemblyAnalysisType.NameFile,
                CheckNameFile,
                FixNameFile
            );

            Namespace = RegisterAnalysis(
                "Namespace",
                AssemblyAnalysisType.Namespace,
                CheckNamespace,
                FixNamespace
            );
            
            NamespaceFoldersExclusions = RegisterAnalysis(
                "Namespace Folders Exclusions",
                AssemblyAnalysisType.NamespaceFoldersExclusions,
                CheckNamespaceFoldersExclusions,
                FixNamespaceFoldersExclusions
            );
            
            NamespaceFoldersEncoding = RegisterAnalysis(
                "Namespace Folders Formatting",
                AssemblyAnalysisType.NamespaceFoldersEncoding,
                CheckNamespaceFoldersEncoding,
                FixNamespaceFoldersEncoding
            );

            ReferenceValidity = RegisterAnalysis(
                "Reference Validity",
                AssemblyAnalysisType.ReferenceValidity,
                CheckReferenceValidity,
                FixReferenceValidity
            );

            ReferenceStyle = RegisterAnalysis(
                "Reference Style",
                AssemblyAnalysisType.ReferenceStyle,
                CheckReferenceStyle,
                FixReferenceStyle
            );

            DependencyPresence = RegisterAnalysis(
                "Dependency Presence",
                AssemblyAnalysisType.DependencyPresence,
                CheckDependencyPresence,
                FixDependencyPresence
            );

            DependencyValidity = RegisterAnalysis(
                "Dependency Validity",
                AssemblyAnalysisType.DependencyValidity,
                CheckDependencyValidity,
                FixDependencyValidity
            );

            DependencyLevel = RegisterAnalysis(
                "Dependency Level",
                AssemblyAnalysisType.DependencyLevel,
                CheckDependencyLevel,
                FixDependencyLevel
            );

            DependencyVersions = RegisterAnalysis(
                "Dependency Versions",
                AssemblyAnalysisType.DependencyVersions,
                CheckDependencyVersions,
                FixDependencyVersions
            );

            DependencyOpportunity = RegisterAnalysis(
                "Dependency Opportunity",
                AssemblyAnalysisType.DependencyOpportunity,
                CheckDependencyOpportunity,
                FixDependencyOpportunity
            );

            Sorting = RegisterAnalysis("Sorting", AssemblyAnalysisType.Sorting, CheckSorting, FixSorting);

            ReferenceDuplicates = RegisterAnalysis(
                "Reference Duplicates",
                AssemblyAnalysisType.ReferenceDuplicates,
                CheckReferenceDuplicates,
                FixReferenceDuplicates
            );
        }

        private bool CheckDependencyLevel()
        {
            return Target.references.Any(d => d.isLevelIssue);
        }

        private bool CheckDependencyOpportunity()
        {
            return Target.opportunities.Any();
        }

        private bool CheckDependencyPresence()
        {
            return Target.dependencies.Any(d => d.IsMissing);
        }

        private bool CheckDependencyValidity()
        {
            return Target.dependencies.Any(d => !d.IsValid);
        }

        private bool CheckDependencyVersions()
        {
            return Target.dependencies.Any(d => d.IsOutOfDate);
        }

        private bool CheckNameAssembly()
        {
            if (!Target.Name.StartsWith("Appalachia"))
            {
                return false;
            }

            return Target.assembly_current != Target.assembly_ideal;
        }

        private bool CheckNameFile()
        {
            return Target.filename_current != Target.filename_ideal;
        }

        private bool CheckNamespace()
        {
            if (!Target.Name.StartsWith("Appalachia"))
            {
                return false;
            }

            var ns1 = Target.root_namespace_current;
            var ns2 = Target.root_namespace_ideal;

            var bothNull = string.IsNullOrWhiteSpace(ns1) && string.IsNullOrWhiteSpace(ns2);

            return !bothNull && (ns1 != ns2);
        }

        private bool CheckNamespaceFoldersExclusions()
        {
            return Target.dotSettings?.AllFolders.Any(f => !f.excluded || f.encodingIssue) ?? false;
        }
        
        private bool CheckNamespaceFoldersEncoding()
        {
            return Target.dotSettings?.AllFolders.Any(f => !f.excluded || f.encodingIssue) ?? false;
        }
        
        private bool CheckReferenceDuplicates()
        {
            return Target.references?.Any(r => r.isDuplicate) ?? false;
        }

        private bool CheckReferenceStyle()
        {
            return Target.references?.Any(s => !s.IsGuidReference) ?? false;
        }

        private bool CheckReferenceValidity()
        {
            return Target.references?.Any(r => r.assembly == null) ?? false;
        }

        private bool CheckSorting()
        {
            return Target.references.Any(d => d.outOfSorts);
        }

        private void FixDependencyLevel(bool useTestFiles, bool reimport)
        {
            for (var index = Target.references.Count - 1; index >= 0; index--)
            {
                var reference = Target.references[index];

                if (reference.isLevelIssue)
                {
                    Target.references.RemoveAt(index);
                }
            }

            WriteReferences();

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixDependencyOpportunity(bool useTestFiles, bool reimport)
        {
            var referenceLookup = Target.references.Select(r => r.guid).ToHashSet();

            for (var index = 0; index < Target.opportunities.Count; index++)
            {
                var opportunity = Target.opportunities[index];
                if (!referenceLookup.Contains(opportunity.guid))
                {
                    Target.references.Add(opportunity);
                }
            }

            WriteReferences();

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixDependencyPresence(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = Target.repository.npmPackage;

            for (var index = Target.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = Target.dependencies[index];
                var refRepo = dependency.repository;

                if (refRepo != null)
                {
                    var packageName = refRepo.PackageName;
                    var packageVersion = refRepo.PackageVersion;

                    if (npmPackage.Dependencies == null)
                    {
                        npmPackage.Dependencies = new Dictionary<string, string>();
                    }

                    if (!npmPackage.Dependencies.ContainsKey(packageName))
                    {
                        changed = true;
                        npmPackage.Dependencies.Add(packageName, packageVersion);
                    }
                }
            }

            if (changed)
            {
                Target.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixDependencyValidity(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = Target.repository.npmPackage;

            for (var index = Target.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = Target.dependencies[index];

                if (!dependency.IsValid)
                {
                    changed = true;
                    Target.dependencies.RemoveAt(index);
                    npmPackage.Dependencies.Remove(dependency.name);
                }
            }

            if (changed)
            {
                Target.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixDependencyVersions(bool useTestFiles, bool reimport)
        {
            var changed = false;

            var npmPackage = Target.repository.npmPackage;

            for (var index = Target.dependencies.Count - 1; index >= 0; index--)
            {
                var dependency = Target.dependencies[index];
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
                Target.repository.SavePackageJson(useTestFiles, reimport);
            }
        }

        private void FixNameAssembly(bool useTestFiles, bool reimport)
        {
            Target.assetModel.name = Target.assembly_ideal;

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixNameFile(bool useTestFiles, bool reimport)
        {
            AssetDatabaseManager.RenameAsset(Target.path, Target.filename_ideal);
        }

        private void FixNamespace(bool useTestFiles, bool reimport)
        {
            Target.assetModel.rootNamespace = Target.root_namespace_ideal;

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixNamespaceFoldersExclusions(bool useTestFiles, bool reimport)
        {
            Target.dotSettings.AddMissingFolders();
            Target.SaveDotSettingsFile(useTestFiles);
        }


        private void FixNamespaceFoldersEncoding(bool useTestFiles, bool reimport)
        {
            Target.dotSettings.FixEncodingIssues();
            Target.SaveDotSettingsFile(useTestFiles);
        }

        private void FixReferenceDuplicates(bool useTestFiles, bool reimport)
        {
            WriteReferences();

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixReferenceStyle(bool useTestFiles, bool reimport)
        {
            var allAssemblies = AssemblyDefinitionMetadata.Instances.ToList();

            var changed = false;

            Target.referenceStrings.Clear();

            for (var i = 0; i < Target.references.Count; i++)
            {
                var reference = Target.references[i];

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
                Target.referenceStrings.Add(reference.assembly?.guid);
            }

            WriteReferences();

            if (changed)
            {
                Target.SaveFile(useTestFiles, reimport);
            }
        }

        private void FixReferenceValidity(bool useTestFiles, bool reimport)
        {
            for (var i = Target.references.Count - 1; i >= 0; i--)
            {
                var reference = Target.references[i];

                if (reference.assembly == null)
                {
                    Target.references.RemoveAt(i);
                }
            }

            WriteReferences();

            Target.SaveFile(useTestFiles, reimport);
        }

        private void FixSorting(bool useTestFiles, bool reimport)
        {
            Target.references.Sort();

            WriteReferences();

            Target.SaveFile(useTestFiles, reimport);
        }

        private void WriteReferences()
        {
            Target.referenceStrings.Clear();
            Target.references.Sort();
            Target.references = Target.references.Distinct().ToList();

            for (var i = 0; i < Target.references.Count; i++)
            {
                var reference = Target.references[i];

                Target.referenceStrings.Add(reference.guid);
            }

            Target.assetModel.references = Target.referenceStrings.ToArray();
        }

        private static void AnalyzeDependencies(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies,
            HashSet<AssemblyDefinitionMetadata> uniqueReferences,
            HashSet<string> uniqueReferenceStrings,
            HashSet<AssemblyDefinitionMetadata> uniqueOpportunities,
            AnalysisResult<AssemblyAnalysisType> dependencyValidity,
            AnalysisResult<AssemblyAnalysisType> dependencyPresence,
            AnalysisResult<AssemblyAnalysisType> dependencyVersions,
            AnalysisResult<AssemblyAnalysisType> dependencyOpportunity)
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
                if (!assembly.Name.StartsWith("Appalachia") || (thisLevel > 1000))
                {
                    break;
                }

                if (!instance.Name.StartsWith("Appalachia"))
                {
                    continue;
                }

                if (uniqueReferences.Contains(instance) || uniqueOpportunities.Contains(instance))
                {
                    continue;
                }

                var instanceLevel = instance.GetAssemblyDependencyLevel();
                var oportunityCutoffLevel = instance.GetOpportunityCutoffLevel();

                if ((instanceLevel < oportunityCutoffLevel) && (instanceLevel < thisLevel))
                {
                    uniqueOpportunities.Add(instance);
                    var oppReff = new AssemblyDefinitionReferenceMetadata(instance);
                    assembly.opportunities.Add(oppReff);

                    SetColor(analysis, assembly, oppReff, dependencyOpportunity);
                }
            }
        }

        private static void AnalyzeProject(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            AnalysisResult<AssemblyAnalysisType> nameAssembly,
            AnalysisResult<AssemblyAnalysisType> nameFile,
            AnalysisResult<AssemblyAnalysisType> ns,
            AnalysisResult<AssemblyAnalysisType> namespaceFoldersExclusions,
            AnalysisResult<AssemblyAnalysisType> namespaceFoldersFormatting)
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

            assembly.dotSettings.CheckNamespaceFolderIssues(assembly);

            if (namespaceFoldersExclusions.HasIssue)
            {
                SetColor(analysis, assembly, namespaceFoldersExclusions);
            }
            
            if (namespaceFoldersFormatting.HasIssue)
            {
                SetColor(analysis, assembly, namespaceFoldersFormatting);
            }
        }

        private static void AnalyzeReferences(
            AssemblyDefinitionAnalysisMetadata analysis,
            AssemblyDefinitionMetadata assembly,
            Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies,
            HashSet<AssemblyDefinitionMetadata> uniqueReferences,
            HashSet<string> uniqueReferenceStringss,

            //HashSet<AssemblyDefinitionMetadata> uniqueOpportunities,
            AnalysisResult<AssemblyAnalysisType> referenceDuplicates,
            AnalysisResult<AssemblyAnalysisType> referenceValidity,
            AnalysisResult<AssemblyAnalysisType> dependencyLevel,
            AnalysisResult<AssemblyAnalysisType> referenceStyle,
            AnalysisResult<AssemblyAnalysisType> sorting)
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

        private static void AnalzyeRepository(
            RepositoryDirectoryMetadata repository,
            Dictionary<string, RepositoryDependencyMetadata> uniqueDependencies)
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
    }
}
