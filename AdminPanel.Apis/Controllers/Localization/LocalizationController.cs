using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace AdminPanel.Apis.Controllers.Localization
{
    public class LocalizationController : BaseController
    {
        private readonly IStringLocalizer _localizer;

        public LocalizationController(IStringLocalizerFactory factory)
        {
            _localizer = factory.Create(typeof(SharedResource));
        }

        [HttpGet] // GET /api/localization?culture=ar
        public IActionResult Get([FromQuery] string culture = "en")
        {
            var result = new Dictionary<string, string>();

            foreach (var entry in _localizer.GetAllStrings(includeParentCultures: true))
            {
                result[entry.Name] = entry.Value;
            }

            return Ok(result);
        }
    }
}
