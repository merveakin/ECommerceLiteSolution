using ECommerceLiteBLL.Account;
using ECommerceLiteBLL.Repository;
using ECommerceLiteBLL.Settings;
using ECommerceLiteEntity.Enums;
using ECommerceLiteEntity.IdentityModels;
using ECommerceLiteEntity.Models;
using ECommerceLiteEntity.ViewModels;
using ECommerceLiteUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ECommerceLiteUI.Controllers
{
    public class AccountController : BaseController
    {
        //GLOBAL ALAN 
        CustomerRepo myCustomerRepo = new CustomerRepo();

        PassiveUserRepo myPassiveUserRepo = new PassiveUserRepo();

        UserManager<ApplicationUser> myUserManager = MembershipTools.NewUserManager();
        UserStore<ApplicationUser> myUserStore = MembershipTools.NewUserStore();

        RoleManager<ApplicationRole> myRoleManager = MembershipTools.NewRoleManager();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var checkUserTC = myUserStore.Context.Set<Customer>()
 .FirstOrDefault(x => x.TCNumber == model.TCNumber)?.TCNumber;
                if (checkUserTC != null)
                {
                    ModelState.AddModelError("", "This TC number has already been registered in the system!");
                    return View(model);
                }

                var checkUserEmail = myUserStore.Context.Set<ApplicationUser>()
 .FirstOrDefault(x => x.Email == model.Email)?.Email;
                if (checkUserEmail != null)
                {
                    ModelState.AddModelError("", "This email address is already registered in the system. If you forgot your password, you can get a new password with I forgot my password!");
                    return View(model);
                }

                var theActivationCode = Guid.NewGuid().ToString().Replace("-", "");
                var newUser = new ApplicationUser()
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    ActivationCode = theActivationCode,
                    UserName = model.Email,
                    //TODO : Yarın düzenlenecek >> 20.01.2022
                    PhoneNumber = "05392609129"
                };
                var theResult = myUserManager.CreateAsync(newUser, model.Password);

                if (theResult.Result.Succeeded)
                {
                    //AspnetUsers tablosuna kayıt gerçekleşirse yeni kayıt olmuş kişiyi pasif tablosuna ekleyeceğiz.
                    //Kişi kendisine gelen aktifleşme işlemini yaparsa PasifKullanıcılar tablosundan kendisini silip olması gereken roldeki tabloya ekleyeceğiz.
                    await myUserManager.AddToRoleAsync(newUser.Id, TheIdentityRoles.Passive.ToString());
                    PassiveUser newPassiveUser = new PassiveUser()
                    {
                        TCNumber = model.TCNumber,
                        UserId = newUser.Id,
                        TargetRole = TheIdentityRoles.Customer
                    };
                    myPassiveUserRepo.Insert(newPassiveUser);
                    string siteUrl =
                        Request.Url.Scheme
                        + Uri.SchemeDelimiter
                        + Request.Url.Host
                        + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    await SiteSettings.SendMail(new MailModel()
                    {
                        To = newUser.Email,
                        Subject = "ECommerceLite Site Activation",
                        Message = $"Hi {newUser.Name} {newUser.Surname}, <br/>Click <b><a href='{siteUrl}/Account/Activation?code={theActivationCode}'>Activation Link</a></b> to activate your account..."
                    });
                    return RedirectToAction("Login", "Account"
                        , new { email = $"{newUser.Email}" });

                }
                else
                {
                    ModelState.AddModelError("", "Error occurred during user registration!");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                //TODO : Ex_Loglama
                ModelState.AddModelError("", "Unexpected error occurred");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Activation(string code)
        {
            try
            {
                var theUser = myUserStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.ActivationCode == code);
                if (theUser == null)
                {
                    ViewBag.ActivationResult = "Activation failed";
                    return View();
                }

                if (theUser.EmailConfirmed)
                {
                    ViewBag.ActivationResult = "Your e-mail address has already been confirmed.";
                    return View();
                }
                theUser.EmailConfirmed = true;
                await myUserStore.UpdateAsync(theUser);
                await myUserStore.Context.SaveChangesAsync();
                //Kullanıcıyı passiveuser tablosundan bulalım
                PassiveUser thePassiveUser = myPassiveUserRepo.Queryable().FirstOrDefault(x => x.UserId == theUser.Id);
                if (thePassiveUser != null)
                {
                    if (thePassiveUser.TargetRole == TheIdentityRoles.Customer)
                    {
                        //Yeni customer oluşacak ve kaydedilecek.
                        Customer newCustomer = new Customer()
                        {
                            TCNumber = thePassiveUser.TCNumber,
                            UserId = theUser.Id
                        };
                        myCustomerRepo.Insert(newCustomer);
                        //Passive Tablosundan bu kayıt silinsin.
                        myPassiveUserRepo.Delete(thePassiveUser);

                        //User'daki passive rol silinip customer rol eklenecek.
                        myUserManager.RemoveFromRole(theUser.Id, TheIdentityRoles.Passive.ToString());
                        myUserManager.AddToRole(theUser.Id, TheIdentityRoles.Customer.ToString());
                        ViewBag.ActivationResult = $"Hi {theUser.Name} {theUser.Surname},your activation is successful. ";
                        return View();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                //TODO: ex Loglama
                ModelState.AddModelError("", "An unexpected error has occurred!");
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl, string email)
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    var url = ReturnUrl.Split('/');
                    //TODO : devam edebilir.
                }
                var model = new LoginViewModel()
                {
                    ReturnUrl = ReturnUrl
                };
                return View(model);
            }
            catch (Exception ex)
            {

                //ex loglanacak
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var theUser = await myUserManager.FindAsync(model.Email, model.Password);
                if (theUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Make sure you enter your email or password correctly!");
                    return View(model);
                }
                if (theUser.Roles.FirstOrDefault().RoleId == myRoleManager.FindByName(Enum.GetName(typeof(TheIdentityRoles), TheIdentityRoles.Passive)).Id)
                {
                    ViewBag.TheResult = "In order to use the system, you need to activate your membership. You can activate by clicking the activation link sent to your email!";
                    return View(model);
                }
                var authManager = HttpContext.GetOwinContext().Authentication;
                var userIdentity = await myUserManager.CreateIdentityAsync(theUser, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                }, userIdentity);
                if (theUser.Roles.FirstOrDefault().RoleId == myRoleManager.FindByName(Enum.GetName(typeof(TheIdentityRoles), TheIdentityRoles.Admin)).Id)
                {
                    return RedirectToAction("Index", "Admin");

                }
                if (theUser.Roles.FirstOrDefault().RoleId == myRoleManager.FindByName(Enum.GetName(typeof(TheIdentityRoles), TheIdentityRoles.Customer)).Id)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (string.IsNullOrEmpty(model.ReturnUrl))
                    return RedirectToAction("Index", "Home");

                var url = model.ReturnUrl.Split('/');
                if (url.Length == 4)
                {
                    return RedirectToAction(url[2], url[1], new { id = url[3] });
                }
                else
                {
                    return RedirectToAction(url[2], url[1]);
                }
            }
            catch (Exception ex)
            {
                //TODO ex loglanacak
                ModelState.AddModelError("", "Unexpected error occurred!");
                return View(model);

            }
        }

        [HttpGet]
        public ActionResult UpdatePassword()
        {

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(ProfileViewModel model)
        {
            try
            {
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match!");

                    //TODO :   Profile göndermişiz ?

                    return View(model);
                }

                var theUser = myUserManager
                    .FindById(HttpContext.User.Identity.GetUserId());

                var theCheckUser = myUserManager.Find(theUser.UserName, model.OldPassword);

                if (theCheckUser == null)
                {
                    ModelState.AddModelError("", "You entered your current password incorrectly!");
                    //TODO :   Profile göndermişiz ?
                    return View();
                }

                await myUserStore.SetPasswordHashAsync(theUser, myUserManager.PasswordHasher.HashPassword(model.NewPassword));
                await myUserStore.UpdateAsync(theUser);
                await myUserStore.Context.SaveChangesAsync();
                TempData["PasswordUpdated"] = "Your password has been changed.";
                HttpContext.GetOwinContext().Authentication.SignOut();
                return RedirectToAction("Login", "Account", new { email = theUser.Email });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error occurred.");
                return View(model);
                //TODO : ex loglanacak
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult UserProfile()
        {
            var theUser = myUserManager.FindById(HttpContext.User.Identity.GetUserId());
            var model = new ProfileViewModel()
            {
                Email = theUser.Email,
                Name = theUser.Name,
                Surname = theUser.Surname,
                Username = theUser.UserName
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(ProfileViewModel model)
        {
            try
            {
                var theUser = myUserManager.FindById(HttpContext.User.Identity.GetUserId());
                if (theUser == null)
                {
                    ModelState.AddModelError("", "Cannot process because the user is not found!");  //Kullanıcı bulunamadığı için işlem yapılamıyor.
                    return View(model);
                }

                theUser.Name = model.Name;
                theUser.Surname = model.Surname;
                //TODO: telefon numarası eklenebilir.

                await myUserStore.UpdateAsync(theUser);
                await myUserStore.Context.SaveChangesAsync();
                ViewBag.TheResult = "Your information has been updated.";
                var newModel = new ProfileViewModel()
                {
                    Email = theUser.Email,
                    Name = theUser.Name,
                    Surname = theUser.Surname,
                    Username = theUser.UserName
                };
                return View(newModel);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unexpected error occurred. ERROR : " + ex.Message);
                return View(model);

                //TODO : ex loglanacak
            }
        }

        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RecoverPassword(ProfileViewModel model)
        {
            try
            {
                var theUser = myUserStore.Context.Set<ApplicationUser>().FirstOrDefault(x => x.Email == model.Email);
                if (theUser == null)
                {
                    ViewBag.TheResult = "Since there is no such user in the system, we cannot renew the password. Please register in the system first!";    //Sistemde böyle bir kullanıcı olmadığı için şifre yenileyemiyoruz.Lütfen önce sisteme kayıt olunuz!
                    return View(model);
                }

                var randomPassword = CreateRandomNewPassword();
                await myUserStore.SetPasswordHashAsync(theUser, myUserManager.PasswordHasher.HashPassword(randomPassword));
                await myUserStore.UpdateAsync(theUser);
                string siteUrl = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
(Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                await SiteSettings.SendMail(new MailModel()
                {
                    To = theUser.Email,
                    Subject = "ECommerceLite Site-Password Changed.",
                    Message = $"Hi {theUser.Name} {theUser.Surname} <br/>New Password: <b>{randomPassword}</b>" +
                    $"Click <b><a href='{siteUrl}/Account/Login?email={theUser.UserName}'>HERE</a></b> to login to the system."
                });
                ViewBag.TheResult = "Your new password has been sent to your e-mail address.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.TheResult = "System error occurred! TRY AGAIN!";
                return View(model);
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
