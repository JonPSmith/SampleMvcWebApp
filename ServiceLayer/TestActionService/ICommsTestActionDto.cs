using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GenericServices.Services;
using ServiceLayer.TestActionService.Concrete;

namespace ServiceLayer.TestActionService
{
    public interface ICommsTestActionDto
    {
        [UIHint("Enum")]
        TestServiceModes Mode { get; set; }

        bool FailToRespondToCancel { get; set; }
        int NumErrorsToExitWith { get; set; }

        [Range(0, 100)]
        double SecondsBetweenIterations { get; set; }

        int NumIterations { get; set; }
        double SecondsDelayToRespondingToCancel { get; set; }

        /// <summary>
        /// Instrumentation property which returns a list of call points as a comma delimited string.
        /// For start/end call points it only returns one entry.
        /// </summary>
        string FunctionsCalledCommaDelimited { get; }

        /// <summary>
        /// Instrumentation property which returns a list of instrumented call points with the time since the dto was created
        /// </summary>
        ReadOnlyCollection<InstrumentedLog> LogOfCalls { get; }

        /// <summary>
        /// This allows the user to control whether data should still be written even if warnings found
        /// </summary>
        bool WriteEvenIfWarning { get; }

        /// <summary>
        /// Instrumentation method which allows a specific point to be logged with a given name
        /// </summary>
        /// <param name="callPoint"></param>
        /// <param name="callType">defaults to Point</param>
        void LogSpecificName(string callPoint, CallTypes callType = CallTypes.Point);

        /// <summary>
        /// Thsi will log the name of the calling method
        /// </summary>
        /// <param name="callType">defaults to Point</param>
        /// <param name="callerName">Do not use. Filled in by system with the calling method name</param>
        void LogCaller(CallTypes callType = CallTypes.Point, [CallerMemberName] string callerName = "");

        /// <summary>
        /// Optional method that will setup any mapping etc. that are cached. This will will improve speed later.
        /// The GenericDto will still work without this method being called, but the first use that needs the map will be slower. 
        /// </summary>
        void CacheSetup();
    }
}
