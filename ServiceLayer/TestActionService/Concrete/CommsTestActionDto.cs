using System.ComponentModel.DataAnnotations;
using GenericServices.Core;

namespace ServiceLayer.TestActionService.Concrete
{

    /// <summary>
    /// This is a copy of CommsTestActionDto just so we can try the TActionData to TDto versions
    /// </summary>
    public class CommsTestActionDto : InstrumentedEfGenericDto<CommsTestActionData, CommsTestActionDto>, ICommsTestActionDto
    {
        private double _secondsBetweenIterations = 1;
        private int _numIterations = 5;

        [UIHint("Enum")]
        public TestServiceModes Mode { get; set; }

        public bool FailToRespondToCancel { get; set; }

        public int NumErrorsToExitWith { get; set; }

        [Range(0, 100)]
        public double SecondsBetweenIterations
        {
            get { return _secondsBetweenIterations; }
            set { _secondsBetweenIterations = value; }
        }

        public int NumIterations
        {
            get { return _numIterations; }
            set { _numIterations = value; }
        }

        public double SecondsDelayToRespondingToCancel { get; set; }

        protected override ServiceFunctions SupportedFunctions
        {
            get { return ServiceFunctions.DoAction | ServiceFunctions.DoDbAction; }
        }
    }
}