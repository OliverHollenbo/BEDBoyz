using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
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
	public class JobsController : ControllerBase
	{
		private readonly ApplicationDBContext _context;

		public JobsController(ApplicationDBContext context)
		{
			_context = context;
		}
		

		[HttpPost]
		public async Task<ActionResult<JobCreate>> PostJob(JobCreate jobCreate)
		{
			_context.Jobs.Add(jobCreate.Adapt<Job>());
			await _context.SaveChangesAsync();

			//var dbJobs = await _context.Jobs.ToListAsync();

			return Ok(jobCreate.Adapt<List<JobCreate>>());
		}

		// DELETE: api/Jobs/5
		[HttpDelete("{JobId}")]
		public async Task<IActionResult> DeleteJob(long id)
		{
			var job = await _context.Jobs.FindAsync(id);
			if (job == null)
			{
				return NotFound();
			}

			_context.Jobs.Remove(job);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		//Put Jobs med specifikke data
		[HttpPut("{JobId}")]
		public async Task<ActionResult<Job>> PutJob(int JobId, JobUpdate Update)
		{
			var dbJob = await _context.Jobs.FindAsync(JobId);
			if (dbJob == null)
			{
				return NotFound("Could not find Jobs");
			}


			var job = Update.Adapt(dbJob);
			_context.Jobs.Update(job);


			await _context.SaveChangesAsync();


			return Ok(dbJob);

		}

		[HttpPut("{JobId}/AddModel/{ModelId}")]
		public async Task<ActionResult<JobWithModel>> AddModelToJob(int JobId, long ModelId)
		{
			var dbJob = await _context.Jobs
				.Include(x => x.Models)
				.FirstOrDefaultAsync(x => x.JobId == JobId);
			if (dbJob == null)
			{
				return NotFound("Could not find Jobs");
			}

			var dbModel = await _context.Models
				.Include(x => x.Jobs)
				.FirstOrDefaultAsync(x => x.ModelId == ModelId);
			if (dbModel == null)
			{
				return NotFound("Could not find Model");
			}

			if (dbJob.Models.Contains(dbModel)) { return Conflict("Model already on Job"); }

			dbJob.Models ??= new List<Model>();
			dbJob.Models.Add(dbModel);
			await _context.SaveChangesAsync();


			return Accepted(dbModel.Adapt<JobWithModel>());
		}

		// GET: api/Jobs
		[HttpGet]
		public async Task<ActionResult<JobWithModelNames>> GetJobWithModelNames()
		{
			List<JobWithModelNames> allJobsWithModelNames = new List<JobWithModelNames>();

			foreach (var job in _context.Jobs)
			{
				// explicit loading of models for each job
				await _context.Entry(job).Collection(j => j.Models).LoadAsync();

				var jobWithMN = job.Adapt<JobWithModelNames>();
				//var aJobWithModelNames = job.Adapt<JobWithModelNames>();
				jobWithMN.ModelNames = new List<string>();
				foreach (var model in job.Models)
				{
					jobWithMN.ModelNames.Add($"{model.FirstName} {model.LastName}");
				}

				allJobsWithModelNames.Add(jobWithMN);
			}

			return Ok(allJobsWithModelNames);
		}



		[HttpGet("WithModel/{ModelId}")]
		public async Task<ActionResult<List<JobWithModel>>> GetJobsWithModels(long ModelId)
		{
			// get all jobs from db
			var dbJobs = await _context.Jobs
				.Include(x => x.Models
					.Where(Models => Models.ModelId == ModelId))
				.ToListAsync();

			return Ok(dbJobs.Adapt<List<JobWithModel>>());
		}


		[HttpGet("WithExpenses/{JobId}")]
		public async Task<ActionResult<List<JobWithExpenses>>> GetJobsWithExpenses(int JobId)
		{
			var dbJobs = await _context.Jobs
				.Include(j => j.Expenses)
				.Include(j => j.Models)
				.FirstOrDefaultAsync(x => x.JobId == JobId);

			var jobWithExpenses = dbJobs.Adapt<JobWithExpenses>();
			jobWithExpenses.Expenses = new List<Expense>();

			if (dbJobs == null)
			{
				return NotFound("Could not find job with id" + JobId);
			}


			// add all expenses related to the job to the jobWithExpenses object
			foreach (var expense in dbJobs.Expenses)
			{ 
				 jobWithExpenses.Expenses.Add(expense.Adapt<Expense>());

			}

			return Ok(jobWithExpenses);
		}
	}
}
