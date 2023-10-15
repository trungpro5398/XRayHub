using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using XRayHub.Models;

namespace XRayHub.Controllers
{
    public class EmailController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> SendBulkEmails(EmailViewModel emailData)
        {
            Debug.WriteLine(emailData.Emails);

            // Ensure email data is valid
            if (emailData?.Emails != null && emailData.Emails.Count > 0 &&
                !string.IsNullOrEmpty(emailData.Subject) &&
                !string.IsNullOrEmpty(emailData.Message))
            {
                // Retrieve API Key securely
                string sendGridApiKey = "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";

                if (string.IsNullOrEmpty(sendGridApiKey))
                {
                    return Json(new { success = false, message = "SendGrid API key is missing or invalid." });
                }

                var client = new SendGridClient(sendGridApiKey);

                // Sending emails
                foreach (var email in emailData.Emails)
                {
                    Debug.WriteLine(email);
                    var from = new EmailAddress("trungprogrammarly@gmail.com", "Your Name");
                    var to = new EmailAddress(email);
                    var msg = MailHelper.CreateSingleEmail(from, to, emailData.Subject, emailData.Message, emailData.Message);

                    try
                    {
                        var response = await client.SendEmailAsync(msg);

                        if (response.StatusCode != System.Net.HttpStatusCode.Accepted &&
                            response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            // Log failure for internal review
                            System.Diagnostics.Debug.WriteLine($"Failed to send email to {email}. Status Code: {response.StatusCode}. Body: {await response.Body.ReadAsStringAsync()}");
                            return Json(new { success = false, message = "Error sending email to " + email });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log exception for internal review
                        System.Diagnostics.Debug.WriteLine($"Exception occurred: {ex.Message}");
                        return Json(new { success = false, message = "Exception occurred while sending email to " + email });
                    }
                }
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Invalid email data" });
        }
    }
}
