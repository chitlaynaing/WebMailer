using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using System;
using WebMailerAPI.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace WebMailerAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("DevPolicy")]
    [ApiController]
    public class MailController : ControllerBase
    {
        readonly IConfiguration _configuration;

        public MailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<string>> SendMail(Mail mail)
        {

            int numberOfAttempts = Convert.ToInt32(_configuration["MaximumNumberOfRetries"]);

            string result = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    result = SendMailByMailGun(mail);

                    while (result != "OK" && numberOfAttempts > 3)
                    {
                        numberOfAttempts--;
                        result = SendMailByMailGun(mail);
                    }

                    while (result != "OK" && numberOfAttempts > 0)
                    {
                        numberOfAttempts--;
                        result = await SendMailBySendGrid(mail);
                    }

                    if (result == "OK")
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                catch (Exception)
                {
                    return Content("Ops Snap!!!");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        private string SendMailByMailGun(Mail mail)
        {
            try
            {
                RestClient client = new RestClient();
                client.BaseUrl = new Uri("https://api.mailgun.net/v3");

                client.Authenticator = new HttpBasicAuthenticator("api", _configuration["MailGunAPI"]);

                RestRequest request = new RestRequest();
                request.AddParameter("domain", _configuration["YOUR_DOMAIN_NAME"], ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", mail.From);
                request.AddParameter("to", mail.To);
                request.AddParameter("subject", mail.Subject);
                request.AddParameter("text", mail.Text);

                if (!string.IsNullOrEmpty(mail.CCs))
                {
                    request.AddParameter("cc", mail.CCs);
                }

                if (!string.IsNullOrEmpty(mail.BCCs))
                {
                    request.AddParameter("bcc", mail.BCCs);
                }

                request.Method = Method.POST;

                return client.Execute(request).StatusCode.ToString();
                
            }
            catch (Exception)
            {
                throw;
            }

        }

        private async Task<string> SendMailBySendGrid(Mail mail)
        {
            try
            {

                var apiKey = _configuration["SendGridAPI"];
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage();
                
                var from = new EmailAddress(mail.From, "Test SendGrid Sender");
                var to = new EmailAddress(mail.To, "Test SendGrid Recipient");
                var subject = mail.Subject;
                var plainTextContent = mail.Text;

                msg.SetFrom(from.ToString(), "");
                msg.AddTo(to.ToString(), "");
                msg.SetSubject(subject);
                msg.AddContent(MimeType.Text, plainTextContent);
                var htmlContent = "";

                Personalization envelope = new Personalization();


                if (!string.IsNullOrEmpty(mail.CCs))
                {
                    var cc_emails = new List<EmailAddress>();
                    List<string> CCs = new List<string>();
                    CCs.AddRange(mail.CCs.Split(","));

                    foreach (string cc in CCs)
                    {
                        cc_emails.Add(new EmailAddress(cc, ""));
                    }

                    msg.AddCcs(cc_emails, 0);
                    envelope.Ccs = cc_emails;
                }

                if (!string.IsNullOrEmpty(mail.BCCs))
                {
                    List<EmailAddress> bcc_emails = new List<EmailAddress>();
                    List<string> BCCs = new List<string>();
                    BCCs.AddRange(mail.BCCs.Split(","));

                    foreach (var bcc in BCCs)
                    {
                        bcc_emails.Add(new EmailAddress(bcc, ""));
                    }

                    msg.AddBccs(bcc_emails,0);
                    envelope.Bccs = bcc_emails;
                }
                
                msg.Personalizations.Add(envelope);

                msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                Response response = await client.SendEmailAsync(msg);

                if (response.StatusCode.ToString() == "Accepted")
                {
                    return "OK";
                }
                else
                {
                    return "Bad Request";
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}