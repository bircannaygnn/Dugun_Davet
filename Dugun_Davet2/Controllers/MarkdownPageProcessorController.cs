using Microsoft.AspNetCore.Mvc;
using Westwind.AspNetCore.Markdown;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace Westwind.AspNetCore.Markdown.Utilities
{
    internal static class StringUtils
    {
        /// <summary>
        /// Extracts a string from between a pair of delimiters. Only the first 
        /// instance is found.
        /// </summary>
        /// <param name="source">Input String to work on</param>
        /// <param name="StartDelim">Beginning delimiter</param>
        /// <param name="endDelim">ending delimiter</param>
        /// <param name="CaseInsensitive">Determines whether the search for delimiters is case sensitive</param>
        /// <returns>Extracted string or ""</returns>
        public static string ExtractString(string source,
            string beginDelim,
            string endDelim,
            bool caseSensitive = false,
            bool allowMissingEndDelimiter = false,
            bool returnDelimiters = false)
        {
            int at1, at2;

            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (caseSensitive)
            {
                at1 = source.IndexOf(beginDelim);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length);
            }
            else
            {
                //string Lower = source.ToLower();
                at1 = source.IndexOf(beginDelim, 0, source.Length, StringComparison.OrdinalIgnoreCase);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.OrdinalIgnoreCase);
            }

            if (allowMissingEndDelimiter && at2 < 0)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length);

                return source.Substring(at1);
            }

            if (at1 > -1 && at2 > 1)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length, at2 - at1 - beginDelim.Length);

                return source.Substring(at1, at2 - at1 + endDelim.Length);
            }

            return string.Empty;
        }


        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or empty array if the string passed was a null</returns>
        public static string[] GetLines(string s, int maxLines = 0)
        {
            if (s == null)
                return new string[] { };

            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
                return s.Split('\n');

            return s.Split('\n').Take(maxLines).ToArray();
        }



        public class MarkdownPageProcessorController : Controller
        {
            public MarkdownConfiguration MarkdownProcessorConfig { get; }

            [Obsolete]
            private readonly IHostingEnvironment hostingEnvironment;

            [Obsolete]
            public MarkdownPageProcessorController(
                IHostingEnvironment hostingEnvironment,
                MarkdownConfiguration config)
            {
                MarkdownProcessorConfig = config;
                this.hostingEnvironment = hostingEnvironment;
            }

            [Route("markdownprocessor/markdownpage")]
            [Obsolete]
            public async Task<IActionResult> MarkdownPage()
            {
                var model = HttpContext.Items["MarkdownProcessor_Model"] as MarkdownModel;

                var basePath = hostingEnvironment.WebRootPath;
                var relativePath = model.RelativePath;
                if (relativePath == null)
                    return NotFound();

                if (!System.IO.File.Exists(model.PhysicalPath))
                    return NotFound();

                // string markdown = await File.ReadAllTextAsync(pageFile);
                string markdown;
                using (var fs = new FileStream(model.PhysicalPath,
                    FileMode.Open,
                    FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    markdown = await sr.ReadToEndAsync();
                }

                if (string.IsNullOrEmpty(markdown))
                    return NotFound();

                // set title, raw markdown, yamlheader and rendered markdown
                ParseMarkdownToModel(markdown, model);

                if (model.FolderConfiguration != null)
                {
                    model.FolderConfiguration.PreProcess?.Invoke(model, this);
                    return View(model.FolderConfiguration.ViewTemplate, model);
                }

                return View(MarkdownConfiguration.DefaultMarkdownViewTemplate, model);
            }

            private MarkdownModel ParseMarkdownToModel(string markdown,
                                                       MarkdownModel? model = null)
            {
                if (model == null)
                    model = new MarkdownModel();


                if (model.FolderConfiguration.ExtractTitle)
                {
                    var firstLines = StringUtils.GetLines(markdown, 30);
                    var firstLinesText = String.Join("\n", firstLines);

                    // Assume YAML 
                    if (markdown.StartsWith("---"))
                    {
                        var yaml = StringUtils.ExtractString(firstLinesText, "---", "---", returnDelimiters: true);
                        if (yaml != null)
                        {
                            model.Title = StringUtils.ExtractString(yaml, "title: ", "\n");
                            model.YamlHeader = yaml.Replace("---", "").Trim();
                        }
                    }

                    if (model.Title == null)
                    {
                        foreach (var line in firstLines.Take(10))
                        {
                            if (line.TrimStart().StartsWith("# "))
                            {
                                model.Title = line.TrimStart(new char[] { ' ', '\t', '#' });
                                break;
                            }
                        }
                    }
                }

                model.RawMarkdown = markdown;
                model.RenderedMarkdown = Markdown.ParseHtmlString(markdown);

                return model;
            }
        }
    }
}