using Microsoft.Extensions.Localization;

namespace MyProject.Admin.Helpers.Localization
{
    public interface IGenericControllerLocalizer<T> : IStringLocalizer<T>
    {
        
    }
}