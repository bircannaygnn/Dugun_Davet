using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;


namespace Dugun_Davet2.Controllers
{
    public class CustomMarkdownController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;

        public CustomMarkdownController(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

    [Route("Documentation/{*id=Index}")]
    public async Task<IActionResult> Index(string id)

{
    if (!id.EndsWith(".md"))
    {
        id = id + ".md";
    }
    var basePath = hostingEnvironment.WebRootPath;
    string relativeFolder = "Posts/Documentation";
    string relativePath = Path.Combine(relativeFolder, id);

    var pageFile = Path.Combine(basePath, relativePath);
    if (!System.IO.File.Exists(pageFile))
        return NotFound();

    var markdown = await System.IO.File.ReadAllTextAsync(pageFile);
    if (string.IsNullOrEmpty(markdown))
        return NotFound();

    if (markdown.Contains("{{username}}"))
    { markdown = markdown.Replace("{{username}}", "Bircan"); }

    ViewBag.id = Markdown.ParseHtmlString(markdown);
    return View();
}
        static string NormalizePath(string path)
{
    if (string.IsNullOrEmpty(path))
        return path;
    char slash = Path.DirectorySeparatorChar;
    path = path.Replace('/', slash).Replace('\\', slash);
    return path.Replace(slash.ToString() + slash.ToString(), slash.ToString());
}
    }
}
