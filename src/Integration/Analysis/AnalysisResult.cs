using System;
using UnityEngine;

namespace Appalachia.CI.Integration.Analysis
{
    [Serializable]
    public class AnalysisResult
    {
        protected AnalysisResult(string name, Func<bool> issueChecker, Action<bool, bool> correction)
        {
            this.name = name;
            _issueChecker = issueChecker;
            _correction = correction;
        }

        protected AnalysisResult(
            string name,
            Color color,
            Func<bool> issueChecker,
            Action<bool, bool> correction)
        {
            this.name = name;
            this.color = color;
            _issueChecker = issueChecker;
            _correction = correction;
        }

        public Color color;
        public string name;
        protected Action<bool, bool> _correction;
        protected bool? _hasIssue;
        protected Func<bool> _issueChecker;

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

    [Serializable]
    public class AnalysisResult<T> : AnalysisResult
    {
        public AnalysisResult(string name, T type, Func<bool> issueChecker, Action<bool, bool> correction) :
            base(name, issueChecker, correction)
        {
            this.type = type;
        }

        public AnalysisResult(
            string name,
            T type,
            Color color,
            Func<bool> issueChecker,
            Action<bool, bool> correction) : base(name, color, issueChecker, correction)
        {
            this.type = type;
        }

        public T type;
    }
}
