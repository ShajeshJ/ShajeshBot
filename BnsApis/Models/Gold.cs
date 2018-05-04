using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BnsApis.Models
{
    public class Gold : IComparable<Gold>
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
                return Math.Abs((Total / 100) % 100);
            }
        }
        public int CopperPart
        {
            get
            {
                return Math.Abs(Total % 100);
            }
        }

        #endregion
        
        public static Gold operator *(Gold g1, int amt)
        {
            return new Gold(g1.Total * amt);
        }

        public static Gold operator +(Gold g1, Gold g2)
        {
            return new Gold(g1.Total + g2.Total);
        }

        public static Gold operator -(Gold g1, Gold g2)
        {
            return new Gold(g1.Total - g2.Total);
        }

        public static bool operator ==(Gold g1, int amt)
        {
            return g1.Total == amt;
        }

        public static bool operator !=(Gold g1, int amt)
        {
            return g1.Total != amt;
        }

        public int CompareTo(Gold other)
        {
            return Total - other.Total;
        }
    }
}
