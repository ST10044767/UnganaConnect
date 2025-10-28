using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnganaConnect.Frontend.Models;

namespace UnganaConnect.Frontend.Controllers
{
    public class ResourceController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = new ResourceLibraryViewModel
            {
                Resources = GetResources()
            };
            return View(viewModel);
        }

        [Authorize]
        public IActionResult Preview(int id)
        {
            var resource = GetResourceById(id);
            return View(resource);
        }

        private List<ResourceViewModel> GetResources()
        {
            return new List<ResourceViewModel>
            {
                new() { Id = 1, Title = "Grant Writing Templates", Category = "Grant Writing", Type = "Templates", Description = "Comprehensive templates for grant proposals", Downloads = 245, Url = "https://www.youtube.com/embed/Vl0H-qTclOg" },
                new() { Id = 2, Title = "Financial Management Toolkit", Category = "Finance", Type = "Toolkit", Description = "Essential tools for CSO financial management", Downloads = 189, Url = "https://www.youtube.com/embed/yZvFH7B6gKI" },
                new() { Id = 3, Title = "Project Planning Guide", Category = "Project Management", Type = "Guide", Description = "Step-by-step project planning methodology", Downloads = 156, Url = "https://www.youtube.com/embed/3qYbVd7DILs" },
                new() { Id = 4, Title = "Digital Marketing Strategies", Category = "Marketing", Type = "Guide", Description = "Modern marketing approaches for CSOs", Downloads = 134, Url = "https://www.youtube.com/embed/c9Wg6Cb_YlU" },
                new() { Id = 5, Title = "Cybersecurity Checklist", Category = "Security", Type = "Checklist", Description = "Essential security measures for organizations", Downloads = 98, Url = "https://www.youtube.com/embed/inWWhr5tnEA" },
                new() { Id = 6, Title = "Data Analytics Templates", Category = "Analytics", Type = "Templates", Description = "Ready-to-use data analysis templates", Downloads = 87, Url = "https://www.youtube.com/embed/a9UrKTVEeZA" }
            };
        }

        private ResourceViewModel GetResourceById(int id)
        {
            var resources = GetResources();
            return resources.FirstOrDefault(r => r.Id == id) ?? resources.First();
        }
    }
}