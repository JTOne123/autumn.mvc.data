using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data
{
    public static class AutumnExtensions
    {
        /// <summary>
        /// format string to camelCase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (value.Length == 1) return value.ToLowerInvariant();
            var result = ToPascalCase(value);
            return result[0].ToString(CultureInfo.InvariantCulture).ToLowerInvariant() + result.Substring(1);
        }

        /// <summary>
        /// format string to case
        /// </summary>
        /// <param name="value"></param>
        /// <param name="namingStrategy"></param>
        /// <returns></returns>
        public static string ToCase(this string value, NamingStrategy namingStrategy)
        {
            switch (namingStrategy)
            {
                case CamelCaseNamingStrategy _:
                    return ToCamelCase(value);
                case SnakeCaseNamingStrategy _:
                    return ToSnakeCase(value);
            }
            return value;
        }

        /// <summary>
        /// format string to PascalCase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPascalCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (value.Length == 1) return value.ToUpperInvariant();
            var items = value.Split('_');
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                switch (item.Length)
                {
                    case 0:
                        continue;
                    case 1:
                        builder.Append(item[0].ToString().ToUpperInvariant());
                        break;
                    default:
                        builder.Append(item[0].ToString().ToUpperInvariant() + item.Substring(1));
                        break;
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// format string to snake_case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (value.Length == 1) return value.ToLowerInvariant();
            var builder = new StringBuilder();
            foreach (var item in value)
            {
                if (char.IsLetter(item) && char.IsUpper(item))
                {
                    builder.Append((builder.Length > 0 ? "_" : "") + item.ToString().ToLowerInvariant());
                }
                else
                {
                    builder.Append(item);
                }
            }
            return builder.ToString();
        }
    }
}