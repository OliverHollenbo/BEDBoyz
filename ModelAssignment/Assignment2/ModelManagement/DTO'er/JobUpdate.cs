using System.ComponentModel.DataAnnotations;

namespace ModelManagement.DTO_er
{
	public class JobUpdate
	{
		
		public DateTime StartDate { get; set; }
		public string? Location { get; set; }
		[MaxLength(2000)]
		public string? Comments { get; set; }
	}
}
