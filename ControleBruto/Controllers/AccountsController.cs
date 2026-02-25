using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]

public class AccountsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Accounts endpoint");
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok($"Account with id {id}");
    }
    [HttpPost]
    public IActionResult Create()
    {
        return Ok("Account created");
    }
    [HttpPut]
    [Route("{id}")]
    public IActionResult Update(int id)
    {
        return Ok($"Account with id {id} updated");
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok($"Account with id {id} deleted");
    }
}
