using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

public class GlobalRoutePrefixConvention : IApplicationModelConvention
{
    private readonly string prefix;
    public GlobalRoutePrefixConvention(IConfiguration configuration)
    {
        prefix = configuration.GetSection("Api").GetValue<string>("GlobalApiPrefix") ?? "api";
    }
    public void Apply(ApplicationModel application)
    {
        foreach(var controller in application.Controllers)
        {
            var route = controller.Selectors.FirstOrDefault(s => s.AttributeRouteModel != null);
            if (route != null)
            {
                route.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                    new AttributeRouteModel(new RouteAttribute(prefix)),
                    route.AttributeRouteModel
                );
            }
            else
            {
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(prefix + "[controller]"))
                });
            }
        }
    }
}