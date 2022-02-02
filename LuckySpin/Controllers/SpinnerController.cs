using System;
using Microsoft.AspNetCore.Mvc;
using LuckySpin.Models;
using LuckySpin.ViewModels;

namespace LuckySpin.Controllers
{
    public class SpinnerController : Controller
    {

        private LuckySpinContext _lsc;
        Random random = new Random();

        /***
         * Controller Constructor
         */
        public SpinnerController(LuckySpinContext lsc) 
        {
            _lsc = lsc;
        }

        /***
         * Index Action - Gathers Player info
         **/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexViewModel info)
        {
            if (!ModelState.IsValid) { return View(); }

            //Create a new Player object
            Player player = new Player
            {
                FirstName = info.FirstName,
                Luck = info.Luck,
                Balance = info.StartingBalance
            };
            _lsc.Player.Add(player);
            _lsc.SaveChanges();


            return RedirectToAction("Spin", new {id = player.PlayerId});
        }

        /***
         * Spin Action - Plays one Spin
         **/  
         [HttpGet]      
         public IActionResult Spin(long id)
        {

            Player player = _lsc.Find<Player>(id);
            

            SpinViewModel spinVM = new SpinViewModel() {
                PlayerName = player.FirstName,
                Luck = player.Luck,
                CurrentBalance = player.Balance
            };

            if (!spinVM.ChargeSpin())
            {
                return RedirectToAction("LuckList");
            }
 
            if (spinVM.Winner) { spinVM.CollectWinnings(); }
            
            player.Balance = spinVM.CurrentBalance;

            //Creates a Spin using the logic from the SpinViewModel
            Spin spin = new Spin() {
                IsWinning = spinVM.Winner
            };

            _lsc.Add(spin);
            _lsc.SaveChanges();

            return View("Spin", spinVM); //Sends the updated spin info to the Spin View
        }

        /***
         * ListSpins Action - Displays Spin data
         **/
         [HttpGet]
         public IActionResult LuckList()
        {
            return View(_lsc.Spins);
        }

    }
}

