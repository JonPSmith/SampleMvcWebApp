#region licence
// The MIT License (MIT)
// 
// Filename: JsonHelper.cs
// Date Created: 2014/05/31
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Helpers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Helpers
{
    static class JsonHelper
    {

        //public static string SerialiseToJsonUsingJsonNet(this object data)
        //{
        //    JsonConvert.SerializeObject(data);
        //    
        //}

        //public static string SerialiseToJsonIndentedUsingJsonNet(this object data)
        //{
        //    return JsonConvert.SerializeObject(data, Formatting.Indented);
        //}

        public static string SerialiseToJson(this object data)
        {
            return Json.Encode(data);
        }

        public static string AssertJsonPropertyPresentAndReturnValue<TSource, TProperty>
            (this string jsonString,
                TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            return jsonString.AssertJsonPropertyInOrOutAndReturnValue(source, propertyLambda, false);
        }

        public static void AssertJsonPropertyNotPresent<TSource, TProperty>
            (this string jsonString,
            TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            jsonString.AssertJsonPropertyInOrOutAndReturnValue(source, propertyLambda, true);
        }

        //-----------------------------------------------------------------

        private static string AssertJsonPropertyInOrOutAndReturnValue<TSource, TProperty>
            (this string jsonString, 
             TSource source, Expression<Func<TSource, TProperty>> propertyLambda,
             bool doesNotContain)
        {

            var stringToFind = string.Format("\"{0}\":", source.GetPropertyInfo(propertyLambda).Name);
            var startIndex = jsonString.IndexOf(stringToFind, StringComparison.InvariantCultureIgnoreCase);
            if (doesNotContain)
            {
                if (startIndex == -1) return null;      //all good
                Assert.Fail("The property '{0}' should NOT be in the in json string", stringToFind);
            }

            //otherwise we expect it to be in
            if (startIndex == -1)
                Assert.Fail("Looked for '{0}' in json and could not find it", stringToFind);

            //now return value after it

            var closingIndex = jsonString.IndexOf('\n', startIndex + 1);
            if (closingIndex == -1)
                throw new ValidationException("This only works on indented json, and this doesn't seem to be indented");

            var result = jsonString.Substring(startIndex + stringToFind.Length, closingIndex - startIndex - stringToFind.Length).Trim();
            return result.EndsWith(",") ? result.Substring(0, result.Length - 1).Trim() : result;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

    }
}
