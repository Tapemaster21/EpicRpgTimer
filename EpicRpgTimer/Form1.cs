using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Speech.Synthesis;
using System.Windows.Forms;

namespace EpicRpgTimer
{
    public partial class Form1 : Form
    {
        public class Timeline
        {
            public string Name;
            public TimeSpan Cur;
            public TimeSpan Max;

            public Timeline(string n)
            {
                Name = n;
            }
        }

        public List<Timeline> Lines;
        public List<System.Windows.Forms.Timer> timers;
        public System.Windows.Forms.Timer pbar;
        public System.Speech.Synthesis.SpeechSynthesizer ss;
        public List<TimeSpan> ThirtyFive;
        public List<TimeSpan> Default;

        public Form1()
        {
            InitializeComponent();

            ThirtyFive = new List<TimeSpan>();
            ThirtyFive.Add(new TimeSpan(0, 3, 15));
            ThirtyFive.Add(new TimeSpan(0, 9, 45));
            ThirtyFive.Add(new TimeSpan(0, 39, 0));
            ThirtyFive.Add(new TimeSpan(2, 0, 0));
            ThirtyFive.Add(new TimeSpan(2, 0, 0));
            ThirtyFive.Add(new TimeSpan(3, 0, 0));
            ThirtyFive.Add(new TimeSpan(3, 54, 0));
            ThirtyFive.Add(new TimeSpan(7, 48, 0));
            ThirtyFive.Add(new TimeSpan(7, 48, 0));

            Default = new List<TimeSpan>();
            Default.Add(new TimeSpan(0, 5, 0));
            Default.Add(new TimeSpan(0, 15, 0));
            Default.Add(new TimeSpan(1, 0, 0));
            Default.Add(new TimeSpan(2, 0, 0));
            Default.Add(new TimeSpan(2, 0, 0));
            Default.Add(new TimeSpan(3, 0, 0));
            Default.Add(new TimeSpan(6, 0, 0));
            Default.Add(new TimeSpan(12, 0, 0));
            Default.Add(new TimeSpan(12, 0, 0));
        }


        private void LoadTimes(bool cool)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Lines[i].Max = (cool ? ThirtyFive[i] : Default[i]);
            }

            progressBar1.Maximum = (cool? 38 : 60) + 4;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Lines = new List<Timeline>();

            Lines.Add(new Timeline("Axe"));
            Lines.Add(new Timeline("Training"));
            Lines.Add(new Timeline("Adventure"));
            Lines.Add(new Timeline("Duel"));
            Lines.Add(new Timeline("Guild"));
            Lines.Add(new Timeline("Lootbox"));
            Lines.Add(new Timeline("Quest"));
            Lines.Add(new Timeline("Arena"));
            Lines.Add(new Timeline("Miniboss"));

            LoadTimes(true);

            timers = new List<System.Windows.Forms.Timer>();

            pbar = new Timer();
            pbar.Interval = 1000;
            pbar.Tick += pbar_Tick;
            ToolTip tt = new ToolTip();
            tt.ShowAlways = true;
            tt.IsBalloon = true;
            tt.SetToolTip(progressBar1, "Timer bar for \"Hunt,\" click to activate.");


            for (int i = 0; i < Lines.Count; i++)
            {
                timers.Add(new System.Windows.Forms.Timer());
                timers[i].Interval = 1000;
                timers[i].Tick += timer_Tick;
                timers[i].Tag = i;

                TextBox lb = (TextBox)this.Controls.Find("label" + i, true).First();
                lb.Text = Lines[i].Name;

                TextBox tb = (TextBox)this.Controls.Find("textbox" + i, true).First();
                tb.Text = Lines[i].Max.ToString();
                tb.KeyUp += Tb_KeyUp;
                tb.ContextMenuStrip = contextMenuStrip1;

            }

            this.ActiveControl = progressBar1;
        }

        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyValue.ToString() == "13")
            {
                try
                {
                    int number = int.Parse(((TextBox)sender).Name.Last().ToString());
                    string t = ((TextBox)sender).Text;

                    int h = Math.Abs(int.Parse(t.Split(':')[0]));
                    int m = Math.Abs(int.Parse(t.Split(':')[1]));
                    int s = Math.Abs(int.Parse(t.Split(':')[2]));

                    Lines[number].Max = new TimeSpan(h, m, s);
                    ((TextBox)sender).BackColor = System.Drawing.SystemColors.Window;
                    if (cbPopup.Checked)
                    {
                        MessageBox.Show("Timer " + (number + 1).ToString() + " set to: " + Lines[number].Max.ToString());
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Errors are hard.\nFormat your time as:\n01:23:45");
                }
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            int timerNum = int.Parse(((Button)sender).Name.Last().ToString());
            TextBox tb = (TextBox)this.Controls.Find("textbox" + timerNum.ToString(), true).First();

            if (((Button)sender).Text == "Stop")
            {   // Stop or Restart
                timers[timerNum].Stop();

                ((Button)sender).Text = "Restart";
                tb.BackColor = System.Drawing.Color.MistyRose;
            }
            else
            {   // Go
                Lines[timerNum].Cur = Lines[timerNum].Max;

                timers[timerNum].Start();

                tb.Text = Lines[timerNum].Cur.ToString();
                ((Button)sender).Text = "Stop";
                tb.BackColor = System.Drawing.SystemColors.Window;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //get timer number from its tag
            int timerNum = int.Parse(((Timer)sender).Tag.ToString());
            TextBox tb = (TextBox)this.Controls.Find("textbox" + timerNum.ToString(), true).First();
            Button b = (Button)this.Controls.Find("button" + timerNum.ToString(), true).First();

            //counting
            Lines[timerNum].Cur = Lines[timerNum].Cur.Subtract(new TimeSpan(0, 0, 1));
            tb.Text = Lines[timerNum].Cur.ToString();

            if (Lines[timerNum].Cur.TotalSeconds <= 0)
            {
                timers[timerNum].Stop();

                tb.BackColor = System.Drawing.Color.PaleGreen;
                b.Text = "Go";

                if (cbSpeech.Checked)
                {
                    ss.Speak("rpg " + ((TextBox)this.Controls.Find("label" + timerNum.ToString(), true).First()).Text);
                }
                if (cbDing.Checked)
                {
                    SystemSounds.Beep.Play();
                }
            }
        }

        private void rb35_CheckedChanged(object sender, EventArgs e)
        {
            LoadTimes(rb35.Checked);

            progressBar1.Value = 0;
            pbar.Stop();

            for (int i = 0; i < Lines.Count; i++)
            {
                TextBox tb = (TextBox)this.Controls.Find("textbox" + i, true).First();
                tb.Text = Lines[i].Max.ToString();
                tb.BackColor = System.Drawing.SystemColors.Window;

                ((Button)this.Controls.Find("button" + i, true).First()).Text = "Go";

                timers[i].Stop();
            }
        }

        private void cbSpeech_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSpeech.Checked)
            {
                ss = new SpeechSynthesizer();
                ss.SetOutputToDefaultAudioDevice();
            }
            else
            {
                ss.Dispose();
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Timers for EPIC RPG Discord Bot\n" +
                "Made by [SpatC] Tape#4122" +
                "\n\nv1\n" +
                " - Creation\n" +
                " - Editable times\n" +
                "v1.1\n" +
                " - Added options panel\n" +
                " - Added changeable labels\n" +
                " - Added voice label reading\n" +
                " - Added cooldown presets\n" +
                "v1.2\n" +
                " - Added \"always on top\"\n" +
                "v1.3\n" +
                " - Added hunt progress bar\n" +
                "v1.3.1\n" +
                " - Now clears hunt bar when change cd preset\n" +
                " - Tacked on 2 seconds to hunt bar\n" +
                "v1.3.2\n" +
                " - Fixed a bug in the added seconds to the hunt bar\n" +
                " - Added right click remove custom on timer boxes\n" +
                "", "About");
        }

        private void cbAOT_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbAOT.Checked;
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {
            if (progressBar1.Value > 0)
            {
                progressBar1.Value = 0;
                pbar.Stop();
            }
            else
            {
                progressBar1.Value = 1;
                pbar.Start();
            }

        }
        
        private void pbar_Tick(object sender, EventArgs e)
        {
            if(progressBar1.Value >= progressBar1.Maximum)
            {
                progressBar1.Value = 0;
            }
            else
            {
                progressBar1.PerformStep();
            }
        }

        private void removeCustomTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // get the parent of the menuitem and menu, being the textbox, just to get it's number
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ContextMenuStrip strip = (ContextMenuStrip)item.Owner;
            int num = int.Parse(((TextBox)strip.SourceControl).Name.Last().ToString());

            // reset button text
            ((Button)this.Controls.Find("button" + num, true).First()).Text = "Go";
            
            timers[num].Stop();
            
            // set time
            Lines[num].Max = (rb35.Checked? ThirtyFive[num] : Default[num]);
            
            // reset textbox
            TextBox tb = (TextBox)this.Controls.Find("textbox" + num, true).First();
            tb.Text = Lines[num].Max.ToString();
            tb.BackColor = System.Drawing.SystemColors.Window;
        }
    }
}
