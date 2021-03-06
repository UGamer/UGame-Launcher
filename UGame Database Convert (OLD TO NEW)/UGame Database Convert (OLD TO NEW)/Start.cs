﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UGame_Database_Convert__OLD_TO_NEW_
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }
        
        public string oldPath;
        public string newPath;

        public string oldConString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=";
        string newConString = "Data Source=";

        public SQLiteConnection newCon;
        public SQLiteCommand insertCmd;

        public DataTable newTable = new DataTable();
        public DataTable gameTable = new DataTable();

        Review review;
        int highestId;

        private void OldFileButton_Click(object sender, EventArgs e)
        {
            FileDialog.FileName = "Collection.accdb";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                OldFileBox.Text = FileDialog.FileName;
            }
        }

        private void NewFileButton_Click(object sender, EventArgs e)
        {
            FileDialog.FileName = "UGameDB.mdf";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                NewFileBox.Text = FileDialog.FileName;
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            oldPath = OldFileBox.Text;
            newPath = NewFileBox.Text;

            oldConString += oldPath;
            newConString += newPath + ";Version=3;";

            newCon = new SQLiteConnection(newConString);


            SQLiteCommand fillNewTable = new SQLiteCommand("SELECT * FROM Games", newCon);

            

            

            if (!QuickConvertCheck.Checked)
            {
                insertCmd = new SQLiteCommand("INSERT INTO Games ([Id], [Title], [Platform], [Status], [Rating], [TimePlayed], [Seconds], [Obtained], [StartDate], [LastPlayed], [Notes], [URLs], [Filters], [Developers], [Publishers], [ReleaseDate], [Genre], [PlayerCount], [Price], [GameDesc], [Launch], [Blur], [Overlay], [Discord]) VALUES (@Id, @Title, @Platform, @Status, @Rating, @TimePlayed, @Seconds, @Obtained, @StartDate, @LastPlayed, @Notes, @URLs, @Filters, @Developers, @Publishers, @ReleaseDate, @Genre, @PlayerCount, @Price, @GameDesc, @Launch, @Blur, @Overlay, @Discord);", newCon);

                Review review = new Review(this, newTable, newCon);
                int index = 0;
                review.NextEntry();
                review.Show();
            }
            else
            {
                insertCmd = new SQLiteCommand("INSERT INTO Games ([Id], [Title], [Platform], [Status], [Rating], [TimePlayed], [Seconds], [Obtained], [StartDate], [LastPlayed], [Notes], [URLs], [Launch], [Blur], [Overlay]) VALUES (@Id, @Title, @Platform, @Status, @Rating, @TimePlayed, @Seconds, @Obtained, @StartDate, @LastPlayed, @Notes, @URLs, @Launch, @Blur, @Overlay);", newCon);

                
            }
        }

        
    }
}
