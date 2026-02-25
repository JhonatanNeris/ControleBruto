using AutoMapper;
using ControleBruto.Data.Dtos;
using ControleBruto.Models;
using Microsoft.AspNetCore.Identity;

namespace ControleBruto.Services
{
    public class UserService
    {
        private IMapper _mapper;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private TokenService _tokenService;

        public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;

        }

        public async Task Register(CreateUserDto dto)
        {
            // (opcional mas recomendado) validar duplicidade antes
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                //return ServiceResult.Fail(new[] { "E-mail já cadastrado." });
                throw new Exception("E-mail já cadastrado.");

            User user = _mapper.Map<User>(dto);

            user.UserName = dto.Email;

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                // Opção 1
                //throw new ApplicationException("Falha ao cadastrar usuário");

                //Opção 2
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<TokenDto> Login(LoginUserDto dto)
        {
            //Estamos passando o email como username, pois é o que definimos no Register
            var result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

            if (!result.Succeeded)
                throw new Exception("E-mail ou senha inválidos.");

            //var user = _signInManager.UserManager.Users.FirstOrDefault(u => u.NormalizedEmail == dto.Email.ToUpper());
            var user = await _userManager.FindByEmailAsync(dto.Email);
            
            if (user == null)
                throw new Exception("Usuário não encontrado.");

            var token = _tokenService.GenerateToken(user);

            return token;
        }
    }
}
