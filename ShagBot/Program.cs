using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShagBot
{
    public class Program
    {
        //invite https://discordapp.com/oauth2/authorize?client_id=383487792308813825&scope=bot&permissions=1610087545
        public static void Main(string[] args)
        {
            var shagbot = new ShagBot();

            shagbot.StartAsync().GetAwaiter().GetResult();
        }
    }
}
