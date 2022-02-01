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
            _lsc.CurrentPlayer = player;

            
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

            //TODO: Use _lsc to add the spin to dbContext _lsc and save changes to the database, instead of the repository
            Spin spin = _lsc.Find<Spin>(id);
            _lsc.Spins.AddSpin(spin);

            return View("Spin", spinVM); //Sends the updated spin info to the Spin View
        }

        /***
         * ListSpins Action - Displays Spin data
         **/
         [HttpGet]
         public IActionResult LuckList()
        {
            //TODO: Pass the DbSet of Spins from the _lsc to the LuckyList View, instead of the repository spins
            return View(_repository.PlayerSpins);
        }

    }
}

