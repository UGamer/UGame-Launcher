﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.OleDb;
using CefSharp;
using CefSharp.WinForms;

namespace The_UGamer_Launcher
{
    public partial class GameDetails : Form
    {
        private Stopwatch gameTime = new Stopwatch();
        private ChromiumWebBrowser chromeBrowser;
        string newsUrl;
        string wikiUrl;

        // Starts up a detail form.
        public GameDetails() 
        {
            InitializeComponent();
            if (noImageText.Visible == true)
            {
                gamePicture.Visible = false;
            }
        }
        
        // Displays all the info for the game.
        public void DisplayInfo(string title, string input2, string platform,
            string status, string rating, string hours, string obtained,
            string startDate, string endDate, string notes, string launchString2, bool exePath2, bool batPath2,
            string newsString2, string wikiString2)
        { 
            noImageText.Visible = false;
            
            // This block of text determines the icon.
            try
            {
                Icon windowIcon = new Icon("Resources/Icons/" + input2 + ".ico");
                this.Icon = windowIcon;
            }
            catch (FileNotFoundException e) { }

            Image detailedPic = detailedImageAssign(input2);
            Image backgroundPic = backgroundImageAssign(input2, detailedPic);

            if (detailedPic.Width != 1)
            {
                gamePicture.BackgroundImage = detailedPic;
            }
            else
            {
                try
                {
                    if (backgroundPic.Width != 1)
                    {
                        gamePicture.BackgroundImage = backgroundPic;
                        noImageText.Visible = false;
                    }
                }
                catch (FileNotFoundException e) { }
            }

            if (backgroundPic.Width != 1)
            {
                try
                {
                    Bitmap bg = new Bitmap(backgroundPic);
                    var radius = 20;
                    StackBlur.StackBlur.Process(bg, radius);
                    this.BackgroundImage = bg;
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        Bitmap bg = new Bitmap(detailedPic);
                        var radius = 20;
                        StackBlur.StackBlur.Process(bg, radius);
                        this.BackgroundImage = bg;
                    }
                    catch (FileNotFoundException g) { }
                }
            }

            nameLabel.Text = title; // Displays the name of the game.
            platformLabel.Text = "Platform: " + platform;

            if (status == "")
                statusLabel.Text = "Status:  ";
            else
                statusLabel.Text = "Status: " + status;

            if (rating == "")
                ratingLabel.Text = "Rating:  ";
            else
                ratingLabel.Text = "Rating: " + rating;

            if (hours == "")
                hoursLabel.Text = "Time Played:  ";
            else
                hoursLabel.Text = "Time Played: " + hours;

            if (obtained == "")
                obtainedLabel.Text = "Obtained:  ";
            else
                obtainedLabel.Text = "Obtained: " + obtained;

            if (startDate == "")
                startDateLabel.Text = "Start Date:  ";
            else
                startDateLabel.Text = "Start Date: " + startDate;

            if (endDate == "")
                endDateLabel.Text = "End Date:  ";
            else
                endDateLabel.Text = "End Date: " + endDate;

            if (notes == "")
                notesBox.Text = " ";
            else
                notesBox.Text = notes;

            if (launchString2 != "")
                launchLabel.Text = launchString2;

            if (launchString2 == "" || launchString2 == " ")
            {
                button1.Text = "Track Time";
                batPath2 = true;
            }

            bool hasPage = setURLs(newsString2, wikiString2);

            if (hasPage == true)
                chromeBrowser.Load(newsString2);

            button1.Click += (sender, EventArgs) => { button_Click(sender, EventArgs, launchString2, exePath2, batPath2); }; // This passes the launch URL to the launch button.
        }

        private void button1_Click(object sender, EventArgs e) { }

        private void button_Click(object sender, EventArgs e, string launchString3, bool exePath3, bool batPath3)
        {
            Process game = new Process();
            game.StartInfo.FileName = "";
            if (exePath3 == true || batPath3 == true)
            {
                game.StartInfo.FileName = launchString3;
                gameTime.Start();
                if (game.StartInfo.FileName != "" && game.StartInfo.FileName != " ")
                {
                    game.Start();
                }
                /* for (bool exit = false; exit != true;)
                {
                    int overlaySeconds = Convert.ToInt32(gameTime.ElapsedMilliseconds / 1000);
                } */
            }
            else
            {
                Uri launch2;
                launch2 = new Uri(launchString3);
                gameTime.Start();
                launcher.Url = launch2; // The game launches through URL.
            }
            button1.Visible = false;
            stopTime.Visible = true;
        }


        private Image detailedImageAssign(string input2)
        {
            Image background;
            try
            {
                background = Image.FromFile("Resources/Details/" + input2 + ".png");
                return background;
            }
            catch (FileNotFoundException e)
            {
                try
                {
                    background = Image.FromFile("Resources/Details/" + input2 + ".jpg");
                    return background;
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        background = Image.FromFile("Resources/Details/" + input2 + ".jpeg");
                        return background;
                    }
                    catch (FileNotFoundException g)
                    {
                        try
                        {
                           background = Image.FromFile("Resources/Details/" + input2 + ".gif");
                            return background;
                        }
                        catch (FileNotFoundException h)
                        {
                            noImageText.Text = "Image \"" + input2 + "\" not found.";
                            noImageText.Visible = true;
                            return background = Image.FromFile("Resources/DONT TOUCH.png");
                        }
                    }
                }
            }

        }

        private Bitmap backgroundImageAssign(string input2, Image backup)
        {
            Image background;
            Bitmap bg;
            try
            {
                background = Image.FromFile("Resources/BG/" + input2 + ".png");
                bg = new Bitmap(background);
                return bg;
            }
            catch (FileNotFoundException e)
            {
                try
                {
                    background = Image.FromFile("Resources/BG/" + input2 + ".jpg");
                    bg = new Bitmap(background);
                    return bg;
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        background = Image.FromFile("Resources/BG/" + input2 + ".jpeg");
                        bg = new Bitmap(background);
                        return bg;
                    }
                    catch (FileNotFoundException g)
                    {
                        try
                        {
                            background = Image.FromFile("Resources/BG/" + input2 + ".gif");
                            bg = new Bitmap(background);
                            return bg;
                        }
                        catch (FileNotFoundException h)
                        {
                            bg = new Bitmap(backup);
                            return bg;
                        }
                    }
                }
            }
        }

        private void stopTime_Click(object sender, EventArgs e)
        {
            gameTime.Stop();
            int seconds = Convert.ToInt32(gameTime.ElapsedMilliseconds / 1000);

            int playSeconds = seconds % 60;
            int playMinutes = seconds / 60;
            int playHours = playMinutes / 60;
            playMinutes %= 60;


            string message = "";
            if (playHours != 0 && playMinutes != 0 && playSeconds != 0)
            {
                message = "You played " + nameLabel.Text + " for " + playHours + " hours, "
                    + playMinutes + " minutes, and " + playSeconds + " seconds!";
            }
            else if (playHours != 0 && playMinutes != 0 && playSeconds == 0)
            {
                message = "You played " + nameLabel.Text + " for " + playHours + " hours and "
                    + playMinutes + " minutes!";
            }
            else if (playHours != 0 && playMinutes == 0 && playSeconds != 0)
            {
                message = "You played " + nameLabel.Text + " for " + playHours + " hours and "
                    + playSeconds + " seconds!";
            }
            else if (playHours != 0 && playMinutes == 0 && playSeconds == 0)
            {
                message = "You played " + nameLabel.Text + " for " + playHours + " hours!";
            }
            else if (playHours == 0 && playMinutes != 0 && playSeconds != 0)
            {
                message = "You played " + nameLabel.Text + " for " + playMinutes + " minutes and "
                    + playSeconds + " seconds!";
            }
            else if (playHours == 0 && playMinutes != 0 && playSeconds == 0)
            {
                message = "You played " + nameLabel.Text + " for " + playMinutes + " minutes!";
            }
            else
            {
                message = "You played " + nameLabel.Text + " for " + playSeconds + " seconds!";
            }
            string caption = "Play Time";

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);
            OleDbCommand delCmd = new OleDbCommand("DELETE FROM Table1 WHERE Title='" + nameLabel.Text + "';", con);
            OleDbCommand cmd = new OleDbCommand("INSERT INTO Table1 (Title, Platform, Status, Rating, PlayTime, Obtained, StartDate, EndDate, Notes, Launch) VALUES (@Title, @Platform, @Status, @Rating, @PlayTime, @Obtained, @StartDate, @EndDate, @Notes, @Launch);", con);

            con.Open();

            delCmd.ExecuteNonQuery();

            string platform = platformLabel.Text.Substring(10);
            string status = statusLabel.Text.Substring(8);
            string rating = ratingLabel.Text.Substring(8);
            string obtained = obtainedLabel.Text.Substring(10);
            string startDate = startDateLabel.Text.Substring(12);
            string endDate = endDateLabel.Text.Substring(10);
            string launchCode = launchLabel.Text;

            string hoursPlayed = hoursLabel.Text.Substring(13);
            string minutesPlayed = hoursLabel.Text.Substring(13);
            string secondsPlayed = hoursLabel.Text.Substring(13);

            int hourIndex = hoursPlayed.IndexOf("h");
            string hPlayed = hoursPlayed.Substring(0, hourIndex);
            int minuteIndex = minutesPlayed.IndexOf("m");
            int minuteLength = minutesPlayed.IndexOf("m") - (hourIndex + 2);
            string mPlayed = minutesPlayed.Substring(hourIndex + 2, minuteLength);
            int secondIndex = secondsPlayed.IndexOf("s");
            int secondLength = secondsPlayed.IndexOf("s") - (minuteIndex + 2);
            string sPlayed = secondsPlayed.Substring(minuteIndex + 2, secondLength);

            int oldHoursPlayed = Convert.ToInt32(hPlayed);
            int oldMinutesPlayed = Convert.ToInt32(mPlayed);
            int oldSecondsPlayed = Convert.ToInt32(sPlayed);

            int newHours = oldHoursPlayed + playHours;
            int newMinutes = oldMinutesPlayed + playMinutes;
            int newSeconds = oldSecondsPlayed + playSeconds;

            newHours += newMinutes / 60;
            newMinutes %= 60;
            newMinutes += newSeconds / 60;
            newSeconds %= 60;

            string newHoursString = newHours.ToString();
            string newMinutesString = newMinutes.ToString();
            string newSecondsString = newSeconds.ToString();

            if (newHours < 10)
                newHoursString = "0" + newHours;
            if (newMinutes < 10)
                newMinutesString = "0" + newMinutes;
            if (newSeconds < 10)
                newSecondsString = "0" + newSeconds;

            string timePlayed = newHoursString + "h:" + newMinutesString + "m:" + newSecondsString + "s";

            cmd.Parameters.AddWithValue("@Title", nameLabel.Text);
            cmd.Parameters.AddWithValue("@Platform", platform);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Rating", rating);
            cmd.Parameters.AddWithValue("@PlayTime", timePlayed);
            cmd.Parameters.AddWithValue("@Obtained", obtained);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            cmd.Parameters.AddWithValue("@Notes", notesBox.Text);
            cmd.Parameters.AddWithValue("@Launch", launchCode);
            cmd.ExecuteNonQuery();

            MessageBox.Show(message, caption);
            gameTime.Restart();
            button1.Visible = true;
            stopTime.Visible = false;
        }

        FormWindowState LastWindowState = FormWindowState.Normal;
        private void GameDetails_Resize(object sender, EventArgs e)
        {

            // When window state changes
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;

                if (WindowState == FormWindowState.Maximized)
                {
                    browserDock.Visible = true;
                    newsButton.Visible = true;
                    wikiButton.Visible = true;
                    // Maximized!
                }
                if (WindowState == FormWindowState.Normal)
                {
                    browserDock.Visible = false;
                    newsButton.Visible = false;
                    wikiButton.Visible = false;
                    // Restored!
                }
            }

        }

        private void newsButton_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load(newsUrl);
        }

        private bool setURLs(string news, string wiki)
        {
            if (news == " " || wiki == " " || news == "" || wiki == "")
            {
                this.Controls.Remove(newsButton);
                this.Controls.Remove(wikiButton);
                this.Controls.Remove(browserDock);
                return false;
            }
            else
            {
                // Create a browser component
                chromeBrowser = new ChromiumWebBrowser(news);
                // Add it to the form and fill it to the form window.
                Size browserSize = new Size(659, 88);
                chromeBrowser.Size = browserSize;
                chromeBrowser.Anchor = (AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top);
                this.browserDock.Controls.Add(chromeBrowser);
                this.newsUrl = news;
                this.wikiUrl = wiki;
                return true;
            }
        }

        private void wikiButton_Click(object sender, EventArgs e)
        {
            chromeBrowser.Load(wikiUrl);
        }
    }
}
