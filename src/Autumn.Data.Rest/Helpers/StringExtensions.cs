using System.Text;

namespace Autumn.Data.Rest.Helpers
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            if (value.Length == 1) return value.ToLowerInvariant();
            return value[0].ToString().ToLower() + value.Substring(1);
        }

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