using System.Text.Json;

namespace Fun_Funding.Api.Middleware
{
    public class KebabCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return string.Concat(
                name.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())
            ).ToLower();
        }
    }
}
