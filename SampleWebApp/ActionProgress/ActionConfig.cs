using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SampleWebApp.ActionProgress
{

    public enum ActionEndsModes { CurrentState, GotoSuccessUrl, Dissappears}


    /// <summary>
    /// This contains the configuration for how the action should be handled
    /// </summary>
    public class ActionConfig
    {
        private ActionEndsModes _actionEndsAt = ActionEndsModes.CurrentState;

        //------------------------------------------------
        //display controls

        /// <summary>
        /// This holds the header text to show in the header of any window shown.
        /// If null then default text shown
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HeaderText { get; set; }

        /// <summary>
        /// This holds a url that the browser side code should jump to on exit from a successful run of the action.
        /// An null value means stay on the current page
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SuccessExitUrl { get; set; }

        //-----------------------------------------------------
        //action configuration

        /// <summary>
        /// This says that the action is of indeterminate length, i.e. ignore any progress percents
        /// </summary>
        public bool IndeterminateLength { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ActionEndsModes ActionEndsAt
        {
            get { return _actionEndsAt; }
            set { _actionEndsAt = value; }
        }

        //---------------------------------------------------------------
        //ctors

        public ActionConfig() : this(null) {}

        public ActionConfig(string successExitUrl)
        {
            SuccessExitUrl = successExitUrl;
        }

    }
}