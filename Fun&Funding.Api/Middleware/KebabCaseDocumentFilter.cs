using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fun_Funding.Api.Middleware
{
    public class KebabCaseDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    foreach (var parameter in operation.Value.Parameters)
                    {
                        parameter.Name = ToKebabCase(parameter.Name);
                    }
                }
            }
        }

        private string ToKebabCase(string value)
        {
            return string.Concat(
                value.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())
            ).ToLower();
        }
    }
}
