using Microsoft.AspNetCore.Identity;
using raupjc_obg.Models.GameContentModels;
using System.Collections.Generic;

namespace raupjc_obg.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string InGameName { get; set; }
        public bool Admin { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public List<GameModel> FavoriteGames { get; set; }
    }
}