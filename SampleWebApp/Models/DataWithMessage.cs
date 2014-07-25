using System.Collections;
using System.Collections.Generic;
using System.Web;
using GenericServices;

namespace SampleWebApp.Models
{

    public enum DisplayMessageTypes { NoMessage, Success, Error}

    public class ListWithMessage<T>
    {

        public IEnumerable<T> Data { get; private set; }

        public DisplayMessageTypes MessageType { get; private set; }

        public string Message { get; private set; }

        public ListWithMessage(IEnumerable<T> data, ISuccessOrErrors status)
        {
            Data = data;
            MessageType = status.IsValid ? DisplayMessageTypes.Success : DisplayMessageTypes.Error;
            Message = status.SuccessMessage;

        }

        public ListWithMessage(IEnumerable<T> data, DisplayMessageTypes messageType, string message)
        {
            Data = data;
            MessageType = messageType;
            Message = message;
        }

        public HtmlString FormHtmlMessage()
        {
            return MessageType == DisplayMessageTypes.NoMessage 
                ? new HtmlString(string.Empty) 
                : new HtmlString( string.Format("<div class=\"{0}\">{1}</div><br />",
                    MessageType == DisplayMessageTypes.Success ? "text-success" : "text-danger", Message));
        }
    }

    /// <summary>
    /// A static extension method to make using ListWithMessage much easier
    /// </summary>
    public static class ListWithMessageFactory
    {
        public static ListWithMessage<T> ShowData<T>(this IEnumerable<T> data)
        {
            return new ListWithMessage<T>(data, DisplayMessageTypes.NoMessage, null);
        }

        public static ListWithMessage<T> ShowDataAndMessage<T>(this IEnumerable<T> data, ISuccessOrErrors status)
        {
            return new ListWithMessage<T>(data, status);
        }
    }
}