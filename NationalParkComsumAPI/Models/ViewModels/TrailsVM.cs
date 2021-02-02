using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication3.Models.ViewModels
{
    public class TrailsVM
    {
        public IEnumerable<SelectListItem> NationParkList { get; set; }
        public Trail Trail { get; set; }
    }
}