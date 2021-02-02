using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebApplication3.Models;
using WebApplication3.Repository;
using WebApplication3.Repository.IRepository;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class NationalParkController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;

        public NationalParkController(INationalParkRepository nationalParkRepository)
        {
            _nationalParkRepository = nationalParkRepository;
        }
        // GET
        public IActionResult Index()
        {
            return View(new NationalPark(){});
        }
        
        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new {data = await _nationalParkRepository.GetAllAsync(SD.NationalAPIPath, HttpContext.Session.GetString("JWToken"))});
        }
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();
            if (id == null)
            {
                return View(obj);
            }

            obj = await _nationalParkRepository.GetAsync(SD.NationalAPIPath, id.GetValueOrDefault(),HttpContext.Session.GetString("JWToken"));
            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark obj)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {       
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyToAsync(ms1);
                            p1 = ms1.ToArray();
                        }
                    }

                    obj.Picture = p1;
                }
                else
                {
                    var objFromDb = await _nationalParkRepository.GetAsync(SD.NationalAPIPath, obj.Id, HttpContext.Session.GetString("JWToken"));
                    obj.Picture = objFromDb.Picture;
                }

                if (obj.Id == 0)
                {
                    await _nationalParkRepository.CreateAsync(SD.NationalAPIPath, obj, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _nationalParkRepository.UpdateAsync(SD.NationalAPIPath + obj.Id, obj, HttpContext.Session.GetString("JWToken"));
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(obj);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _nationalParkRepository.DeleteAsync(SD.NationalAPIPath, id, HttpContext.Session.GetString("JWToken"));
            if (status)
            {
                return Json(new {success = true, message = "Delete Successful"});
            }
            return Json(new {success = false, message = "Delete Successful"});
        }
    }
}