using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Linq;

namespace Autumn.Mvc.Data.MongoDB.Conventions
{
    public class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention, IConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            var elementName = this.GetElementName(memberMap.MemberName);
            memberMap.SetElementName(elementName);
        }

        private string GetElementName(string memberName)
        {
            switch (memberName.Length)
            {
                case 0:
                    return "";
                case 1:
                    return char.ToLowerInvariant(memberName[0]).ToString();
            }

            return string.Concat(memberName.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? string.Concat("_", x.ToString()) : x.ToString())).ToLower();
        }
    }
}