using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BrightIdeas.Factory;
using BrightIdeas.Models;
using Dapper;



namespace BrightIdeas.Controllers
{
    public class IdeasController : Controller
    {
        private readonly UserFactory userFactory;
        private readonly PostFactory postFactory;
        private readonly LikeFactory likeFactory;
        public IdeasController(UserFactory uf, PostFactory pf, LikeFactory lkf){
            userFactory = uf;
            postFactory = pf;
            likeFactory = lkf;
        }
       
       
        // GET: /Home/
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return RedirectToAction("Main");
        }

        [HttpGet]
        [Route("/main")]
        public IActionResult Main()
        {
            return View();
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User newUser)
        {   
            ViewBag.EmailError = null;
            if(ModelState.IsValid)
            {
                var checkuser = userFactory.GetUserByEmail(newUser.email);
                if(checkuser != null){
                    ViewBag.EmailErrors = "Address is already in use.";
                    return View("Main");
                }
                else{
                    var Hasher = new PasswordHasher<User>();
                    newUser.password = Hasher.HashPassword(newUser, newUser.password);
                    userFactory.Add(newUser);
                    User CurrentUser = userFactory.GetLatestUser();
                    HttpContext.Session.SetInt32("CurrUserId", CurrentUser.id);
                    return RedirectToAction("Ideas");
                }
            }

            ViewBag.Errors = ModelState.Values;
            return View("Main");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(string email, string password)
        {
            ViewBag.LogErrors = null;
            if(email != null){
                if(password != null){
                    User checkUser = userFactory.GetUserByEmail(email);
                    if (checkUser != null){
                        var Hasher = new PasswordHasher<User>();
                        if(0 != Hasher.VerifyHashedPassword(checkUser, checkUser.password, password)){
                            HttpContext.Session.SetInt32("CurrUserId", checkUser.id);
                            return RedirectToAction("Ideas");    
                        }
                        else{
                            ViewBag.LogErrors = "Username and/or password are incorrect. Try again.";
                            return View("Main");
                        }
                    }
                }
                else{
                    ViewBag.LogErrors = "Please enter a password";
                    return  View("Main");
                }
                ViewBag.LogErrors = "Username not found.  Please register.";
                return  View("Main");
            }
            else{
                ViewBag.LogErrors = "Enter an email address.";
                return View("Main");
            }
        }

        [HttpGet]
        [Route("/bright_ideas")]
        public IActionResult Ideas()
        {
            ViewBag.User = userFactory.GetUserById((int) HttpContext.Session.GetInt32("CurrUserId"));
            ViewBag.Posts = postFactory.GetAllPosts();
            if(TempData["Errors"] != null){
                ViewBag.Errors = TempData["Errors"];
            }
            return View();
        }

        [HttpPost]
        [Route("/post")]
        public IActionResult Post(Post newPost)
        {

            if(ModelState.IsValid){
                newPost.user_id = (int)HttpContext.Session.GetInt32("CurrUserId");
                postFactory.Add(newPost);
                return RedirectToAction("Ideas");
            }
            ViewBag.User = userFactory.GetUserById((int) HttpContext.Session.GetInt32("CurrUserId"));
            ViewBag.Posts = postFactory.GetAllPosts();
            TempData["Errors"] = "Post's content cannot be empty";
           

            return RedirectToAction("Ideas");     
        }

        [HttpGet]
        [Route("/bright_ideas/{post_id}")]
        public IActionResult Idea(int post_id)
        {
            if(HttpContext.Session.GetInt32("CurrUserId") != null)
            {
                ViewBag.Post = postFactory.GetPostById(post_id);
                ViewBag.Users = userFactory.GetUserByPost(post_id);
                return View("Idea");
            }
            else{
                return RedirectToAction("Default");
            }
        }

        [HttpGet]
        [Route("/like/{post_id}")]
        public IActionResult Like(int post_id){
            if(HttpContext.Session.GetInt32("CurrUserId") == null){
                return RedirectToAction("Default");
            }
            int user_id = (int) HttpContext.Session.GetInt32("CurrUserId");
            likeFactory.Add(user_id, post_id);
            return RedirectToAction("Ideas");
        }

        [HttpGet]
        [Route("/users/{user_id}")]
        public IActionResult GetUser(int user_id)
        {
            if(HttpContext.Session.GetInt32("CurrUserId") != null)
            {
                ViewBag.User = userFactory.GetUserById(user_id);
                ViewBag.numPosts = postFactory.GetPostsByUser(user_id).Count;
                ViewBag.numLikes = likeFactory.GetLikesByUser(user_id).Count;
                return View("User");  
            }
            else{
                return RedirectToAction("Default");
            }
        }

        [HttpPost]
        [Route("/delete")]
        public IActionResult Delete(int post_id){
            userFactory.Delete(post_id);
            return RedirectToAction("Ideas");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Main");
        }
    }
}
