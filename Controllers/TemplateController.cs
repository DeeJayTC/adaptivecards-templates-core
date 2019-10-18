using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using adaptivecards_templates_core.Models;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace adaptivecards_templates_core.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public TemplateController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> GetTemplate()
        {
            try
            {
                var templateSearchName = Request.Path.Value;
                using (StreamReader sr = new StreamReader($".\\templates{templateSearchName}"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String templateFile = await sr.ReadToEndAsync();
                    return Ok(templateFile);
                }

                return NotFound();
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }


        public async Task<IActionResult> ListTemplates()
        {
            try
            {
                var templateRoot = ".\\templates";
                var list = GetChildren(templateRoot);
                return Ok(JsonConvert.SerializeObject(list, Formatting.Indented));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public List<TemplateList> GetChildren(string path)
        {
            var list = new List<TemplateList>();
            var children = System.IO.Directory.EnumerateFileSystemEntries(path).ToList();
            children.ForEach(delegate (String name)
            {
                try
                {
                    var fileName = GrabFileName(name);

                    if (string.IsNullOrEmpty(fileName))
                    {
                        list.Add(new TemplateList()
                        {
                            Path = name.Replace(".\\templates\\", "").Replace(@"\\", @"\"),
                            Templates = name.Contains(".json") ? null : GetChildren(name)
                        });
                    }
                    else
                    {
                        list.Add(new TemplateList()
                        {
                            Path = name.Replace(".\\templates\\", "").Replace(@"\\", @"\"),
                            Name = fileName
                        });
                    }
                }
                catch(Exception ex)
                {
                    list.Add(new TemplateList()
                    {
                        Path = path.Replace(".\\templates\\", "").Replace(@"\\", @"\"),
                        Name = "Error parsing path"
                    });
                }



            });
            return list;
        }

        public class TemplateList
        {
            [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
            public string Path { get; set; }
            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }
            [JsonProperty("templates", NullValueHandling = NullValueHandling.Ignore)]
            public List<TemplateList> Templates { get; set; }
        }


        public string GrabFileName(string filename)
        {
            string[] temparray = filename.Split('\\');
            var file = temparray[temparray.Length - 1];
            if (file.Contains(".json")) return file;
            return "";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
