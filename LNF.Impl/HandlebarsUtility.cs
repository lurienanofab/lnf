using HandlebarsDotNet;
using System;
using System.IO;

namespace LNF.Impl
{
    public static class HandlebarsUtility
    {
        static HandlebarsUtility()
        {
            Handlebars.RegisterHelper("format", (writer, context, parameters) =>
            {
                if (parameters.Length == 0)
                {
                    // do nothing
                    return;
                }

                var val = parameters[0].ToString();

                string format = null;

                if (parameters.Length > 1)
                    format = parameters[1].ToString();

                if (DateTime.TryParse(val, out DateTime date))
                {
                    writer.WriteSafeString(date.ToString(format));
                }
                else if (double.TryParse(val, out double num))
                {
                    writer.WriteSafeString(num.ToString(format));
                }
                else
                {
                    // return the parameter as is
                    writer.WriteSafeString(val);
                }
            });
        }

        public static HandlebarsTemplate<object, object> GetTemplate(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"Cannot find template file: {path}");

            var template = File.ReadAllText(path);
            var result = Handlebars.Compile(template);

            return result;
        }
    }
}
