using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RegistrationAndLogin.Controllers
{
    public class UserController : Controller
    {

        //Registration Action
        [HttpGet]

        public ActionResult Registration()
        {
            return View();
        }
        // Registration Post Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode")]User user)
        {

            bool Status = false;
            string Message = "";
            //Model validation

            if (ModelState.IsValid)
            {
                #region//Email Already Exist
                var isExist = isEmailExist(user.EmailId);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email Already Exist");
                    return View(user);
                }
                #endregion

                #region  //Generate Activation code
                user.ActivationCode = Guid.NewGuid();
                #endregion


                #region  //Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion
                user.IsEmailVerified = false;


                #region  //Save data to database
                using (myDatabaseEntities1 dc = new myDatabaseEntities1())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();
                    //Send email to user
                    SendVerificationLinkEmail(user.EmailId, user.ActivationCode.ToString());
                    Message = "Registration Successfully Completed. Account activation link"+
                        "has been sent to you email address "+ user.EmailId;
                    Status = true;
                }
                #endregion

            }
            else
            {
                Message = "Invalid Request";
            }
             
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify Account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;
            using (myDatabaseEntities1 dc = new myDatabaseEntities1())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;

                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    status = true;
                }
                else
                {
                    ViewBag.Message = "Request Is Invalid";
                }
            }

            ViewBag.status = status;
                return View();
        }

        //Login
         [HttpGet]
         public ActionResult Login()
        {
            return View();
        }
        //Login Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login,string ReturnUrl)
        {
            string message = "";
            using (myDatabaseEntities1 dc = new myDatabaseEntities1())
            {
                var v = dc.Users.Where(a => a.EmailId ==  login.EmailId).FirstOrDefault();
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20;
                        var ticket = new FormsAuthenticationTicket(login.EmailId, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                    message = "You couldn't be logged in, Please check your login details";

                    }
                }
                else
                {
                    message = "You couldn't be logged in, Please check your login details";
                }
            }


            ViewBag.Message = message;
            return View();
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public bool isEmailExist(string emailId)
        {
            using (myDatabaseEntities1 dc = new myDatabaseEntities1())
            {
                var v = dc.Users.Where(a => a.EmailId == emailId).FirstOrDefault();
                return v != null;
            }
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationCCode, string EmailFor = "VerifyAccount")
        {
            //var scheme = Request.Url.Scheme;
            //var host = Request.Url.Host;
            //var port = Request.Url.Port;

            //string url = scheme + "://" + host

            var verifyUrl = "/User/"+EmailFor+"/" + activationCCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("******@gmail.com", "I am awesome");
            var toEmail = new MailAddress(emailId);
            var fromPassword = "******"; //email password

            string subject = "";
            string body = "";
            if (EmailFor == "VerifyAccount")
            {
                subject = "Your account has been successfully created!";

                body = "<br/><br/>We are excited to tell you that your account " +
                    " has been successfully created, Please click on the link below to verify" +
                    "<br/><br/><a href='" + link + "' >" + link + "</a>";
            }
            else if (EmailFor == "ResetPassword")
            {
                subject = "Reset Password!!";
                body = "Hi <br/> <br/> We got a request to reset your password Please click on" +
                    " the link to reeset your password" +
                    "<br/><br/><a href='" + link + "' >" + link + "</a>";

            }
           
            
                
            

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true

            })
                smtp.Send(message);
                
                
        }

        //Forget Password
        public ActionResult ForgetPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgetPassword(string EmailID)
        {
            //verify email Id

            //Generate Reset Password Link

            //Send email

            string message = "";
            bool status = false;

            using (myDatabaseEntities1 dc = new myDatabaseEntities1())
            {
                var account = dc.Users.Where(a => a.EmailId == EmailID).FirstOrDefault();

                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EmailId, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;

                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset Password link has been sent to your email address";
                }
                else
                {
                    message = "Error!!, Email or Account does not exist";
                }
            }

            ViewBag.Message = message;
                return View();
        }
    
        public ActionResult ResetPassword(string id)
        {
            //verfiy the resetPassword
            //find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }
            using (myDatabaseEntities1 dc = new myDatabaseEntities1())
            {
                var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }

           // return View();
        }
   
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";

            if (ModelState.IsValid)
            {
                using (myDatabaseEntities1 dc = new myDatabaseEntities1())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New Password has been updated successfully.";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}
