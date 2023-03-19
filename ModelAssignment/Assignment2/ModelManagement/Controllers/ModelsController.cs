using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelManagement.Data;
using ModelManagement.DTO_er;
using ModelManagement.Models;

namespace ModelManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ModelsController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<ActionResult<AllModels>> PostModel(AllModels model)
        {
			var models = model.Adapt<Model>();

			// add the model to the database and save changes
			_context.Models.Add(models);
			await _context.SaveChangesAsync();

			// return the model as a ModelBaseData
			return Ok(model.Adapt<AllModels>());

		}

        [HttpDelete("{ModelId}")]
        public async Task<ActionResult<Model>> DeleteModel(long ModelId)
        {
	        // get the model from the database
	        var dbModel = await _context.Models.FindAsync(ModelId);
	        if (dbModel == null)
	        {
		        return NotFound("Model not found");
	        }

	        // delete the model from the database
	        _context.Models.Remove(dbModel);


	        _context.Entry(dbModel).State = EntityState.Deleted;

	        // save changes
	        await _context.SaveChangesAsync();

	        // return the updated list of models
	        return Ok(await _context.Models.ToListAsync());
        }

        [HttpPut("BaseData/{ModelId}")]
        public async Task<ActionResult<AllModels>> PutModel(long ModelId, ChangeModelBaseData modelBaseData)
        {
	        // get the model from the database
	        var dbModel = await _context.Models.FindAsync(ModelId);
	        if (dbModel == null) { return NotFound("Could not find Model"); }

	        // update the model in the database using mapster adapt
	        var model = modelBaseData.Adapt(dbModel);
	        _context.Models.Update(model);

	        // save the changes
	        await _context.SaveChangesAsync();

	        // return the updated modelBaseData using mapster adapt
	        return Ok(model.Adapt<AllModels>());

        }

        // GET ModelBasedata
        [HttpGet("BaseData")]
        public async Task<ActionResult<List<AllModels>>> GetModels()
        {
			var dbModel = await _context.Models.ToListAsync();
			if (dbModel == null) { return BadRequest("Could not find any models"); }

			foreach (var model in dbModel)
			{
				await _context.Entry(model)
					.Collection(m => m.Jobs)
					.LoadAsync();
			}
			return Ok(dbModel.Adapt<List<AllModels>>());
        }
        // GET {model.id} Model
        [HttpGet("{ModelId}")]
        public async Task<ActionResult<Model>> GetModel(long ModelId)
        {
	        // get the model from the database
	        var dbModel = _context.Models
		        .Include(m => m.Jobs)
		        .Include(m => m.Expenses)
		        .FirstOrDefault(x => x.ModelId == ModelId)!;

	        if (dbModel == null) { return NotFound("Could not find Model"); }

	        _context.Entry(dbModel);

	        _context.Entry(dbModel);

	        return Ok(dbModel);
        }

        private bool ModelExists(long id)
        {
	        return _context.Models.Any(e => e.ModelId == id);
        }
    }
}

