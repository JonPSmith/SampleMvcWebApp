#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

//see blog http://james.newtonking.com/archive/2008/10/16/asp-net-mvc-and-json-net.aspx

namespace SampleWebApp.Infrastructure
{
  public class JsonNetResult : ActionResult
  {
    public Encoding ContentEncoding { get; set; }
    public string ContentType { get; set; }
    public object Data { get; set; }

    public JsonSerializerSettings SerializerSettings { get; set; }
    public Formatting Formatting { get; set; }

    public JsonNetResult()
    {
      SerializerSettings = new JsonSerializerSettings();
    }

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
        throw new ArgumentNullException("context");

      HttpResponseBase response = context.HttpContext.Response;
      
      response.ContentType = !string.IsNullOrEmpty(ContentType)
        ? ContentType
        : "application/json";

      if (ContentEncoding != null)
        response.ContentEncoding = ContentEncoding;

      if (Data != null)
      {
        JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

        JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
        serializer.Serialize(writer, Data);

        writer.Flush();
      }
    }
  }
}