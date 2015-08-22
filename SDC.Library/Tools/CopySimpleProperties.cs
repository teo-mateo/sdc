using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.Tools
{
    public class CopySimpleProperties
    {
        /// <summary>
        /// Copies all public fields values from the source object to the destination values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static TDest Copy<TSource, TDest>(TSource source, TDest dest) 
        {
            Type[] supportedTypes = new Type[]
            {
                typeof(int),
                typeof(double),
                typeof(bool),
                typeof(string),
                typeof(DateTime)
            };

            //we're only going to copy the instance fields that are public.
            var sprops = typeof(TSource).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var dprops = typeof(TDest).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var f in sprops)
            {
                // I SHOULD WANT TO DO THIS FOR CERTAIN TYPES: 
                // int, string. not complex types.
                // I don't want to do a real deep copy.
                if (!supportedTypes.Contains(f.PropertyType))
                    continue;

                //get the value from the source prop
                var value = f.GetValue(source);
                //set the value to the destination object's property with the same name.
                var targetprop = dprops.FirstOrDefault(p => p.Name == f.Name);
                if (targetprop != null)
                    targetprop.SetValue(dest, value);
            }




            //we're only going to copy the instance fields that are public.
            var sfields = typeof(TSource).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var dfields = typeof(TDest).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var f in sfields)
            {
                // I SHOULD WANT TO DO THIS FOR CERTAIN TYPES: 
                // int, string. not complex types.
                // I don't want to do a real deep copy.
                if (!supportedTypes.Contains(f.FieldType))
                    continue;

                //get the value from the source prop
                var value = f.GetValue(source);
                //set the value to the destination object's property with the same name.
                var targetfield = dfields.FirstOrDefault(p => p.Name == f.Name);
                if (targetfield != null)
                    targetfield.SetValue(dest, value);
            }

            return dest;
        }
    }
}
