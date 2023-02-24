using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    class Hero
    {
        private string icon = "";
        private int win = 0;
        private int lose = 0;
        public int IdHero { get; set; }
        public int Win
        {
            get { return win; }
            set { win = value; }
        }
        public int Lose
        {
            get { return lose; }
            set { lose = value; }
        }
        public int CountMatches
        {
            get { return win + lose; }
        }
        public int PercentWin
        {
            get { return (100 / CountMatches * win); }
        }
        public string Icon
        {
            get
            {
                if (icon == "")
                {
                    return MatchStats.GetImageHero(IdHero);
                }
                else
                {
                    return icon;
                }
            }
        }
    }
}
