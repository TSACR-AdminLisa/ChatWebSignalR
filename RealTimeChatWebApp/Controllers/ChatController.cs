using RealTimeChatWebApp.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Configuration;

namespace RealTimeChatWebApp.Controllers
{
    //[RoutePrefix("Chat")]
    //[Route("{action = index}")]
    public class ChatController : Controller
    {
        // GET: Chat
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(UserChatModel paramUserInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    paramUserInfo._UserType = UserChatModel.RegisteredUserType.regularUser;
                    paramUserInfo._GroupNameChat = Guid.NewGuid().ToString();

                    EmailModel emailService = new EmailModel();

                    //creates custom URL to access HelpDesk Login for users in service desk
                    string urlWebDomain = ConfigurationManager.AppSettings["WebDomain"];
                    UrlHelper uhelper = new UrlHelper(ControllerContext.RequestContext);
                    string urlHelpDeskLogin = urlWebDomain + uhelper.Action("HelpDeskLogin", "Chat", new { groupName = paramUserInfo._GroupNameChat });

                    //sends the email to helpdesk group
                    await emailService.CreatesUserStartSessionNotificationEmail(paramUserInfo, urlHelpDeskLogin); 

                    return RedirectToAction("ChatRoom", paramUserInfo);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = String.Format("Se ha presentado un problema al tratar de ingresar al chat. \n {0}", ex.Message);
                return View();
            }
            
        }

        [HttpGet]
        public ActionResult ChatRoom(UserChatModel paramUserInfo)
        {

            if (paramUserInfo != null)
            {
                if ((paramUserInfo._UserFullName != null) && (paramUserInfo._GroupNameChat != null))
                {
                    return View(paramUserInfo);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
            
        }

        [HttpGet]
        public ActionResult HelpDeskLogin(string groupName)
        {
            if (groupName != null)
            {
                HelpDeskUserModel helpDeskInfo = new Models.HelpDeskUserModel()
                {
                    _GroupNameChat = groupName
                };

                return View(helpDeskInfo);

            }
            else
            {
                return HttpNotFound();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HelpDeskLogin(HelpDeskUserModel paramHelpDeskInfo)
        {
            UserChatModel userInfo = new Models.UserChatModel();

            try
            {
                if (ModelState.IsValid)
                {
                    userInfo = new UserChatModel()
                    {
                        _GroupNameChat = paramHelpDeskInfo._GroupNameChat,
                        _UserFullName = paramHelpDeskInfo._FullName,
                        _UserType = UserChatModel.RegisteredUserType.helpDeskUser,
                        _UserEmail = paramHelpDeskInfo._UserEmail
                    };

                    return RedirectToAction("ChatRoom", userInfo);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = String.Format("Se ha presentado un problema al tratar de iniciar sesión. \n {0}", ex.Message);
                return View();
            }

        }
    }
}