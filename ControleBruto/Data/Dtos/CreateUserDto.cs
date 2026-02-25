using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "As senhas precisam ser iguais")]
        public string ConfirmPassword { get; set; }
    }
}
