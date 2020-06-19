using System;

namespace ProSum.Models
{
    public class Step
    {
        public enum Status
        {
            not_started,
            in_progress,
            awaiting_confirmation,
            confirmed
        }
        public int StepNumber { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Step(string name, int stepNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            StepNumber = stepNumber;
        }

        public Step()
        {

        }
    }
}
