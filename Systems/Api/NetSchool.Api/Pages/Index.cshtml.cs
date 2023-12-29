using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetSchool.Api.Settings;
using NetSchool.Common;
using NetSchool.Services.Settings;

namespace NetSchool.Api.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SwaggerSettings _swaggerSettings;
        private readonly ApiSpecialSettings _apiSpecialSettings;
        
        [BindProperty]
        public bool OpenApiEnabled => _swaggerSettings.Enabled;

        [BindProperty]
        public string Version => Assembly.GetExecutingAssembly().GetAssemblyVersion();


        [BindProperty]
        public string HelloMessage => _apiSpecialSettings.HelloMessage;

        public IndexModel(SwaggerSettings swaggerSettings, ApiSpecialSettings apiSpecialSettings)
        {
            _swaggerSettings = swaggerSettings;
            _apiSpecialSettings = apiSpecialSettings;
        }

        public void OnGet()
        {

        }
    }
}