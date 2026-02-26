using ControleBruto.Models;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos
{
    public class UpdateAccountDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Type { get; set; }
        public long InitialBalanceCents { get; set; } = 0;
    }
}
