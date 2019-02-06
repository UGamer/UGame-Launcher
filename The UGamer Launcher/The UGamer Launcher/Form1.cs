﻿using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using CefSharp;
using CefSharp.WinForms;
using System.ComponentModel;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Threading;

namespace The_UGamer_Launcher
{
    public partial class Form1 : Form
    {
        public GameDetails gameWindow;
        public AddGame addGame;
        public Thread notificationCheck;
        private bool displayData = false;
        DataTable newTable;

        public Form1()
        {
            // Starts up the program.
            InitializeComponent();
            notificationCheck = new Thread(new ThreadStart(NotificationSystem));
            try
            {
                IconAssign();
            }
            catch (FileNotFoundException e) { }

            try
            {
                File.Delete("Resources/Theme/logoUSING.png");
                File.Delete("Resources/Theme/backgroundImageUSING.png");
            }
            catch (FileNotFoundException e) { }

            try
            {
                File.Copy("Resources/Theme/backgroundImage.png", "Resources/Theme/backgroundImageUSING.png");
                this.BackgroundImage = ThemeAssign("backgroundImageUSING");
                File.Delete("Resources/Theme/backgroundImage.png");
            }
            catch (FileNotFoundException e)
            {
                try
                {
                    File.Copy("Resources/Theme/backgroundImage.jpg", "Resources/Theme/backgroundImageUSING.png");
                    this.BackgroundImage = ThemeAssign("backgroundImageUSING");
                    File.Delete("Resources/Theme/backgroundImage.jpg");
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        File.Copy("Resources/Theme/backgroundImage.jpeg", "Resources/Theme/backgroundImageUSING.png");
                        this.BackgroundImage = ThemeAssign("backgroundImageUSING");
                        File.Delete("Resources/Theme/backgroundImage.jpeg");
                    }
                    catch (FileNotFoundException g)
                    {
                        try
                        {
                            File.Copy("Resources/Theme/backgroundImage.gif", "Resources/Theme/backgroundImageUSING.png");
                            this.BackgroundImage = ThemeAssign("backgroundImageUSING");
                            File.Delete("Resources/Theme/backgroundImage.gif");
                        }
                        catch (FileNotFoundException h) { }
                    }
                }
            }

            try
            {
                File.Copy("Resources/Theme/logo.png", "Resources/Theme/logoUSING.png");
                logo.BackgroundImage = ThemeAssign("logoUSING");
                File.Delete("Resources/Theme/logo.png");
            }
            catch (FileNotFoundException e)
            {
                try
                {
                    File.Copy("Resources/Theme/logo.jpg", "Resources/Theme/logoUSING.png");
                    logo.BackgroundImage = ThemeAssign("logoUSING");
                    File.Delete("Resources/Theme/logo.jpg");
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        File.Copy("Resources/Theme/logo.jpeg", "Resources/Theme/logoUSING.png");
                        logo.BackgroundImage = ThemeAssign("logoUSING");
                        File.Delete("Resources/Theme/logo.jpeg");
                    }
                    catch (FileNotFoundException g)
                    {
                        try
                        {
                            File.Copy("Resources/Theme/logo.gif", "Resources/Theme/logoUSING.png");
                            logo.BackgroundImage = ThemeAssign("logoUSING");
                            File.Delete("Resources/Theme/logo.gif");
                        }
                        catch (FileNotFoundException h)
                        {
                        }
                    }
                }
            }
        }

        public class Statuses
        {

        }


        // This fills the data table with the user data.
        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'notificationsSet.Notifications' table. You can move, or remove it, as needed.
            this.notificationsTableAdapter1.Fill(this.notificationsSet.Notifications);
            // TODO: This line of code loads data into the 'notificationDataSet.Notifications' table. You can move, or remove it, as needed.
            this.notificationsTableAdapter.Fill(this.notificationDataSet.Notifications);
            // TODO: This line of code loads data into the 'collectionDataSet5.Table1' table. You can move, or remove it, as needed.
            this.table1TableAdapter2.Fill(this.collectionDataSet4.Table1);
            // TODO: This line of code loads data into the 'collectionDataSet5.Themes' table. You can move, or remove it, as needed.
            try
            {
                this.table1TableAdapter2.Fill(this.collectionDataSet4.Table1);
                dataTable.Sort(dataTable.Columns[0], ListSortDirection.Ascending);
            }
            // This is caught if you don't have the required OLE DB drivers.
            catch (InvalidOperationException d)
            {
                dataTable.Visible = false;
                driverWarning.Visible = true;
                Uri installURL = new Uri("https://www.microsoft.com/en-us/download/confirmation.aspx?id=23734");
                driverInstall.Url = installURL;
                searchBox.Visible = false;
                gameCountText.Visible = false;
                addEntryButton.Visible = false;
            }

            int entryCount = dataTable.Rows.Count;
            if (entryCount != 1)
                gameCountText.Text = Convert.ToString(entryCount) + " total games";
            else
                gameCountText.Text = Convert.ToString(entryCount) + " total game";

            DataTable dt = collectionDataSet4.Table1;
            AutoCompleteStringCollection autoFill = new AutoCompleteStringCollection();
            int columnIndex = 1; // Name column
            string[] table = new string[dt.Rows.Count];
            int index = 0;
            for (index = 0; index < dt.Rows.Count; index++)
            {
                table[index] = dt.Rows[index][columnIndex].ToString();
                autoFill.Add(table[index]);
            }
            searchBox.AutoCompleteCustomSource = autoFill;

            /* 
            var dataSource1 = new List<Statuses>();
            var dataSource2 = new List<Statuses>();
            columnIndex = 3;
            for (index = 0; index < dt.Rows.Count; index++)
            {
                dataSource1.Add();
            } */

            
            var columnSource = new List<CategoryColumn>();
            columnIndex = 3;
            for (index = 0; index < dt.Columns.Count; index++)
            {
                columnSource.Add(new CategoryColumn() { Name = "blah"});
            }

            CefSettings settings = new CefSettings();
            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            dataTable.SortCompare += customSortCompare;
        }

        private void NotificationSystem()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);

            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Notifications", con);
            OleDbCommand cmd2 = new OleDbCommand("SELECT * FROM Table1", con);
            OleDbCommand addNowPlayingNotif = new OleDbCommand("INSERT INTO Notifications (DateAdded, NotificationType, GameTitle, Message, Action) VALUES (@DateAdded, @NotificationType, @GameTitle, @Message, @Action);", con);
            
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable notificationTable = new DataTable();
            da.Fill(notificationTable);
            
            cmd2.CommandType = CommandType.Text;
            OleDbDataAdapter da2 = new OleDbDataAdapter(cmd2);
            DataTable gameTable = new DataTable();
            da2.Fill(gameTable);


            int gameTableNameIndex = 1;
            int statusIndex = 3;
            int lastPlayedIndex = 8;
            int dateIndex = 1; // Name column
            int typeIndex = 2;
            string[] NowPlaying;

            DateTime fourteenDaysAgo = DateTime.Today.AddDays(-14);
            Regex dateFix = new Regex("-");
            string endDate1 = fourteenDaysAgo.ToString("u");
            string twoWeeksAgo = endDate1.Substring(0, 10);
            twoWeeksAgo = dateFix.Replace(twoWeeksAgo, "/");

            DateTime today2 = DateTime.Now;
            string todayDate = today2.ToString("u");
            string todayString = endDate1.Substring(0, 10);
            todayString = dateFix.Replace(todayString, "/");


            string[] gameTable2Names = new string[gameTable.Rows.Count];
            string[] notifTable2Dates = new string[notificationTable.Rows.Count];
            string[] notifTable2Types = new string[notificationTable.Rows.Count];
            int index = 0;
            for (index = 0; index < gameTable.Rows.Count; index++)
            {
                if (gameTable.Rows[index][statusIndex].ToString() == "Dropped" ||
                    gameTable.Rows[index][statusIndex].ToString() == "Never Started" ||
                    gameTable.Rows[index][statusIndex].ToString() == "On Hold" ||
                    gameTable.Rows[index][statusIndex].ToString() == "Plan to Play" ||
                    gameTable.Rows[index][statusIndex].ToString() == "Start Over" ||
                    gameTable.Rows[index][statusIndex].ToString() == "Don't Have")
                {
                    addNowPlayingNotif.Parameters.AddWithValue("@DateAdded", todayString);
                    addNowPlayingNotif.Parameters.AddWithValue("@NotificationType", "NowPlaying");
                    addNowPlayingNotif.Parameters.AddWithValue("@GameTitle", gameTable.Rows[index][gameTableNameIndex].ToString());
                    addNowPlayingNotif.Parameters.AddWithValue("@Message", "You started playing " + gameTable.Rows[index][gameTableNameIndex].ToString() + ". Would you like to change it's status?");
                    addNowPlayingNotif.Parameters.AddWithValue("@Action", "Change");

                    addNowPlayingNotif.ExecuteNonQuery();
                }
                    
                gameTable2Names[index] = gameTable.Rows[index][gameTableNameIndex].ToString();
            }


            con.Close();
        }

        private void dataTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string titleValue;
            try
            {
                object value = dataTable.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                titleValue = value.ToString();
                dataScan(titleValue);
            }
            catch (ArgumentOutOfRangeException f) { }
        }

        private void addEntryButton_Click(object sender, EventArgs e)
        {
            if (displayData == true)
            {
                NotificationsDGV.Visible = false;
                dataTable.Visible = true;
                displayData = false;
                addEntryButton.Text = "Notifications";
            }

            else
            {
                NotificationsDGV.Visible = true;
                dataTable.Visible = false;
                displayData = true;
                addEntryButton.Text = "Collection";
            }
        }

        private void searchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                dataScan(searchBox.Text);
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            dataScan(searchBox.Text);
        }

        public void dataScan(String input)
        {
            string input2 = "";
            int y = 0, z = 0;

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);

            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Table1", con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable newTable = new DataTable();
            da.Fill(newTable);

            con.Close();

            // This makes the whole database into an array.

            // DataTable dt = collectionDataSet4.Table1;
            DataTable dt = newTable;
            int columnIndex = 1; // Name column
            string[] table = new string[dt.Rows.Count];
            int index = 0;
            for (index = 0; index < dt.Rows.Count; index++)
            {
                table[index] = dt.Rows[index][columnIndex].ToString();
            }

            // This checks if the user input is actually an entry.
            input2 = input;
            Regex pathFix = new Regex(@"T:\\");

            // This section fixes the title so it can be translated to an image file.
            Regex rgxFix1 = new Regex("/");
            Regex rgxFix2 = new Regex(":");
            Regex rgxFix3 = new Regex(".*");
            Regex rgxFix4 = new Regex(".?");
            Regex rgxFix5 = new Regex("\"");
            Regex rgxFix6 = new Regex("<");
            Regex rgxFix7 = new Regex(">");
            Regex rgxFix8 = new Regex("|");

            if (input2.IndexOf("\\") != -1)
                input2 = pathFix.Replace(input, "/");
            if (input2.IndexOf("/") != -1)
                input2 = rgxFix1.Replace(input2, "");
            if (input2.IndexOf(":") != -1)
                input2 = rgxFix2.Replace(input2, "");
            if (input2.IndexOf("*") != -1)
                input2 = rgxFix3.Replace(input2, "");
            if (input2.IndexOf("?") != -1)
                input2 = rgxFix4.Replace(input2, "");
            if (input2.IndexOf("\"") != -1)
                input2 = rgxFix5.Replace(input2, "");
            if (input2.IndexOf("<") != -1)
                input2 = rgxFix6.Replace(input2, "");
            if (input2.IndexOf(">") != -1)
                input2 = rgxFix7.Replace(input2, "");
            if (input2.IndexOf("|") != -1)
                input2 = rgxFix8.Replace(input2, "");

            for (int x = 0; x < dt.Rows.Count; x++)
                if (input == table[x])
                {
                    z = x;
                    y = 1;
                }

            // This transfers all of the entry's data to the Game Details window.
            gameWindow = new GameDetails();
            gameWindow.FormClosed += new FormClosedEventHandler(gameWindow_FormClosed);
            if (y == 1)
            {
                string platform = dt.Rows[z][2].ToString();
                string status = dt.Rows[z][3].ToString();
                string rating = dt.Rows[z][4].ToString();
                string hours = dt.Rows[z][5].ToString();
                string obtained = dt.Rows[z][6].ToString();
                string startDate = dt.Rows[z][7].ToString();
                string endDate = dt.Rows[z][8].ToString();
                string notes = dt.Rows[z][9].ToString();
                string launchString = dt.Rows[z][10].ToString();
                string newsString = dt.Rows[z][11].ToString();
                string wikiString = dt.Rows[z][12].ToString();

                if (launchString.IndexOf("\\") != -1)
                    launchString = pathFix.Replace(launchString, "/"); // This fixes .exe links automatically.

                bool exePath = isExe(launchString);
                bool batPath = isBat(launchString);
                bool hasArgs = false;

                if (exePath == true || batPath == true)
                    hasArgs = hasArgsMethod(launchString);

                gameWindow.Text = input;
                gameWindow.Show();
                gameWindow.DisplayInfo(input, input2, platform, status, rating,
                    hours, obtained, startDate, endDate, notes, launchString, exePath, batPath,
                    newsString, wikiString, hasArgs);
                noGameLabel.Visible = false;
                y = 0;
            }
            // If the entry does not exist, an error message shows.
            else
                noGameLabel.Visible = true;
        }

        private static bool isExe(string p)
        {
            if (p.IndexOf(".exe") == -1)
                return false;
            else
                return true;
        }

        private static bool isBat(string p)
        {
            if (p.IndexOf(".bat") == -1)
                return false;
            else
                return true;
        }

        private static bool hasArgsMethod(string p)
        {
            int exeLoc = p.IndexOf(".exe");
            string lookForArgs = p.Substring(exeLoc);

            if (lookForArgs.IndexOf("-") == -1)
                return false;
            else
                return true;
        }

        public Image ThemeAssign(string input2)
        {
            Image background;
            try
            {
                background = Image.FromFile("Resources/Theme/" + input2 + ".png");
                return background;
            }
            catch (FileNotFoundException e)
            {
                try
                {
                    background = Image.FromFile("Resources/Theme/" + input2 + ".jpg");
                    return background;
                }
                catch (FileNotFoundException f)
                {
                    try
                    {
                        background = Image.FromFile("Resources/Theme/" + input2 + ".jpeg");
                        return background;
                    }
                    catch (FileNotFoundException g)
                    {
                        try
                        {
                            background = Image.FromFile("Resources/Theme/" + input2 + ".gif");
                            return background;
                        }
                        catch (FileNotFoundException h)
                        {
                            return background = Image.FromFile("Resources/DONT TOUCH.png");
                        }
                    }
                }
            }
        }

        public void IconAssign()
        {
            Icon windowIcon;
            try
            {
                windowIcon = new Icon("Resources/Theme/icon.ico");
                this.Icon = windowIcon;
            }
            catch (FileNotFoundException e)
            {

            }
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Show();
        }

        public Icon IconAssign(string input2)
        {
            Icon windowIcon;
            try
            {
                windowIcon = new Icon("Resources/Theme/" + input2 + ".ico");
                return windowIcon;
            }
            catch (FileNotFoundException e)
            {
                windowIcon = new Icon("Resources/Theme/icon.ico");
                return windowIcon;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);

            dataTable.DataSource = null;
            dataTable.Update();
            dataTable.Refresh();
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Table1", con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            DataTable newTable = new DataTable();
            da.Fill(newTable);
            dataTable.DataSource = newTable;
            dataTable.Sort(dataTable.Columns[0], ListSortDirection.Ascending);
            con.Close();

            int entryCount = dataTable.Rows.Count;
            if (entryCount != 1)
                gameCountText.Text = Convert.ToString(entryCount) + " total games";
            else
                gameCountText.Text = Convert.ToString(entryCount) + " total game";

            DataTable dt = newTable;
            AutoCompleteStringCollection autoFill = new AutoCompleteStringCollection();
            int columnIndex = 1; // Name column
            string[] table = new string[dt.Rows.Count];
            int index = 0;
            for (index = 0; index < dt.Rows.Count; index++)
            {
                table[index] = dt.Rows[index][columnIndex].ToString();
                autoFill.Add(table[index]);
            }
            searchBox.AutoCompleteCustomSource = autoFill;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings(this);
            settings.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                File.Copy("Resources/Theme/backgroundImageUSING.png", "Resources/Theme/backgroundImage.png");
                File.Copy("Resources/Theme/logoUSING.png", "Resources/Theme/logo.png");
            }
            catch
            {

            }
        }

        private void customSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 5)
            {
                string value1 = e.CellValue1.ToString();
                string value2 = e.CellValue2.ToString();
                Regex fixingTime1 = new Regex("h:");
                Regex fixingTime2 = new Regex("m:");
                Regex fixingTime3 = new Regex("s");
                fixingTime1.Replace(value1, "");
                fixingTime2.Replace(value1, "");
                fixingTime3.Replace(value1, "");
                
                fixingTime1.Replace(value2, "");
                fixingTime2.Replace(value2, "");
                fixingTime3.Replace(value2, "");

                int value1Int = Convert.ToInt32(value1);
                int value2Int = Convert.ToInt32(value2);

                e.SortResult = int.Parse(e.CellValue1.ToString()).CompareTo(int.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }

            dataTable.SortCompare += customSortCompare;
        }

        private void EntriesToolTipButton_Click_1(object sender, EventArgs e)
        {
            bool refresh = false;
            addGame = new AddGame(this, refresh);
            addGame.Show();
            addGame.FormClosed += new FormClosedEventHandler(addGame_FormClosed);
        }

        private void gameWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (gameWindow.refresh == true)
                RefreshGrid();
        }

        private void addGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (addGame.refresh == true)
                RefreshGrid();
        }

        private class CategoryColumn
        {
            public string Name { get; set; }
        }

        private void replaceEntryMethod(string originalTitleString, string title, string platform, string status,
            string rating, string hours, string minutes,
            string seconds, string obtained, string startDate,
                string endDate, string launchCode, string notes, string newsCode, string wikiCode)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);

            OleDbCommand delCmd = new OleDbCommand("DELETE FROM Table1 WHERE Title=\"" + originalTitleString + "\";", con);
            OleDbCommand cmd = new OleDbCommand("INSERT INTO Table1 (Title, Platform, Status, Rating, PlayTime, Obtained, StartDate, EndDate, Notes, Launch, News, Wiki) VALUES (@Title, @Platform, @Status, @Rating, @PlayTime, @Obtained, @StartDate, @EndDate, @Notes, @Launch, @News, @Wiki);", con);

            string message = "Are you sure you want to edit entry \"" + originalTitleString + "\"?";
            string caption = "Editing entry \"" + originalTitleString + "\"";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, caption, buttons);

            int hoursInt = 0;
            int minsInt = 0;
            int secsInt = 0;

            string newHoursString = "00";
            string newMinutesString = "00";
            string newSecondsString = "00";

            if (hours != "")
            {
                hoursInt = Convert.ToInt32(hours);
                newHoursString = hours;
            }
            if (minutes != "")
            {
                minsInt = Convert.ToInt32(minutes);
                newMinutesString = minutes;
            }
            if (seconds != "")
            {
                secsInt = Convert.ToInt32(seconds);
                newSecondsString = seconds;
            }

            if (hoursInt < 10 && hoursInt > 0)
                newHoursString = "0" + hours;
            if (minsInt < 10 && minsInt > 0)
                newMinutesString = "0" + minutes;
            if (secsInt < 10 && secsInt > 0)
                newSecondsString = "0" + seconds;

            string playTime = newHoursString + "h:" + newMinutesString + "m:" + newSecondsString + "s";

            title.Trim();
            platform.Trim();
            status.Trim();
            rating.Trim();
            playTime.Trim();
            obtained.Trim();
            startDate.Trim();
            endDate.Trim();
            notes.Trim();
            launchCode.Trim();
            newsCode.Trim();
            wikiCode.Trim();

            if (result == DialogResult.Yes)
            {
                con.Open();

                delCmd.ExecuteNonQuery();

                cmd.Parameters.AddWithValue("@Title", title);
                if (platform == "")
                    cmd.Parameters.AddWithValue("@Platform", "");
                else
                    cmd.Parameters.AddWithValue("@Platform", platform);

                if (status == "")
                    cmd.Parameters.AddWithValue("@Status", "");
                else
                    cmd.Parameters.AddWithValue("@Status", status);

                if (rating == "")
                    cmd.Parameters.AddWithValue("@Rating", "0");
                else
                    cmd.Parameters.AddWithValue("@Rating", rating);

                if (playTime == "")
                    cmd.Parameters.AddWithValue("@PlayTime", "00h:00m:00s");
                else
                    cmd.Parameters.AddWithValue("@PlayTime", playTime);

                if (obtained == "")
                    cmd.Parameters.AddWithValue("@Obtained", "");
                else
                    cmd.Parameters.AddWithValue("@Obtained", obtained);

                if (startDate == "")
                    cmd.Parameters.AddWithValue("@StartDate", "");
                else
                    cmd.Parameters.AddWithValue("@StartDate", startDate);

                if (endDate == "")
                    cmd.Parameters.AddWithValue("@EndDate", "");
                else
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                if (notes == "")
                    cmd.Parameters.AddWithValue("@Notes", "");
                else
                    cmd.Parameters.AddWithValue("@Notes", notes);

                if (title == "Sonic World")
                    cmd.Parameters.AddWithValue("@Launch", "BATs/Sonic World.bat");
                else if (launchCode == "")
                    cmd.Parameters.AddWithValue("@Launch", "");
                else
                    cmd.Parameters.AddWithValue("@Launch", launchCode);

                if (newsCode == "")
                    cmd.Parameters.AddWithValue("@News", "");
                else
                    cmd.Parameters.AddWithValue("@News", newsCode);

                if (wikiCode == "")
                    cmd.Parameters.AddWithValue("@Wiki", "");
                else
                    cmd.Parameters.AddWithValue("@Wiki", wikiCode);

                try
                {
                    cmd.ExecuteNonQuery();
                    this.Text = "Edit an entry... Game edited.";
                    FillTable();
                }
                catch (OleDbException e)
                {
                    caption = "ERROR: Notes/Comments field too long.";
                    message = "Your notes/comments field is too long. Please reduce to 255 characters.";
                    MessageBox.Show(message, caption);
                }

            }
            else
            {
                return;
            }
            return;
        }

        private void FillTable()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Collection.accdb";
            OleDbConnection con = new OleDbConnection(connectionString);

            OleDbCommand cmd = new OleDbCommand("SELECT * FROM Table1", con);
            con.Open();
            cmd.CommandType = CommandType.Text;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            newTable = new DataTable();
            da.Fill(newTable);

            con.Close();
        }



    }
}
