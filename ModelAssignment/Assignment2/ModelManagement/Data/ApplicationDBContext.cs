using Microsoft.EntityFrameworkCore;
using ModelManagement.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace ModelManagement.Data
{
	public class ApplicationDBContext : DbContext
	{
		public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options){ }

		public DbSet<Model> Models { get; set; } = default!;
		public DbSet<Expense> Expenses { get; set; } = default!;
		public DbSet<Job> Jobs { get; set; } = default!;
	}
}
