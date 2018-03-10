using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShagBot
{
    public class Program
    {
        //invite https://discordapp.com/oauth2/authorize?client_id=421836036101111818&scope=bot&permissions=1610087545
        [STAThread]
        public static void Main(string[] args)
        {
            ShagBot shagbot = null;
            string patchNotes = null;

            var getPatchNotes = MessageBox.Show("Include patch notes?", "Confirm", MessageBoxButtons.YesNo);

            if (getPatchNotes == DialogResult.Yes)
            {
                var ofd = new OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = false;
                
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ofd.FileName))
                    {
                        patchNotes = string.Join("\r\n", File.ReadAllLines(ofd.FileName));
                    }
                }
            }

            shagbot = new ShagBot(patchNotes);

            shagbot.StartAsync().GetAwaiter().GetResult();
        }
    }
}
