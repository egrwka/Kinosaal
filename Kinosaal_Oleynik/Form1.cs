using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Data.SqlClient;
using System.Net.Mail;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Data;

namespace Kinosaal_Oleynik
{
    public partial class Form1 : Form
    {
        public SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;Connect Timeout=30");
        string kinoname;
        List<string> arr_pilet = new List<string>();
        public Form1(string kinon)
        {
            kinoname = kinon;
            InitializeComponent();
        }
        Label[,] _arr = new Label[4, 4];
        Label[] read = new Label[4];
        Button osta1, kinni;
        bool ost = false;
        //readonly string[] arr_pilet;


        private void Form1_Load(object sender, EventArgs e)
        {
            string text = "";
            StreamWriter to_file;
            if (!File.Exists(kinoname + ".txt"))
            {
                to_file = new StreamWriter(kinoname + ".txt", false);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        text += i + "," + j + ",false;";
                    }
                    text += "\n";
                }
                to_file.Write(text);
                to_file.Close();
            }
            this.Size = new Size(300, 430);
            //this.BackgroundImage = Image.FromFile("Images/saal.jpg");
            for (int i = 0; i < 4; i++)
            {
                read[i] = new Label();
                read[i].Text = "Rida " + (i + 1);
                read[i].Size = new Size(50, 50);
                read[i].Location = new Point(1, i * 50);
                this.Controls.Add(read[i]);

                for (int j = 0; j < 4; j++)
                {
                    _arr[i, j] = new Label();
                    _arr[i, j].BackColor = Color.Green;
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        var commandStr = "SELECT x,y from " + kinoname;
                        SqlCommand command = new SqlCommand(commandStr, con);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (Convert.ToInt32(reader["x"]) == i && Convert.ToInt32(reader["y"]) == j)
                                {
                                    _arr[i, j].BackColor = Color.Red;
                                }
                            }
                        }

                        con.Close();
                    }
                    _arr[i, j].Text = "Koht " + (j + 1); //"Rida" + i +
                    _arr[i, j].Size = new Size(50, 50);
                    _arr[i, j].BorderStyle = BorderStyle.Fixed3D;
                    _arr[i, j].Location = new Point(j * 50 + 50, i * 50);
                    this.Controls.Add(_arr[i, j]);
                    _arr[i, j].Tag = new int[] { i, j };
                    _arr[i, j].Click += new System.EventHandler(Form1_Click);
                }
            }
            osta1 = new Button();
            osta1.Text = "Osta";
            osta1.Location = new Point(50, 200);
            this.Controls.Add(osta1);
            osta1.Click += Osta_Click;
            kinni = new Button();
            kinni.Text = "Kinni";
            kinni.Location = new Point(150, 200);
            this.Controls.Add(kinni);
            kinni.Click += Kinni_Click;
        }

        private void Kinni_Click(object sender, EventArgs e)
        {
            string text = "";
            StreamWriter to_file;
            to_file = new StreamWriter(kinoname + ".txt", false);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_arr[i, j].BackColor == Color.Yellow)
                    {
                        Osta_Click_Func();
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_arr[i, j].BackColor == Color.Red)
                    {
                        text += i + "," + j + ",true;";
                    }
                    else
                    {
                        text += i + "," + j + ",false;";
                    }
                }
                text += "\n";
            }
            to_file.Write(text);
            to_file.Close();
            this.Close();
        }
        private void Osta_Click_Func()
        {
            if (ost == true)
            {
                var vastus = MessageBox.Show("Kas oled kindel?", "Appolo küsib", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (vastus == DialogResult.Yes)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (_arr[i, j].BackColor == Color.Yellow)
                            {
                                t++;
                                _arr[i, j].BackColor = Color.Red;
                                if (con.State == ConnectionState.Closed)
                                {
                                    con.Open();
                                    var commandStr = "INSERT Into " + kinoname + "(x,y) values (" + i + "," + j + ")";
                                    using (SqlCommand command = new SqlCommand(commandStr, con))
                                        command.ExecuteNonQuery();

                                    con.Close();
                                }
                                //Сохранить каждый билет в файл
                                StreamWriter pilet = new StreamWriter("Pilet" + (t + 1).ToString() + "Rida" + i.ToString() + "koht" + j.ToString() + ".txt");
                                pilet.Write("Pilet: " + t.ToString() + " rida: " + i.ToString() + " koht: " + j.ToString());
                                pilet.Close();
                                arr_pilet.Add("Pilet" + (t + 1).ToString() + "Rida" + i.ToString() + "koht" + j.ToString() + ".txt");
                            }
                        }
                    }
                    Pilet_saada();
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (_arr[i, j].BackColor == Color.Yellow)
                            {
                                _arr[i, j].Text = "Koht" + (j + 1);
                                _arr[i, j].BackColor = Color.Green;
                                ost = false;
                            }
                        }
                    }
                }
            }
            else { MessageBox.Show("On vaja midagi valida!"); }

        }

        private void Pilet_saada()
        {
            try
            {
                string mailAd = Interaction.InputBox("Sisesta e-mail", "Kuhu saada", "phloyd666@gmail.com");
                MailMessage mail = new MailMessage();
                SmtpClient stmpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("mvc.programmeerimine@gmail.com", "3.Kuursus"),
                    EnableSsl = true
                };
                mail.To.Add(mailAd);
                mail.From = new MailAddress("mvc.programmeerimine@gmail.com");
                mail.Subject = "Pilet";
                mail.Body = "";
                foreach (var item in arr_pilet)
                {
                    Attachment data = new Attachment(item);
                    mail.Attachments.Add(data);
                }
                stmpClient.Send(mail);
            }
            catch (Exception)
            {
            }
        }



        private void Osta_Click(object sender, EventArgs e)
        {
            Osta_Click_Func();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            var label = (Label)sender; // запомнили на какую надпись нажали
            var tag = (int[])label.Tag; // определили координаты надписи

            if (_arr[tag[0], tag[1]].BackColor == Color.Yellow)
            {
                _arr[tag[0], tag[1]].Text = "Koht " + (tag[1] + 1);
                _arr[tag[0], tag[1]].BackColor = Color.Green;
                ost = true;
            }
            else if(_arr[tag[0], tag[1]].BackColor != Color.Red)
            {
                _arr[tag[0], tag[1]].Text = "Ootus " + (tag[1] + 1);
                _arr[tag[0], tag[1]].BackColor = Color.Yellow;
                ost = true;
            }
            else 
            {
                MessageBox.Show("Koht" + (tag[0] + 1) + (tag[1] + 1) + "koht on juba ostetud");
            }
            //_arr[1, 2].Text = "Это место уже куплено";
        }
    }
}

