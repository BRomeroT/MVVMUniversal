using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Core.Helpers
{
    /// <summary>
    /// Enumeración de tipos de conversión.
    /// </summary>
    public enum DateTimeZones
    {
        ToLocal,
        ToUTC,
        Custom
    }

    /// <summary>
    /// Clase que se usa para convertir el valor de un tipo DateTime de UTC a Local y Local a UTC.
    /// </summary>
    public static class DateTimeConverter
    {
        /// <summary>
        /// Tipos excluidos de las propiedades para no iterarlas.
        /// </summary>
        static readonly List<Type> typesExclude = new List<Type>
        {
            typeof(string),
            typeof(int),
            typeof(short),
            typeof(bool),
            typeof(byte),
            typeof(double),
            typeof(decimal),
            typeof(float),
            typeof(char),
            typeof(long)
         };

        /// <summary>
        ///Conversión de zona horaria cuando el tipo de dato es atomico.
        /// </summary>
        /// <typeparam name="T">Tipo de dato.</typeparam>
        /// <param name="obj">Objeto al que se aplica la conversión de valor.</param>
        /// <param name="timeZone">Tipo de conversión de fecha y hora.</param>
        /// <returns>Tipo de dato genérico.</returns>
        public static T? ChangeOutDateTime<T>(this T? obj, DateTimeZones timeZone)
        {
            if (obj == null) return obj;
            Type objType = obj.GetType();
            if (typesExclude.Contains(objType))
                return obj;
            else if (objType == typeof(DateTime) || objType == typeof(DateTime?))
            {
                if (DateTimeZones.ToLocal == timeZone)
                    return (T)Convert.ChangeType(((DateTime)(object)obj!).ToLocalTime(), objType);
                else if (DateTimeZones.ToUTC == timeZone)
                    return (T)Convert.ChangeType(((DateTime)(object)obj!).ToUniversalTime(), objType);
                else
                {
                    //Mejorar cuando requiera personalizar la zona horaria
                    return obj;
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(objType))
            {
                var childs = obj as IEnumerable;
                if (childs != null)
                {
                    foreach (var child in childs)
                    {
                        child?.ChangeInTimeZone(timeZone);
                    }
                }
                return obj;
            }
            else
            {
                var properties = objType.GetProperties().Where(p => !typesExclude.Contains(p.PropertyType));
                foreach (PropertyInfo prop in properties)
                {
                    object? value = prop.GetValue(obj, null);
                    if (value == null) continue;

                    var elements = value as IEnumerable;
                    bool hasElements = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) && elements != null;
                    if (hasElements)
                        foreach (var element in elements!)
                            element?.ChangeOutDateTime(timeZone);
                    else if (value != null && (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)))
                    {
                        if (timeZone == DateTimeZones.ToLocal)
                            prop.SetValue(obj, ((DateTime)value).ToLocalTime());
                        else if (timeZone == DateTimeZones.ToUTC)
                            prop.SetValue(obj, ((DateTime)value).ToUniversalTime());
                        else if (timeZone == DateTimeZones.Custom)
                            prop.SetValue(obj, ((DateTime)value).ToUniversalTime());
                    }
                    else if (prop.DeclaringType?.IsClass == true)
                        value.ChangeOutDateTime(timeZone);
                }
                return obj;
            }
        }

        /// <summary>
        /// Conversión de zona horaria cuando el tipo de dato es objeto o lista
        /// </summary>
        /// <typeparam name="T">Tipo de dato.</typeparam>
        /// <param name="obj">Objeto al que se aplica la conversión de valor.</param>
        /// <param name="timeConvertion">Tipo de conversión de fecha y hora.</param>
        public static void ChangeInTimeZone<T>(this T? obj, DateTimeZones timeConvertion)
        {
            if (obj == null) return;
            try
            {
                Type objType = obj.GetType();
                if (objType == typeof(Tuple) || objType.Name.Contains("Tuple"))
                    obj.ChangeTupleTimeZone(timeConvertion);
                else if (typeof(IEnumerable).IsAssignableFrom(objType))
                {
                    var childs = obj as IEnumerable;
                    if (childs != null)
                    {
                        foreach (var child in childs)
                        {
                            child?.ChangeInTimeZone(timeConvertion);
                        }
                    }
                }
                else
                {
                    var properties = objType.GetProperties().Where(p => !typesExclude.Contains(p.PropertyType));
                    foreach (PropertyInfo prop in properties)
                    {
                        object? value = prop.GetValue(obj, null);
                        if (value == null) continue;

                        var elements = value as IEnumerable;
                        bool hasElements = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>) && elements != null;
                        if (hasElements)
                            foreach (var element in elements!)
                                element?.ChangeInTimeZone(timeConvertion);
                        else if (value != null && (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)))
                        {
                            DateTime dateTimeValue = prop.PropertyType == typeof(DateTime?) 
                                ? ((DateTime?)value).Value 
                                : (DateTime)value;
                                
                            if (timeConvertion == DateTimeZones.ToLocal)
                                prop.SetValue(obj, dateTimeValue.ToLocalTime());
                            else if (timeConvertion == DateTimeZones.ToUTC)
                                prop.SetValue(obj, dateTimeValue.ToUniversalTime());
                            else if (timeConvertion == DateTimeZones.Custom)
                                prop.SetValue(obj, dateTimeValue.ToUniversalTime());
                        }
                        else if (prop.DeclaringType?.IsClass == true)
                            value.ChangeInTimeZone(timeConvertion);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Conversión de zona horaria dentro de tuplas.
        /// </summary>
        /// <typeparam name="T">Tipo de dato.</typeparam>
        /// <param name="tuple">Tupla.</param>
        /// <param name="timeConvertion">Tipo de conversión de fecha y hora.</param>
        public static void ChangeTupleTimeZone<T>(this T? tuple, DateTimeZones timeConvertion)
        {
            if (tuple == null) return;
            
            var fieldsWithTuples = tuple
                .GetType()
                .GetFields(BindingFlags.NonPublic |
                           BindingFlags.Instance |
                           BindingFlags.Public |
                           BindingFlags.Static |
                           BindingFlags.FlattenHierarchy);

            foreach (var field in fieldsWithTuples)
                field.GetValue(tuple)?.ChangeInTimeZone(timeConvertion);
        }
    }
}
