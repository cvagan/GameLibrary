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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameLibrary.Controllers
{
    [Authorize]
    public class GamesController : GameLibController
    {
        private readonly IDeveloperRepo _repo;

        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private readonly Cloudinary _cloudinary;

        [BindProperty]
        public GameViewModel GameViewModel { get; set; }

        public GamesController(IDeveloperRepo repo, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);

            GameViewModel = new GameViewModel()
            {
                Game = new Game(),
                Games = _repo.GetGamesSync()
            };
        }

        // GET game/create
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(GameViewModel);
        }

        public async Task<IActionResult> Create()
        {
            GameViewModel.Developers = await _repo.GetDevelopers();

            return View(GameViewModel);
        }

        // POST game/create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGame()
        {
            if (!ModelState.IsValid)
                return View(GameViewModel);

            var file = GameViewModel.GameImage;

            var uploadResult = new ImageUploadResult();

            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
                GameViewModel.Game.PhotoUrl = uploadResult.Uri.ToString();
                GameViewModel.Game.PhotoId = uploadResult.PublicId;
            }

            _repo.Add(GameViewModel.Game);

            if (await _repo.SaveAll())
            {
                SetMessage("info", "Game added");
                return RedirectToAction(nameof(Index));
            }

            SetMessage("danger", "Something went wrong when saving to database");
            return RedirectToAction(nameof(Index));
        }

        // GET game/edit/id
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            GameViewModel.Game = await _repo.GetGame(id);
            GameViewModel.Developers = await _repo.GetDevelopers();

            if (GameViewModel.Game == null)
                return NotFound();

            return View(GameViewModel);
        }

        // POST game/edit/id
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGame([FromRoute]int? id)
        {
            if (!ModelState.IsValid)
            {
                GameViewModel.Developers = await _repo.GetDevelopers();
                return View(GameViewModel);
            }

            if (id != GameViewModel.Game.Id)
            {
                SetMessage("danger", "Something went wrong");
                return RedirectToAction(nameof(Index));
            }

            var gameFromRepo = await _repo.GetGame(id);

            if (gameFromRepo == null)
            {
                SetMessage("danger", "Not found");
                return RedirectToAction(nameof(Index));
            }

            var devFromRepo = await _repo.GetDeveloper(GameViewModel.Game.DeveloperId);

            if (devFromRepo == null)
            {
                SetMessage("danger", "Invalid developer");
                return RedirectToAction(nameof(Index));
            }

            var file = GameViewModel.GameImage;

            var uploadResult = new ImageUploadResult();

            // if new image is uploaded
            if (file != null && file.Length > 0)
            {
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
                    if (gameFromRepo.PhotoId != null)
                    {
                        var deletionParams = new DeletionParams(gameFromRepo.PhotoId);
                        var deletionResult = _cloudinary.Destroy(deletionParams);
                    }

                    gameFromRepo.PhotoId = uploadResult.PublicId;
                    gameFromRepo.PhotoUrl = uploadResult.Uri.ToString();
                }
            }

            gameFromRepo.Name = GameViewModel.Game.Name;
            gameFromRepo.Year = GameViewModel.Game.Year;
            gameFromRepo.Rating = GameViewModel.Game.Rating;
            gameFromRepo.DeveloperId = GameViewModel.Game.DeveloperId;

            if (await _repo.SaveAll())
            {
                SetMessage("info", "Changes saved");
                return RedirectToAction(nameof(Index));
            }

            SetMessage("danger", "Could not save changes to database");
            return RedirectToAction(nameof(Index));
        }

        // GET game/details/id
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            GameViewModel.Game = await _repo.GetGame(id);

            if (GameViewModel.Game == null)
                return NotFound();

            return View(GameViewModel);
        }

        // GET game/delete/id
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            GameViewModel.Game = await _repo.GetGame(id);

            if (GameViewModel.Game == null)
                return NotFound();

            return View(GameViewModel);
        }

        // POST game/delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame([FromRoute]int id, [FromForm]int formId)
        {
            if (id != formId)
                return BadRequest();

            var game = await _repo.GetGame(id);

            if (game == null)
                return NotFound();

            if (game.PhotoId != null)
            {
                var deleteParams = new DeletionParams(game.PhotoId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result != "ok")
                {
                    return BadRequest();
                }
            }

            _repo.Delete(game);

            if (await _repo.SaveAll())
            {
                SetMessage("danger", "Game deleted");
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }
    }
}