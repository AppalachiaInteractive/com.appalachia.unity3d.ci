using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Colors;
using UnityEngine;

namespace Appalachia.CI.Integration.Analysis
{
    [Serializable]
    public abstract class AnalysisMetadata<T, TT> : IAnalysisColorable
        where T : IAnalysisTarget
        where TT : Enum
    {
        protected AnalysisMetadata(T target)
        {
            _target = target;

            _allIssues = new List<AnalysisResult<TT>>();

            RegisterAllAnalysis();

            SetIssueColors();

            Analyze();
        }

        [NonSerialized] private bool _dependenciesAnalyzed;
        [NonSerialized] private bool _dependenciesAnalyzing;

        private List<AnalysisResult<TT>> _allIssues;

        private T _target;

        public int IssueDisplayColumns { get; set; } = 3;

        public Color IssueColor { get; set; }
        public bool AnyIssues => AllIssues.Any(a => a.HasIssue);

        public IReadOnlyList<AnalysisResult<TT>> AllIssues => _allIssues;

        public T Target => _target;

        protected abstract void OnAnalyze();

        protected abstract void RegisterAllAnalysis();

        public AnalysisResult<TT> IssueByType(TT type)
        {
            foreach (var issue in _allIssues)
            {
                if (issue.type.Equals(type))
                {
                    return issue;
                }
            }

            throw new NotSupportedException(type.ToString());
        }

        public bool HasIssues(TT type)
        {
            var issue = IssueByType(type);

            return issue.HasIssue;
        }

        public void Analyze()
        {
            if (_dependenciesAnalyzed || _dependenciesAnalyzing)
            {
                return;
            }

            _dependenciesAnalyzing = true;

            if (Target == null)
            {
                return;
            }

            OnAnalyze();

            _dependenciesAnalyzed = true;
            _dependenciesAnalyzing = false;
        }

        public void CorrectAllIssues(bool useTestFiles, bool reimport)
        {
            for (var index = 0; index < AllIssues.Count; index++)
            {
                var issue = AllIssues[index];

                issue.Correct(useTestFiles, reimport);
            }
        }

        public void Reanalyze()
        {
            _dependenciesAnalyzed = false;
            _dependenciesAnalyzing = false;

            Target.ClearAnalysisResults();

            Analyze();
        }

        public void SetIssueColors()
        {
            var colors = ColorPalette.Default.bad.Multiple(AllIssues.Count);

            for (var index = 0; index < AllIssues.Count; index++)
            {
                var issue = AllIssues[index];
                issue.color = colors[index];
            }
        }

        protected AnalysisResult<TT> RegisterAnalysis(
            string name,
            TT type,
            Func<bool> issueChecker,
            Action<bool, bool> correction)
        {
            var result = new AnalysisResult<TT>(name, type, issueChecker, correction);

            _allIssues.Add(result);

            return result;
        }

        protected static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            IAnalysisColorable colorable3,
            IAnalysisColorable colorable4,
            AnalysisResult<TT> analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
            SetColor(colorable3, analysis, overwrite);
            SetColor(colorable4, analysis, overwrite);
        }

        protected static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            IAnalysisColorable colorable3,
            AnalysisResult<TT> analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
            SetColor(colorable3, analysis, overwrite);
        }

        protected static void SetColor(
            IAnalysisColorable colorable1,
            IAnalysisColorable colorable2,
            AnalysisResult<TT> analysis,
            bool overwrite = false)
        {
            SetColor(colorable1, analysis, overwrite);
            SetColor(colorable2, analysis, overwrite);
        }

        protected static void SetColor(
            IAnalysisColorable colorable,
            AnalysisResult<TT> analysis,
            bool overwrite = false)
        {
            if (overwrite || (colorable.IssueColor == default))
            {
                colorable.IssueColor = analysis.color;
            }
        }

        public Color GetColor(TT type)
        {
            foreach (var issue in AllIssues)
            {
                if (Equals(issue.type, type))
                {
                    return issue.color;
                }
            }

            throw new NotSupportedException(type.ToString());
        }
    }
}
