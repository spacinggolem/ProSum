using ProSum.Models;
using System.Collections.Generic;

namespace ProSum.Containers.Interfaces
{
    public interface IStepContainer
    {
        public IReadOnlyList<Step> Steps { get; }

        public void CreateStep(Step step);
        public void UpdateStepNumber(Step step);
        public void DeleteStep(Step step);
        public void UpdateStepName(Step step);
    }
}