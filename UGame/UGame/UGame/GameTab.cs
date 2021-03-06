﻿using DiscordRPC;
using DiscordRPC.Logging;
using StackBlur;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UGame
{
    public class GameTab
    {
        public MainForm refer;
        int tabIndex;
        System.Timers.Timer timer;

        int hours;
        int minutes;
        int seconds;

        string hoursString;
        string minutesString;
        string secondsString;

        int rowIndex;
        public int id;
        string title;
        public string platform;
        public string status;
        public int rating;
        public string timePlayed;
        int totalSeconds;
        public DateTime obtained = new DateTime(1753, 1, 1, 0, 0, 0);
        public DateTime startDate = new DateTime(1753, 1, 1, 0, 0, 0);
        public DateTime lastPlayed = new DateTime(1753, 1, 1, 0, 0, 0);
        public string notes;
        public string filters;
        public string developer;
        public string publisher;
        public DateTime releaseDate;
        public string genre;
        public string playerCount;
        public decimal price;
        public string gameDesc;
        public string urlString;
        public string[,] URLs;
        int launchCount = 0;
        string launchString;
        public string[,] launchCodes;
        bool blur;
        bool useOverlay;
        bool discord;

        public string imageTitle;
        DateTime tempStartDate;
        public int screenshotCount;
        DateTime startTime;
        GameSummary gameSummary;

        public TabPage gameTab = new TabPage();
        PictureBox detailsBox = new PictureBox();
        PictureBox iconBox = new PictureBox();
        TextBox titleBox = new TextBox();
        public Button button1 = new Button();
        Button button2 = new Button();
        Button button3 = new Button();
        Button infoButton = new Button();
        Panel infoPanel = new Panel();
        Label platformLabel = new Label();
        Label ratingLabel = new Label();
        Label timePlayedLabel = new Label();
        Button calcTimeButton = new Button();
        Label lastPlayedLabel = new Label();

        static int xPos = 14;
        Point[] labelLocation = { new Point(xPos, 21), new Point(xPos, 53), new Point(xPos, 85), new Point(xPos, 117) };

        bool tracking = false;
        Overlay overlay;
        DiscordRpcClient discordRpc;
        ulong rpcStart;
        ulong rpcEnd;
        int sessionTime;

        public GameTab(MainForm refer, int rowIndex, int tabCount, int id)
        {
            this.refer = refer;
            tabIndex = tabCount - 2;

            this.rowIndex = rowIndex;
            this.id = id;

            title = refer.GamesDGV.Rows[rowIndex].Cells["Title"].Value.ToString();
            platform = refer.GamesDGV.Rows[rowIndex].Cells["Platform"].Value.ToString();
            status = refer.GamesDGV.Rows[rowIndex].Cells["Status"].Value.ToString();
            try { rating = Convert.ToInt32(refer.GamesDGV.Rows[rowIndex].Cells["Rating"].Value.ToString()); } catch { }
            timePlayed = refer.GamesDGV.Rows[rowIndex].Cells["TimePlayed"].Value.ToString();
            try { totalSeconds = Convert.ToInt32(refer.GamesDGV.Rows[rowIndex].Cells["Seconds"].Value.ToString()); } catch { }

            try { obtained = Convert.ToDateTime(refer.GamesDGV.Rows[rowIndex].Cells["Obtained"].Value.ToString()); } catch { }
            try { startDate = Convert.ToDateTime(refer.GamesDGV.Rows[rowIndex].Cells["StartDate"].Value.ToString()); } catch { }
            try { lastPlayed = Convert.ToDateTime(refer.GamesDGV.Rows[rowIndex].Cells["LastPlayed"].Value.ToString()); } catch { }
            try { releaseDate = Convert.ToDateTime(refer.GamesDGV.Rows[rowIndex].Cells["ReleaseDate"].Value.ToString()); } catch { }
            notes = refer.GamesDGV.Rows[rowIndex].Cells["Notes"].Value.ToString();
            
            // URLs
            urlString = refer.GamesDGV.Rows[rowIndex].Cells["URLs"].Value.ToString();
            int urlCount = 0;
            string segment = urlString;
            while (segment.IndexOf("[Title]") != -1)
            {
                urlCount++;
                segment = segment.Substring(segment.IndexOf("[Title]") + 7);
            }
            URLs = new string[urlCount, 2];
            segment = urlString;
            for (int index = 0; index < urlCount; index++)
            {
                segment = segment.Substring(segment.IndexOf("[Title]") + 7);

                URLs[index, 0] = segment.Substring(0, segment.IndexOf("[URL]"));
                segment = segment.Substring(segment.IndexOf("[URL]") + 5);
                try { URLs[index, 1] = segment.Substring(0, segment.IndexOf("[Title]")); } catch { URLs[index, 1] = segment; }
                try { segment = segment.Substring(segment.IndexOf("[Title]")); } catch { segment = ""; }
            }

            // FILTERS
            filters = refer.GamesDGV.Rows[rowIndex].Cells["Filters"].Value.ToString();

            developer = refer.GamesDGV.Rows[rowIndex].Cells["Developers"].Value.ToString();
            publisher = refer.GamesDGV.Rows[rowIndex].Cells["Publishers"].Value.ToString();
            genre = refer.GamesDGV.Rows[rowIndex].Cells["Genre"].Value.ToString();
            playerCount = refer.GamesDGV.Rows[rowIndex].Cells["PlayerCount"].Value.ToString();
            try { price = Convert.ToDecimal(refer.GamesDGV.Rows[rowIndex].Cells["Price"].Value.ToString()); } catch { price = -1; }
            gameDesc = refer.GamesDGV.Rows[rowIndex].Cells["GameDesc"].Value.ToString();

            // LAUNCH CODES
            launchString = refer.GamesDGV.Rows[rowIndex].Cells["Launch"].Value.ToString();
            segment = launchString;
            while (segment.IndexOf("[Title]") != -1)
            {
                launchCount++;
                segment = segment.Substring(segment.IndexOf("[Title]") + 7);
            }

            launchCodes = new string[launchCount, 2];

            try { segment = launchString.Substring(7); } catch { }

            for (int index = 0; index < launchCount; index++)
            {
                launchCodes[index, 0] = segment.Substring(0, segment.IndexOf("[URL]"));

                segment = segment.Substring(segment.IndexOf("[URL]") + 5);

                try { launchCodes[index, 1] = segment.Substring(0, segment.IndexOf("[Title]")); } catch { launchCodes[index, 1] = segment; }

                try { segment = segment.Substring(segment.IndexOf("[Title]") + 7); } catch { segment = ""; }
            }


            try { blur = Convert.ToBoolean(refer.GamesDGV.Rows[rowIndex].Cells["Blur"].Value.ToString()); } catch { blur = true; }
            try { useOverlay = Convert.ToBoolean(refer.GamesDGV.Rows[rowIndex].Cells["Overlay"].Value.ToString()); } catch { useOverlay = true; }
            try { discord = Convert.ToBoolean(refer.GamesDGV.Rows[rowIndex].Cells["Discord"].Value.ToString()); } catch { discord = true; }

            if (obtained == new DateTime())
                obtained = new DateTime(1753, 1, 1);
            if (startDate == new DateTime())
                startDate = new DateTime(1753, 1, 1);
            if (lastPlayed == new DateTime())
                lastPlayed = new DateTime(1753, 1, 1);
            if (releaseDate == new DateTime())
                releaseDate = new DateTime(1753, 1, 1);

            imageTitle = title;
            Regex rgxFix1 = new Regex("/");
            Regex rgxFix2 = new Regex(":");
            Regex rgxFix3 = new Regex(".*");
            Regex rgxFix4 = new Regex(".?");
            Regex rgxFix5 = new Regex("\"");
            Regex rgxFix6 = new Regex("<");
            Regex rgxFix7 = new Regex(">");
            Regex rgxFix8 = new Regex("|");
            Regex rgxFix9 = new Regex(@"T:\\");

            while (imageTitle.IndexOf("/") != -1)
                imageTitle = rgxFix1.Replace(imageTitle, "");
            while (imageTitle.IndexOf(":") != -1)
                imageTitle = rgxFix2.Replace(imageTitle, "");
            while (imageTitle.IndexOf("*") != -1)
                imageTitle = rgxFix3.Replace(imageTitle, "");
            while (imageTitle.IndexOf("?") != -1)
                imageTitle = rgxFix4.Replace(imageTitle, "");
            while (imageTitle.IndexOf("\"") != -1)
                imageTitle = rgxFix5.Replace(imageTitle, "");
            while (imageTitle.IndexOf("<") != -1)
                imageTitle = rgxFix6.Replace(imageTitle, "");
            while (imageTitle.IndexOf(">") != -1)
                imageTitle = rgxFix7.Replace(imageTitle, "");
            while (imageTitle.IndexOf("|") != -1)
                imageTitle = rgxFix8.Replace(imageTitle, "");
            while (imageTitle.IndexOf("\\") != -1)
                imageTitle = rgxFix9.Replace(imageTitle, "");
            
            TabCreation();
        }

        private void TabCreation()
        {
            Image bgImage = null;
            gameTab.Text = title;
            gameTab.BackColor = Color.White;
            List<string> pictures = new List<string>();
            try
            {
                string gameFolder = refer.config.resourcePath + "bg\\" + imageTitle;
                if (Directory.Exists(gameFolder))
                {
                    string[] files = Directory.GetFiles(gameFolder);

                    string temp;
                    for (int index = 0; index < files.Length; index++)
                    {
                        temp = files[index].Substring(files[index].Length - 4);

                        if (temp.IndexOf("png") != -1)
                            pictures.Add(files[index]);
                        else if (temp.IndexOf("jpg") != -1)
                            pictures.Add(files[index]);
                        else if (temp.IndexOf("jpeg") != -1)
                            pictures.Add(files[index]);
                        else if (temp.IndexOf("gif") != -1)
                            pictures.Add(files[index]);
                        else if (temp.IndexOf("jfif") != -1)
                            pictures.Add(files[index]);
                        else if (temp.IndexOf("webp") != -1)
                            pictures.Add(files[index]);
                    }
                    
                    Random randomPicture = new Random();
                    int fileToUse = randomPicture.Next(0, pictures.Count);
                    bgImage = Image.FromFile(pictures[fileToUse]);
                }
                else
                    throw new Exception();
            }
            catch { try { bgImage = Image.FromFile(refer.config.resourcePath + "bg\\" + imageTitle + ".png"); }
            catch { try { bgImage = Image.FromFile(refer.config.resourcePath + "bg\\" + imageTitle + ".jpg"); }
            catch { try { bgImage = Image.FromFile(refer.config.resourcePath + "bg\\" + imageTitle + ".jpeg"); }
            catch { try { bgImage = Image.FromFile(refer.config.resourcePath + "bg\\" + imageTitle + ".gif"); }
            catch { try { bgImage = Image.FromFile(refer.config.resourcePath + "bg\\" + imageTitle + ".jfif"); }
            catch { } } } } } }
            try
            {
                Bitmap bg = new Bitmap(bgImage);
                if (blur)
                {
                    var radius = 20;
                    StackBlur.StackBlur.Process(bg, radius);
                }
                gameTab.BackgroundImage = bg;
                gameTab.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch { }

            detailsBox.Location = new Point(7, 7);
            detailsBox.Size = new Size(351, 351);
            try { detailsBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "details\\" + imageTitle + ".png"); }
            catch { try { detailsBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "details\\" + imageTitle + ".jpg"); }
            catch { try { detailsBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "details\\" + imageTitle + ".jpeg"); }
            catch { try { detailsBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "details\\" + imageTitle + ".gif"); }
            catch { try { detailsBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "details\\" + imageTitle + ".jfif"); }
            catch { } } } } }
            detailsBox.BackgroundImageLayout = ImageLayout.Zoom;
            detailsBox.BackColor = Color.Transparent;

            iconBox.Location = new Point(365, 7);
            iconBox.Size = new Size(68, 68);
            try { iconBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "icons\\" + imageTitle + ".png"); }
            catch { try { iconBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "icons\\" + imageTitle + ".jpg"); }
            catch { try { iconBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "icons\\" + imageTitle + ".jpeg"); }
            catch { try { iconBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "icons\\" + imageTitle + ".gif"); }
            catch { try { iconBox.BackgroundImage = Image.FromFile(refer.config.resourcePath + "icons\\" + imageTitle + ".jfif"); }
            catch { } } } } }
            iconBox.BackgroundImageLayout = ImageLayout.Zoom;
            iconBox.BackColor = Color.Transparent;


            if (iconBox.BackgroundImage == null && detailsBox.BackgroundImage != null)
                iconBox.BackgroundImage = detailsBox.BackgroundImage;

            if (detailsBox.BackgroundImage == null && iconBox.BackgroundImage != null)
                detailsBox.BackgroundImage = iconBox.BackgroundImage;
            if (detailsBox.BackgroundImage == null && gameTab.BackgroundImage != null)
                detailsBox.BackgroundImage = gameTab.BackgroundImage;

            if (gameTab.BackgroundImage == null && detailsBox.BackgroundImage != null)
                gameTab.BackgroundImage = detailsBox.BackgroundImage;
            if (gameTab.BackgroundImage == null && iconBox.BackgroundImage != null)
                gameTab.BackgroundImage = iconBox.BackgroundImage;

            if (iconBox.BackgroundImage != null)
                titleBox.Location = new Point(440, 14);
            else
                titleBox.Location = new Point(508, 14);

            titleBox.Size = new Size(593, 68);
            titleBox.BorderStyle = BorderStyle.None;
            titleBox.Font = new Font("Century Gothic", 32);
            titleBox.Text = title;
            titleBox.ReadOnly = true;
            titleBox.ScrollBars = ScrollBars.Horizontal;
            titleBox.BackColor = Color.White;

            button1.Location = new Point(365, 82);
            button1.Size = new Size(177, 34);
            button1.Text = "Launch & Track";
            button1.UseMnemonic = false;
            button1.UseVisualStyleBackColor = true;
            button1.Tag = rowIndex;
            button1.TabIndex = 0;
            button1.Click += Button1_Click;

            button2.Location = new Point(548, 82);
            button2.Size = new Size(177, 34);
            button2.Text = "Track";
            button2.UseVisualStyleBackColor = true;
            button2.Tag = rowIndex;
            button2.Click += Button2_Click;

            button3.Location = new Point(731, 82);
            button3.Size = new Size(177, 34);
            button3.Text = "Launch";
            button3.UseVisualStyleBackColor = true;
            button3.Tag = rowIndex;
            button3.Click += Button3_Click;

            infoButton.Location = new Point(958, 82);
            infoButton.Size = new Size(75, 34);
            infoButton.Text = "More Info";
            infoButton.UseVisualStyleBackColor = true;
            infoButton.Tag = rowIndex;
            infoButton.Click += InfoButton_Click;

            infoPanel.Location = new Point(365, 122);
            infoPanel.Size = new Size(668, 236);

            int locIndex = 0;
            Font labelFont = new Font("Century Gothic", 12);
            if (platform != "")
            {
                platformLabel.Location = labelLocation[locIndex];
                platformLabel.Font = labelFont;
                platformLabel.AutoSize = true;
                platformLabel.Text = "Platform: " + platform;
                locIndex++;
            }

            if (rating != 0)
            {
                ratingLabel.Location = labelLocation[locIndex];
                ratingLabel.Font = labelFont;
                ratingLabel.AutoSize = true;
                ratingLabel.Text = "Rating: ";
                for (int starNum = 0; starNum < rating; starNum++)
                    ratingLabel.Text += "★";
                for (int emptyNum = 0; emptyNum < 10 - rating; emptyNum++)
                    ratingLabel.Text += "☆";
                ratingLabel.Text += " (" + rating + "/10)";
                locIndex++;
            }

            if (timePlayed != "")
            {
                timePlayedLabel.Location = labelLocation[locIndex];
                timePlayedLabel.Font = labelFont;
                timePlayedLabel.AutoSize = true;
                timePlayedLabel.Text = "Time Played          : " + timePlayed;

                calcTimeButton.Location = new Point(116, labelLocation[locIndex].Y);
                calcTimeButton.Size = new Size(26, 26);
                calcTimeButton.UseVisualStyleBackColor = true;
                calcTimeButton.Text = "...";
                calcTimeButton.Click += CalcTimeButton_Click;

                locIndex++;
            }

            if (lastPlayed.ToString() != "1/1/0001 12:00:00 AM")
            {
                lastPlayedLabel.Location = labelLocation[locIndex];
                lastPlayedLabel.Font = labelFont;
                lastPlayedLabel.AutoSize = true;
                lastPlayedLabel.Text = "Last Played: " + lastPlayed;
            }
           
            gameTab.Controls.Add(detailsBox);
            gameTab.Controls.Add(iconBox);
            gameTab.Controls.Add(titleBox);
            gameTab.Controls.Add(button1);
            gameTab.Controls.Add(button2);
            gameTab.Controls.Add(button3);
            gameTab.Controls.Add(infoButton);
            gameTab.Controls.Add(infoPanel);

            infoPanel.Controls.Add(platformLabel);
            infoPanel.Controls.Add(ratingLabel);
            infoPanel.Controls.Add(timePlayedLabel);
            infoPanel.Controls.Add(calcTimeButton);
            infoPanel.Controls.SetChildIndex(calcTimeButton, 0);
            infoPanel.Controls.Add(lastPlayedLabel);

            refer.AddGameTab(gameTab);

            if (launchString == "[Title]Default[URL]" || launchCount == 0)
            {
                button1.Visible = false;
                button3.Visible = false;
                
                button2.Location = new Point(365, 82);

                button2.TabIndex = 0;
            }
        }

        public void Launch()
        {
            bool launchable = true;

            string launchCode = "";
            if (launchCount > 1)
            {
                LaunchChoice launchChoice = new LaunchChoice(launchCodes, launchCount);
                DialogResult dialogResult = launchChoice.ShowDialog();
                if (dialogResult == DialogResult.Yes)
                {
                    launchCode = launchCodes[launchChoice.index, 1];
                }
            }
            else
            {
                try { launchCode = launchCodes[0, 1]; } catch { launchable = false; }
            }
            
            if (launchable)
            {
                bool isExe = false;
                bool hasArgs = false;
                ProcessStartInfo startInfo;

                if (launchCode.IndexOf(".exe") != -1)
                    isExe = true;
                else if (launchCode.IndexOf(".lnk") != -1)
                    isExe = true;
                else if (launchCode.IndexOf(".bat") != -1)
                    isExe = true;

                try
                {
                    int exeLoc = launchCode.IndexOf(".exe");
                    string lookForArgs = launchCode.Substring(exeLoc);

                    if (lookForArgs.IndexOf("-") != -1 && lookForArgs.IndexOf("\"") != -1)
                        hasArgs = true;
                }
                catch (ArgumentOutOfRangeException) { }

                if (isExe)
                {
                    if (hasArgs)
                    {
                        int getRidOfQuotes = launchCode.IndexOf("\"");
                        if (getRidOfQuotes == 0)
                        {
                            launchCode.Substring(1);
                            int secondQuote = launchCode.IndexOf("\"");
                            launchCode.Substring(0, secondQuote);
                        }
                        int exeLoc = launchCode.IndexOf(".exe");
                        string fileName = launchCode.Substring(0, exeLoc + 5);
                        string args = launchCode.Substring(exeLoc + 5);

                        startInfo = new ProcessStartInfo(fileName, args);
                    }
                    else
                    {
                        startInfo = new ProcessStartInfo(launchCode);
                    }
                    
                    try { Process.Start(startInfo); }
                    catch (Win32Exception) { MessageBox.Show("Launch failed, game not found."); }
                }
                else
                {
                    refer.BrowserLauncher.Url = new Uri(launchCode);
                }
            }
        }

        public void Track()
        {
            button1.Visible = true;
            button3.Visible = true;
            button2.Location = new Point(548, 82);

            if (startDate == new DateTime(1753, 1, 1, 0, 0, 0))
                tempStartDate = DateTime.Now;

            tracking = true;

            overlay = new Overlay(title, iconBox.BackgroundImage, this);

            timer = new System.Timers.Timer
            {
                Interval = 1000
            };
            timer.Elapsed += Timer_Elapsed;

            startTime = DateTime.Now;
            timer.Start();
            hours = 0;
            minutes = 0;
            seconds = 0;

            gameTab.Text = "(...) " + title;
            button1.Text = "Stop Playing (00h:00m:00s)";
            button2.Text = "Pause Playing";
            button3.Text = "Discard Session";

            if (discord)
                InitRichPresence();
        }

        private void InitRichPresence()
        {
            discordRpc = new DiscordRpcClient("556202672236003329");
            discordRpc.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

            discordRpc.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            discordRpc.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            discordRpc.Initialize();

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            rpcStart = (ulong)t.TotalSeconds;

            discordRpc.SetPresence(new RichPresence()
            {
                Details = title,
                State = "Playing",
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                },

                Assets = new Assets()
                {
                    LargeImageKey = "default",
                    // LargeImageText = "Lachee's Discord IPC Library",
                    SmallImageKey = "play",
                    SmallImageText = "Playing"
                }
            });
        }

        public void Stop(bool save)
        {
            discordRpc.Dispose();
            overlay.Close();
            tracking = false;

            button1.Text = "Launch & Track";
            button2.Text = "Track";
            button3.Text = "Launch";

            if (save)
            {
                if (startDate == new DateTime(1753, 1, 1, 0, 0, 0))
                    startDate = tempStartDate;


                totalSeconds += seconds + (minutes * 60) + (hours * 3600);

                int hour; int min; int sec;
                
                min = totalSeconds / 60;
                sec = totalSeconds % 60;
                hour = min / 60;
                min %= 60;

                string hourString = "0"; string minString = "0"; string secString = "0";
                if (hour < 10)
                    hourString += hour;
                else
                    hourString = hour.ToString();
                hourString += "h:";
                
                if (min < 10)
                    minString += min;
                else
                    minString = min.ToString();
                minString += "m:";

                if (sec < 10)
                    secString += sec;
                else
                    secString = sec.ToString();
                secString += "s";

                timePlayed = hourString + minString + secString;

                DateTime lastPlayed = DateTime.Now;

                // WRITE EVERYTHING TO THE DATABASE
                refer.UpdateTime(timePlayed, totalSeconds, lastPlayed, id, startDate);
                // WRITE EVERYTHING TO THE DATABASE

                timePlayedLabel.Text = "Time Played          : " + timePlayed;
                lastPlayedLabel.Text = "Last Played: " + lastPlayed.ToString();

                for (int index = 0; index < refer.GamesDGV.Rows.Count; index++)
                {
                    if (Convert.ToInt32(refer.GamesDGV.Rows[index].Cells["Id"].Value) == id)
                    {
                        refer.GamesDGV.Rows[index].Cells["TimePlayed"].Value = timePlayed;
                        refer.GamesDGV.Rows[index].Cells["Seconds"].Value = totalSeconds;
                        refer.GamesDGV.Rows[index].Cells["StartDate"].Value = startDate;
                        refer.GamesDGV.Rows[index].Cells["LastPlayed"].Value = lastPlayed;
                        break;
                    }
                }

                gameSummary = new GameSummary(title, seconds, minutes, hours, iconBox.BackgroundImage, startTime, DateTime.Now);
                gameSummary.Show();
            }

            gameTab.Text = title;

            if (launchString == "[Title]Default[URL]" || launchCount == 0)
            {
                button1.Visible = false;
                button3.Visible = false;

                button2.Location = new Point(365, 82);
            }
        }
        
        public void PauseResume()
        {
            if (button2.Text == "Pause Playing")
            {
                timer.Stop();
                
                if (discord)
                {
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    rpcEnd = (ulong)t.TotalSeconds;

                    discordRpc.SetPresence(new RichPresence()
                    {
                        Details = title,
                        State = "Paused",
                        Timestamps = new Timestamps()
                        {
                            Start = null,
                            End = null
                        },

                        Assets = new Assets()
                        {
                            LargeImageKey = "default",
                            // LargeImageText = "Lachee's Discord IPC Library",
                            SmallImageKey = "pause",
                            SmallImageText = "Paused"
                        }
                    });
                }

                SetText("Resume Playing", ref button2);
                overlay.PauseButton.Text = "Resume Playing";
            }
            else
            {
                timer.Start();

                if (discord)
                {
                    ulong timePlayed = rpcEnd - rpcStart;
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    rpcStart = (ulong)t.TotalSeconds - timePlayed;

                    discordRpc.SetPresence(new RichPresence()
                    {
                        Details = title,
                        State = "Playing",
                        Timestamps = new Timestamps()
                        {
                            StartUnixMilliseconds = rpcStart,
                        },

                        Assets = new Assets()
                        {
                            LargeImageKey = "default",
                            // LargeImageText = "Lachee's Discord IPC Library",
                            SmallImageKey = "play",
                            SmallImageText = "Playing"
                        }
                    });
                }

                SetText("Pause Playing", ref button2);
                overlay.PauseButton.Text = "Pause Playing";
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (tracking)
            {
                seconds++;

                if (seconds == 60)
                {
                    seconds = 0;
                    minutes++;
                }
                if (minutes == 60)
                {
                    minutes = 0;
                    hours++;
                }

                secondsString = seconds.ToString();
                minutesString = minutes.ToString();
                hoursString = hours.ToString();

                if (seconds < 10)
                    secondsString = "0" + seconds;
                if (minutes < 10)
                    minutesString = "0" + minutes;
                if (hours < 10)
                    hoursString = "0" + hours;

                secondsString += "s";
                minutesString += "m:";
                hoursString += "h:";

                SetText("Stop Playing (" + hoursString + minutesString + secondsString + ")", ref button1);
            }
            else
            {
                SetText("Launch & Track", ref button1);
                timer.Stop();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Launch & Track")
            {
                Button tempButton = (Button)sender;
                int tabIndex = Convert.ToInt32(tempButton.Tag);
                Launch();
                Track();
            }
            else
            {
                Stop(true);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Track")
            {
                Button tempButton = (Button)sender;
                int tabIndex = Convert.ToInt32(tempButton.Tag);
                Track();
            }
            else
            {
                PauseResume();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "Launch")
            {
                Button tempButton = (Button)sender;
                int tabIndex = Convert.ToInt32(tempButton.Tag);
                Launch();
            }
            else
            {
                Stop(false);
            }
        }

        private void InfoButton_Click(object sender, EventArgs e) // IMPROVE TO HIDE FIELDS WHEN A VALUE IS NOT WORTH SHOWING (EX: NULL, RATING = 0, etc.)
        {
            string caption = "Info on \"" + title + "\"";
            string message = "Title: " + title + "\nPlatform: " + platform + "\nStatus: " + status + "\nRating: " + rating + "\nTime Played: " + timePlayed + "\nObtained: ";
            
            if (obtained.ToString() != "1/1/1753 12:00:00 AM")
                message += obtained;

            message += "\nStarted: ";
            if (startDate.ToString() != "1/1/1753 12:00:00 AM")
                message += startDate;

            message += "\nLast Played: ";
            if (lastPlayed.ToString() != "1/1/1753 12:00:00 AM")
                message += lastPlayed;

            message += "\nNotes: " + notes + "\nFilters: " + filters + "\nDevelopers: " + developer + "\nRelease Date: " + releaseDate + "\nGenre: " + genre + "\nPublishers: " + publisher + "\nPlayer Count: " + playerCount + "\nPrice: ";

            if (price > 0)
            {
                message += price.ToString("C");
            }
            else if (price == 0)
            {
                message += "Free";
            }

            message += "\nGame Description: " + gameDesc;

            MessageBox.Show(message, caption);
        }

        private void CalcTimeButton_Click(object sender, EventArgs e)
        {
            string caption = "Time Calculations for \"" + title + "\"";
            string message = "Total Seconds: " + totalSeconds + "\nTotal Minutes: " + totalSeconds / 60 + "\nTotal Hours: " + totalSeconds / 3600 + 
                "\nTotal Days: " + totalSeconds / 86400 + "\nTotal Weeks: " + totalSeconds / 604800 + "\nTotal Months: " + totalSeconds / 2592000 + 
                "\nTotal Years: " + totalSeconds / 31557600;

            MessageBox.Show(message, caption);
        }

        delegate void SetTextCallback(string text, ref Button button);

        private void SetText(string text, ref Button button)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (button1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                refer.Invoke(d, new object[] { text, button });
            }
            else
            {
                button.Text = text;
            }
        }

        public void Close()
        {
            if (button3.Text == "Discard Session") // maybe use if (tracking)
            {
                string message = "Would you like to save the play session for \"" + titleBox.Text + "\"?";
                string caption = "Closing Tab";

                DialogResult dialogResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                {
                    Stop(true);
                }
                else if (dialogResult == DialogResult.No)
                {
                    Stop(false);
                }
                else
                {

                }
            }
        }
    }
}
