using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PHCWS.App_Start
{
	public class AddRequiredHeaderParameter : IOperationFilter
	{
		void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			throw new System.NotImplementedException();
		}
	}
}