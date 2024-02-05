using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mituzsuki.ConsoleUtilities
{
    internal class ObjectTreeGenerator
    {
        private readonly static Dictionary<Type, List<PropertyInfo>> _referenceTypePropsCache = new Dictionary<Type, List<PropertyInfo>>();

        List<PropertyInfo> GetReferenceTypeProperties(object obj)
        {
            Type objType = obj.GetType();

            if (_referenceTypePropsCache.TryGetValue(objType, out var props)) {
                return props;
            }
            else {
                List<PropertyInfo> propInfo = objType.GetProperties().Where(p => !p.PropertyType.IsValueType && !p.PropertyType.Equals(typeof(string)))
                                                                    .ToList();
                _referenceTypePropsCache[objType] = propInfo;
                return propInfo;
            }
        }


        private readonly static Dictionary<Type, List<PropertyInfo>> _valueTypePropsCache = new Dictionary<Type, List<PropertyInfo>>();
        public List<PropertyInfo> test;

        List<PropertyInfo> GetValueTypeProperties(object obj)
        {
            Type objType = obj.GetType();

            if (_valueTypePropsCache.TryGetValue(objType, out var props)) {
                return props;
            }
            else {
                List<PropertyInfo> propInfo = objType.GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType.Equals(typeof(String))).ToList();
                _valueTypePropsCache[objType] = propInfo;
                return propInfo;
            }
        }

        private readonly static List<object> _objectsAlreadyRead = new List<object>();

        public string GetPropertyTreeAsString(object? obj, int indent = 0)
        {

            //Helps prevent infinite loops
            if (obj is not null && _objectsAlreadyRead.Contains(obj)) {
                return string.Empty;
            }
            else if (obj is not null) {
                _objectsAlreadyRead.Add(obj!);
            }

            StringBuilder sb = new StringBuilder();

            string buffer = new string(' ', indent);

            //If we are indented, continuing the main tree branch
            // e.g. |   |
            if (indent > 0) {
                buffer = $"|{buffer}";
            }

            //If obj is null we return (NULL) as the string representation
            //eg. | |
            //    | +--(NULL)
            if (obj is null) {
                sb.AppendLine($"{buffer}|");
                sb.AppendLine($"{buffer}+--(NULL)");
                return sb.ToString();
            }

            Type objType = obj.GetType();

            //If there is no indentation we must be at the base of the tree.
            //Print the objects name
            if (indent == 0) {
                sb.AppendLine($"{buffer}{objType.Name}");
            }

            var valueTypeProps = GetValueTypeProperties(obj);
            var referenceTypeProps = GetReferenceTypeProperties(obj);

            //Print value type properties first
            //eg. | |
            //    | +--'Property Name': Hello, World! 
            foreach (PropertyInfo prop in valueTypeProps) {
                sb.AppendLine($"{buffer}|");
                sb.AppendLine($"{buffer}+--'{prop.Name}': '{prop.GetValue(obj)}'");
            }

            //Print reference type properties
            foreach (PropertyInfo prop in referenceTypeProps) {

                //Print the buffer which creates a gap between each prop
                // e.g. |   |
                sb.AppendLine($"{buffer}|");

                //Different flows for collections
                //This SO thread suggests also checking for string - however strings shouldn't get through to here
                //https://stackoverflow.com/questions/3569811/how-to-know-if-a-propertyinfo-is-a-collection
                if (prop.PropertyType.IsAssignableTo(typeof(IEnumerable)) && prop.PropertyType != typeof(string)) {

                    //Lots of null forgiving operators
                    //Less readable but saves breaking this step up over multiple lines
                    List<object>? list = ((IEnumerable?)prop.GetValue(obj))?.Cast<object>()?.ToList();

                    //It's possible that the list ends up being null
                    //Due to how collections are used they need to be handled differently than other ref types
                    if (list is not null) {
                        for (int i = 0; i < list.Count(); i++) {

                            string propName = $"{prop.Name}[{i}]";
                            int thisPropIndent = indent + propName.Length;

                            sb.Append($"{buffer}+--{propName}\n{GetPropertyTreeAsString(list[i], thisPropIndent)}");
                        }
                    }
                    else {
                        sb.Append($"{buffer}+--{prop.Name}: (NULL)");
                    }

                }
                else {
                    int thisPropIndent = indent + 2 + prop.Name.Length;
                    sb.Append($"{buffer}+--{prop.Name}\n{GetPropertyTreeAsString(prop.GetValue(obj), thisPropIndent)}");
                }

            }

            return sb.ToString();
        }
    }
}
