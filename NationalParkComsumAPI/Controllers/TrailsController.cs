using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebApplication3.Models;
using WebApplication3.Models.ViewModels;
using WebApplication3.Repository;
using WebApplication3.Repository.IRepository;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly ITrailRepository _trailRepository;

        public TrailsController(INationalParkRepository nationalParkRepository, ITrailRepository trailRepository)
        {
            _nationalParkRepository = nationalParkRepository;
            _trailRepository = trailRepository;
        }
        // GET
        public IActionResult Index()
        {
            return View(new Trail(){});
        }
        
        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new {data = await _trailRepository.GetAllAsync(SD.TrailsAPIPath,HttpContext.Session.GetString("JWToken"))});
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(SD.NationalAPIPath,HttpContext.Session.GetString("JWToken"));
            TrailsVM trailsVm = new TrailsVM()
            {
                
                NationParkList = npList.Select(i=>  new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };
            if (id == null)
            {
                return View(trailsVm);
            }

            trailsVm.Trail = await _trailRepository.GetAsync(SD.TrailsAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));
            if (trailsVm.Trail == null)
            {
                return NotFound();
            }

            return View(trailsVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Trail.Id == 0)
                {
                    await _trailRepository.CreateAsync(SD.TrailsAPIPath, obj.Trail,HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _trailRepository.UpdateAsync(SD.TrailsAPIPath + obj.Trail.Id, obj.Trail, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(SD.NationalAPIPath, HttpContext.Session.GetString("JWToken"));
                TrailsVM trailsVm = new TrailsVM()
                {
                
                    NationParkList = npList.Select(i=>  new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    }),
                    Trail = obj.Trail
                };
                return View(trailsVm);
            }
        }
        [HttpDelete]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepository.DeleteAsync(SD.TrailsAPIPath, id,HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new {success = true, message = "Delete Successful"});
            }
            return Json(new {success = false, message = "Delete Successful"});
        }
    }
}