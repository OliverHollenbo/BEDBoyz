﻿using ModelManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace ModelManagement.DTO_er
{
	public class JobWithModelNames
	{

		public long JobId { get; set; }
		[MaxLength(64)]
		public string? Customer { get; set; }
		public DateTimeOffset StartDate { get; set; }
		public int Days { get; set; }
		[MaxLength(128)]
		public string? Location { get; set; }
		[MaxLength(2000)]
		public string? Comments { get; set; }
		public List<string>? ModelNames { get; set; }

	}
}
