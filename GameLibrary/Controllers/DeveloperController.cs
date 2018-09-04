using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GameLibrary.Data;
using GameLibrary.Extensions;
using GameLibrary.Helpers;
using GameLibrary.Models;
using GameLibrary.Models.DeveloperViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameLibrary.Controllers
{
    [Authorize]
    public class DeveloperController : GameLibController
    {
        private readonly IDeveloperRepo _repo;

        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private readonly Cloudinary _cloudinary;


        [BindProperty]
        public DeveloperViewModel DevViewModel { get; set; }

        public DeveloperController(IDeveloperRepo repo, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

            DevViewModel = new DeveloperViewModel()
            {
                Developer = new Developer(),
                DeveloperList = _repo.GetDevelopersSync()
            };
        }

        // GET developers/index
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(DevViewModel);
        }

        // GET developers/create
        public IActionResult Create()
        {
            return View();
        }

        // POST developers/create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeveloper()
        {
            if (!ModelState.IsValid)
                return View(DevViewModel);

            var file = DevViewModel.DeveloperImage;

            // if file is uploaded with form
            if (file != null && file.Length > 0)
            {
                var validation = new ImageValidation() { FileSize = 3000 };

                var validationResult = validation.FileCheck(file);

                if (validationResult == "ok")
                {
                    //image validation succeeds
                    var uploadResult = new ImageUploadResult();

                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream)
                        };

                        uploadResult = _cloudinary.Upload(uploadParams);
                    }

                    if (uploadResult.Error == null)
                    {
                        DevViewModel.Developer.PhotoId = uploadResult.PublicId;
                        DevViewModel.Developer.PhotoUrl = uploadResult.Uri.ToString();
                    }
                }
                else //image validation fails
                {
                    SetMessage("danger", validationResult);
                    return RedirectToAction(nameof(Index));
                }
            }

            _repo.Add(DevViewModel.Developer);

            if (await _repo.SaveAll())
            {
                SetMessage("info", "Developer added");
                return RedirectToAction(nameof(Index));
            }

            SetMessage("danger", "Something went wrong while trying to save changes to the database");
            return RedirectToAction(nameof(Index));
        }

        // GET developers/edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            DevViewModel.Developer = await _repo.GetDeveloper(id);

            if (DevViewModel.Developer == null)
                return NotFound();

            return View(DevViewModel);
        }

        // POST developers/edit/id
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeveloper(int? id)
        {
            if (!ModelState.IsValid)
                return View(DevViewModel);

            if (id != DevViewModel.Developer.Id)
                return BadRequest();

            var devFromRepo = await _repo.GetDeveloper(id);

            if (devFromRepo == null)
                return NotFound();

            var file = DevViewModel.DeveloperImage;

            if (file != null && file.Length > 0)
            {
                var validation = new ImageValidation() { FileSize = 3000 };

                var validationResult = validation.FileCheck(file);

                if (validationResult == "ok")
                {
                    var uploadResult = new ImageUploadResult();

                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream)
                        };

                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                    if (uploadResult.Error == null)
                    {
                        if (devFromRepo.PhotoId != null)
                        {
                            var deletionParams = new DeletionParams(devFromRepo.PhotoId);
                            _cloudinary.Destroy(deletionParams);
                        }

                        devFromRepo.PhotoId = uploadResult.PublicId;
                        devFromRepo.PhotoUrl = uploadResult.Uri.ToString();
                    }
                }
                else
                {
                    SetMessage("danger", validationResult);
                    return RedirectToAction(nameof(Index));
                }
            }

            devFromRepo.Name = DevViewModel.Developer.Name;
            devFromRepo.Created = DevViewModel.Developer.Created;
            devFromRepo.IsActive = DevViewModel.Developer.IsActive;

            if (await _repo.SaveAll())
            {
                SetMessage("info", "Changes saved");
                return RedirectToAction(nameof(Index));
            }

            SetMessage("danger", "Something went wrong. Could not complete request.");
            return RedirectToAction(nameof(Index));
        }

        // GET developers/delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            DevViewModel.Developer = await _repo.GetDeveloper(id);

            if (DevViewModel.Developer == null)
                return NotFound();

            return View(DevViewModel);
        }

        // POST developers/delete/id
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeveloper([FromRoute]int? id, [FromForm]int formId)
        {
            if (id != formId)
                return BadRequest();

            var devForDeletion = await _repo.GetDeveloper(id);

            if (devForDeletion == null)
                return NotFound();

            List<string> photosForDeletion = new List<string>();

            if (devForDeletion.PhotoId != null)
            {
                photosForDeletion.Add(devForDeletion.PhotoId);
            }

            if (devForDeletion.Games.ToList().Count > 0)
            {
                foreach (Game game in devForDeletion.Games)
                {
                    if (!String.IsNullOrEmpty(game.PhotoId))
                    {
                        photosForDeletion.Add(game.PhotoId);
                    }
                }
            }

            if (photosForDeletion.Count > 0)
            {
                var delParams = new DelResParams()
                {
                    PublicIds = photosForDeletion
                };

                _cloudinary.DeleteResources(delParams);
            }

            _repo.Delete(devForDeletion);

            if (await _repo.SaveAll())
            {
                SetMessage("info", "Developer deleted");
                return RedirectToAction(nameof(Index));
            }

            SetMessage("danger", "Something went wrong. Could not complete request");
            return RedirectToAction(nameof(Index));
        }

        // GET developers/details/id
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            DevViewModel.Developer = await _repo.GetDeveloper(id);

            if (DevViewModel.Developer == null)
                return NotFound();

            return View(DevViewModel);
        }
    }
}