using System.Collections.Generic;

namespace Appalachia.CI.Integration.Analysis
{
    public class AnalysisAggregate
    {
        private Dictionary<AnalysisType, int> _counts;

        public AnalysisAggregate()
        {
            _counts = new Dictionary<AnalysisType, int>();
        }

        public int this[AnalysisType type]
        {
            get
            {
                if (!_counts.ContainsKey(type))
                {
                    _counts.Add(type, 0);
                }

                return _counts[type];
            }
        }

        public bool HasIssues(AnalysisType type)
        {
            return this[type] > 0;
        }

        public void Add(IEnumerable<AnalysisResult> results)
        {
            foreach (var issue in results)
            {
                if (!_counts.ContainsKey(issue.type))
                {
                    _counts.Add(issue.type, 0);
                }

                if (!issue.HasIssue)
                {
                    continue;
                }

                _counts[issue.type] += 1;
            }
        }

        public void Reset()
        {
            _counts?.Clear();
        }
    }
}
