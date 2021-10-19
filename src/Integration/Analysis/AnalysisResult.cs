using System;
using UnityEngine;

namespace Appalachia.CI.Integration.Analysis
{
    [Serializable]
    public class AnalysisResult
    {
        public Color color;
        public string name;
        public AnalysisType type;
        private Action<bool, bool> _correction;

        private bool? _hasIssue;

        private Func<bool> _issueChecker;

        public AnalysisResult(
            string name,
            AnalysisType type,
            Func<bool> issueChecker,
            Action<bool, bool> correction)
        {
            this.name = name;
            this.type = type;
            _issueChecker = issueChecker;
            _correction = correction;
        }
        
        public AnalysisResult(
            string name,
            AnalysisType type,
            Color color,
            Func<bool> issueChecker,
            Action<bool, bool> correction)
        {
            this.name = name;
            this.type = type;
            this.color = color;
            _issueChecker = issueChecker;
            _correction = correction;
        }

        public bool HasIssue
        {
            get
            {
                if (!_hasIssue.HasValue)
                {
                    _hasIssue = _issueChecker();
                }

                return _hasIssue ?? false;
            }
        }

        public void Correct(bool useTestFiles, bool reimport)
        {
            if (!HasIssue)
            {
                return;
            }

            _correction(useTestFiles, reimport);
        }
    }
}
