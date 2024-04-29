using EduSchool.Models.Email;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    private readonly IEmailSender _emailSender;

    public ErrorController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    [HttpPost]
    public async Task<IActionResult> SendErrorEmail()
    {
        try
        {
            var errorDetails = new
            {
                ErrorTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            var subject = "Hibajegy - EduSchool";
            var htmlMessage = $@"
                <p>Hiba történt az EduSchool alkalmazásban.</p>
                <p>Hiba időpontja: {errorDetails.ErrorTime}</p>
            ";

            await _emailSender.SendEmailAsync("kristof@kopajler.hu", subject, htmlMessage);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Hiba történt az e-mail küldése során.");
        }
    }

    public IActionResult NotFound()
    {
        return View();
    }

    public IActionResult GeneralError()
    {
        return View();
    }
}