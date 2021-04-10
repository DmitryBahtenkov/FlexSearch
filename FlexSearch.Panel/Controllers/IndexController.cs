using System;
using System.Linq;
using System.Threading.Tasks;
using FlexSearch.Panel.Helpers;
using FlexSearch.Panel.Models.CoreModels;
using FlexSearch.Panel.Models.ViewModels;
using FlexSearch.Panel.Services.Contract;
using Microsoft.AspNetCore.Mvc;

namespace FlexSearch.Panel.Controllers
{
    [Route("[controller]")]
    public class IndexController : Controller
    {
        private readonly IIndexService _indexService;

        public IndexController(IIndexService indexService)
        {
            _indexService = indexService;
        }

        [HttpGet("databases")]
        public async Task<IActionResult> Databases()
        {
            _indexService.Initialize(UserHelper.CurrentUser);
            var model = await _indexService.GetDbs();
            return View(model.ToList());
        }
        
        [HttpGet("indexes/{dbname}")]
        public async Task<IActionResult> Indexes(string dbname)
        {
            ViewData.Add("Database", dbname);
            _indexService.Initialize(UserHelper.CurrentUser);
            var model = await _indexService.GetIndexes(dbname);
            return View(model.ToList());
        }
        
        [HttpGet("documents/{dbname}/{index}")]
        public async Task<IActionResult> Documents(string dbname, string index)
        {
            ViewData.Add("database", dbname);
            ViewData.Add("index", index);
            _indexService.Initialize(UserHelper.CurrentUser);
            var model = await _indexService.GetAll(dbname, index);
            return View(model.ToList());
        }

        [HttpGet("edit/{dbname}/{index}/{id}")]
        public async Task<IActionResult> Edit(Guid id, string dbname, string index)
        {
            _indexService.Initialize(UserHelper.CurrentUser);
            var doc = await _indexService.GetById(dbname, index, id);
            return View(DocumentMapper.MapViewModel(doc, dbname, index));
        }
        
        [HttpPost("edit/{dbname}/{index}/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DocumentViewModel model, string dbname, string index)
        {
            if (ModelState.IsValid)
            {
                _indexService.Initialize(UserHelper.CurrentUser);
                await _indexService.Update(model.DbName ?? dbname, model.Index ?? index, model.Id.GetValueOrDefault(), model.Json);

                return Redirect($"/Index/Documents/{model.DbName ?? dbname}/{model.Index ?? index}");
            }

            return View(model);
        }
        
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string db, string index, Guid id)
        {
            _indexService.Initialize(UserHelper.CurrentUser);
            await _indexService.DeleteItem(db, index, id);
            
            return Redirect($"/Index/Documents/{db}/{index}");
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDocViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _indexService.Initialize(UserHelper.CurrentUser);
                var id = await _indexService.Create(viewModel.DbName, viewModel.Index, viewModel.Value);

                return Redirect($"/Index/Documents/{viewModel.DbName}/{viewModel.Index}");
            }

            return View(viewModel);
        }
    }
}