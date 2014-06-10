using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests.Helpers
{
    static class JsonHelper
    {

        public static string SerialiseToJsonIndentedUsingJsonNet(this object data)
        {
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static string SerialiseToJsonUsingJsonNet(this object data)
        {
            return JsonConvert.SerializeObject(data);
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
