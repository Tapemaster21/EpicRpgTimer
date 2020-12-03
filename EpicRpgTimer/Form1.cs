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
        public System.Speech.Synthesis.SpeechSynthesizer ss;

        public Form1()
        {
            InitializeComponent();
        }

        private void CoolTimes()
        {
            // patreon $5 35% cooldown times
            Lines[0].Max = new TimeSpan(0, 3, 15);
            Lines[1].Max = new TimeSpan(0, 9, 45);
            Lines[2].Max = new TimeSpan(0, 39, 0);
            Lines[3].Max = new TimeSpan(2, 0, 0);
            Lines[4].Max = new TimeSpan(2, 0, 0);
            Lines[5].Max = new TimeSpan(3, 0, 0);
            Lines[6].Max = new TimeSpan(3, 54, 0);
            Lines[7].Max = new TimeSpan(7, 48, 0);
            Lines[8].Max = new TimeSpan(7, 48, 0);
        }

        private void DefaultTimes()
        {
            Lines[0].Max = new TimeSpan(0, 5, 0);
            Lines[1].Max = new TimeSpan(0, 15, 0);
            Lines[2].Max = new TimeSpan(1, 0, 0);
            Lines[3].Max = new TimeSpan(2, 0, 0);
            Lines[4].Max = new TimeSpan(2, 0, 0);
            Lines[5].Max = new TimeSpan(3, 0, 0);
            Lines[6].Max = new TimeSpan(6, 0, 0);
            Lines[7].Max = new TimeSpan(12, 0, 0);
            Lines[8].Max = new TimeSpan(12, 0, 0);
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
            CoolTimes();

            timers = new List<System.Windows.Forms.Timer>();

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
            }

            this.ActiveControl = button0;
        }

        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyValue.ToString() == "13")
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
                    if(cbPopup.Checked)
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

                if(cbSpeech.Checked)
                {
                    ss.Speak("rpg " + ((TextBox)this.Controls.Find("label" + timerNum.ToString(), true).First()).Text);
                }
                if(cbDing.Checked)
                {
                    SystemSounds.Beep.Play();
                }
            }
        }

        private void rb35_CheckedChanged(object sender, EventArgs e)
        {
            if (rb35.Checked)
            {
                CoolTimes();
            }
            else
            {
                DefaultTimes();
            }

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
            if(cbSpeech.Checked)
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
                "","About");
        }

        private void cbAOT_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbAOT.Checked;
        }
    }
}
