using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Madayn.Web.Controllers;

public class SharedResource { }

public abstract class BaseController : Controller
{
    protected readonly IStringLocalizer<SharedResource> Localizer;

    protected BaseController(IStringLocalizer<SharedResource> localizer)
    {
        Localizer = localizer;
    }

    protected string GetCurrentLanguage()
    {
        return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    }

    protected bool IsArabic => GetCurrentLanguage() == "ar";
}
