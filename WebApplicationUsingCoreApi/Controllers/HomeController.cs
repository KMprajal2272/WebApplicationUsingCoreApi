using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationUsingCoreApi.Models;

namespace WebApplicationUsingCoreApi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        Uri api = new Uri("http://localhost:29293/api/Employee/");
        HttpClient client;
        public HomeController()
        {
            client = new HttpClient();
            client.BaseAddress = api;
        }
        [HttpGet]
        public IActionResult Show()
        {
            List<Regstration> list = new List<Regstration>();
            var listemp = client.GetAsync(client.BaseAddress + "ShowDetails").Result;
            if (listemp.IsSuccessStatusCode)
            {
                string data = listemp.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<List<Regstration>>(data);
                foreach (var item in res)
                {
                    list.Add(new Regstration
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Email = item.Email,
                        Password = item.Password,
                        MobileNumber = item.MobileNumber,
                        Address = item.Address
                    });
                }

            }
            return View(list);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(Regstration emp)
        {
            string data = JsonConvert.SerializeObject(emp);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");
            HttpResponseMessage res = client.PostAsync(client.BaseAddress + "adddetails", content).Result;
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction("Show");
            }
            return RedirectToAction("Show");
        }
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var res = client.DeleteAsync(client.BaseAddress + "DeleteDetails" + '?' + "Id" + '=' + Id.ToString()).Result;
            return RedirectToAction("Show");
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var res = client.GetAsync(client.BaseAddress + "EditDetails" + '?' + "Id" + '=' + Id.ToString()).Result;
            string data = res.Content.ReadAsStringAsync().Result;
            var v = JsonConvert.DeserializeObject<Regstration>(data);
            return View("Add", v);
        }




        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(UserLiogin emp)
        {
            string data = JsonConvert.SerializeObject(emp);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");
            HttpResponseMessage htt = client.PostAsync(client.BaseAddress + "LoginPage", content).Result;
            string data1 = htt.Content.ReadAsStringAsync().Result;
            var res = JsonConvert.DeserializeObject<UserLiogin>(data1);
            if (res.Email == "Email Not Found")
            {
                TempData["Email"] = "Email Not Found";
            }
            else
            {
                if (res.Email == emp.Email && res.Password == emp.Password)
                {

                    HttpContext.Session.SetString("Name", res.Name);
                    ViewBag.NameSession = HttpContext.Session.GetString("Name");
                    HttpContext.Session.SetString("Email", res.Email);
                    ViewBag.EmailSession = HttpContext.Session.GetString("Email");
                    var claims = new[] { new Claim(ClaimTypes.Name, res.Name),
                                         new Claim(ClaimTypes.Email, res.Email) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
                    return RedirectToAction("Show");
                }
                else
                {
                    TempData["Wrong"] = "Invalid User Wrong PassWord";
                }
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Sinup()
        {
           
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Sinup(UserLiogin emp)
        {
            string data = JsonConvert.SerializeObject(emp);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/Json");
            HttpResponseMessage res = client.PostAsync(client.BaseAddress + "SignupUser", content).Result;
            if (res.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
