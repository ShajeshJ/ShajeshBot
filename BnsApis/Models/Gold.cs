using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Models
{
    public class Gold
    {
        public int Total { get; set; }

        public Gold (int amount)
        {
            Total = amount;
        }

        #region Gold Division Properties

        public int GoldPart
        {
            get
            {
                return Total / 10000;
            }
        }
        public int SilverPart
        {
            get
            {
                return (Total / 100) % 100;
            }
        }
        public int CopperPart
        {
            get
            {
                return Total % 100;
            }
        }

        #endregion
    }
}
