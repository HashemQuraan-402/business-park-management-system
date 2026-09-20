using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessParkCompaniesManagement.Server.Services;
using BusinessParkCompaniesManagement.Server.Models.Entities;
using BusinessParkCompaniesManagement.Server.Models.Dtos;
using BusinessParkCompaniesManagement.Server.Data;
using BusinessParkCompaniesManagement.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessParkCompaniesManagement.Server.Controllers
{
    // https://localhost:xxxx/api/Auth
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext db;
        private readonly IPasswordService passwordService;
        private readonly ITokenService tokenService;

        public AuthController(AppDbContext db, IPasswordService passwordService, ITokenService tokenService)
        {
            this.db = db;
            this.passwordService = passwordService;
            this.tokenService = tokenService;
        }




        // POST api/Auth/login/admin
        [HttpPost("login/admin")]
        public async Task<ActionResult<LoginResponseDto>> AdminLogin(LoginRequestDto request)
        {
            var admin = await db.AdminUsers.FirstOrDefaultAsync(a =>
                                                                a.Username == request.Username);

            if (admin == null || !passwordService.VerifyPassword(request.Password, admin.PasswordHash, admin.PasswordSalt))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }

            var (token, expiresAt) = tokenService.GenerateToken(admin.Id.ToString(), admin.Username, "Admin");

            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Role = "Admin",
                Displayname = admin.FullName,

            });
        }


        // POST api/Auth/login/company
        [HttpPost("login/company")]
        public async Task<ActionResult<LoginResponseDto>> CompanyLogin(LoginRequestDto request)
        {
            var company = await db.Companies.FirstOrDefaultAsync(c =>
                                                                c.Username == request.Username);
            if (company == null || !passwordService.VerifyPassword(request.Password, company.PasswordHash!, company.PasswordSalt!))
            {
                return Unauthorized(new { message = "Invalid username or password." });
            }
            var (token, expiresAt) = tokenService.GenerateToken(company.Id.ToString(), company.Username!, "Company", company.Id);
            return Ok(new LoginResponseDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Role = "Company",
                Displayname = company.NameEn,
                CompanyId = company.Id
            });
        }
    }
}
