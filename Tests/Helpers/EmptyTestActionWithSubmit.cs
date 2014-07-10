using DataLayer.DataClasses.Concrete;
using GenericServices.ActionComms;

namespace Tests.Helpers
{
    public interface IEmptyTestActionWithSubmit : IActionCommsSync<int, Tag>
    {
    }

    public class EmptyTestActionWithSubmit : EmptyTestAction, IEmptyTestActionWithSubmit
    {

        /// <summary>
        /// If true then the caller should call EF SubmitChanges if the method exited with status IsValid and
        /// it looks to see if the data part has a ICheckIfWarnings and if the WriteEvenIfWarning is false
        /// and there are warnings then it does not call SubmitChanges
        /// </summary>
        public override bool SubmitChangesOnSuccess { get { return true; } }

    }
}