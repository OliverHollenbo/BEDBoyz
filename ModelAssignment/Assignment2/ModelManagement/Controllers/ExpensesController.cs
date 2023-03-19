using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ModelManagement.Data;
using ModelManagement.DTO_er;
using ModelManagement.Models;

namespace ModelManagement.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ExpensesController : ControllerBase
	{
		private readonly ApplicationDBContext _context;

		public ExpensesController(ApplicationDBContext context)
		{
			_context = context;
		}


		// POST: api/Expenses
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<ExpenseDTO>> PostExpense(ExpenseCreate expenseCreate)
		{
			var dbModel = _context.Models.Find(expenseCreate.ModelId);
			if (dbModel == null)
			{
				return NotFound("Model not found");
			}

			_context.Entry(dbModel)
				.Collection(m => m.Expenses)
				.Load();



			// add the expense to the database and save changes
			_context.Expenses.Add(expenseCreate.Adapt<Expense>());
			await _context.SaveChangesAsync();

			//await _hubContext.Clients.All.SendAsync("expenseadded", expenseCreate);

			var dbExpenses = await _context.Expenses.ToListAsync();

			return Accepted(dbExpenses.Adapt<List<ExpenseDTO>>());

		}
	}
}
