using System;
using System.Drawing;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormLogin : Form
    {
        private TextBox txtKorisnickoIme;
        private TextBox txtLozinka;
        private CheckBox chkPrikaziLozinku;
        private Button btnPrijava;
        private Button btnOdustani;
        private Label lblNaslov;
        private Label lblPodnaslov;
        private Label lblKorisnickoIme;
        private Label lblLozinka;
        private Label lblPoruka;
        private Panel pnlSredina;

        public FormLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Prijava";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(31, 41, 55); // Dark Gray pozadina #1F2937

            pnlSredina = new Panel
            {
                Size = new Size(350, 400),
                Location = new Point(50, 30),
                BackColor = Color.FromArgb(31, 41, 55)
            };

            lblNaslov = new Label
            {
                Text = "🚕 TAXI PREVOZ",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.FromArgb(59, 130, 246), // Primarna plava #3B82F6
                Location = new Point(10, 10),
                Size = new Size(330, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblPodnaslov = new Label
            {
                Text = "Dobrodosli! Unesite podatke za prijavu.",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(156, 163, 175), // Siva #9CA3AF
                Location = new Point(10, 55),
                Size = new Size(330, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblKorisnickoIme = new Label
            {
                Text = "Korisnicko ime:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 100),
                Size = new Size(310, 20)
            };

            txtKorisnickoIme = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(55, 65, 81), // Tamnija siva #374151
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(20, 125),
                Size = new Size(310, 30)
            };

            lblLozinka = new Label
            {
                Text = "Lozinka:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 175),
                Size = new Size(310, 20)
            };

            txtLozinka = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true,
                Location = new Point(20, 200),
                Size = new Size(310, 30)
            };

            chkPrikaziLozinku = new CheckBox
            {
                Text = "Prikazi lozinku",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(156, 163, 175),
                Location = new Point(20, 240),
                Size = new Size(150, 20),
                Cursor = Cursors.Hand
            };
            chkPrikaziLozinku.CheckedChanged += ChkPrikaziLozinku_CheckedChanged;

            lblPoruka = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(239, 68, 68), // Crvena #EF4444
                Location = new Point(20, 270),
                Size = new Size(310, 35),
                TextAlign = ContentAlignment.MiddleCenter
            };

            btnPrijava = new Button
            {
                Text = "PRIJAVA",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 315),
                Size = new Size(310, 40),
                Cursor = Cursors.Hand
            };
            btnPrijava.FlatAppearance.BorderSize = 0;
            btnPrijava.Click += BtnPrijava_Click;

            btnOdustani = new Button
            {
                Text = "Izlaz",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99), // Siva dugmad
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 365),
                Size = new Size(310, 35),
                Cursor = Cursors.Hand
            };
            btnOdustani.FlatAppearance.BorderSize = 0;
            btnOdustani.Click += BtnOdustani_Click;

            pnlSredina.Controls.Add(lblNaslov);
            pnlSredina.Controls.Add(lblPodnaslov);
            pnlSredina.Controls.Add(lblKorisnickoIme);
            pnlSredina.Controls.Add(txtKorisnickoIme);
            pnlSredina.Controls.Add(lblLozinka);
            pnlSredina.Controls.Add(txtLozinka);
            pnlSredina.Controls.Add(chkPrikaziLozinku);
            pnlSredina.Controls.Add(lblPoruka);
            pnlSredina.Controls.Add(btnPrijava);
            pnlSredina.Controls.Add(btnOdustani);

            this.Controls.Add(pnlSredina);
        }

        private void ChkPrikaziLozinku_CheckedChanged(object sender, EventArgs e)
        {
            txtLozinka.UseSystemPasswordChar = !chkPrikaziLozinku.Checked;
        }

        private void BtnPrijava_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKorisnickoIme.Text) || string.IsNullOrWhiteSpace(txtLozinka.Text))
            {
                lblPoruka.Text = "Popunite sva polja.";
                return;
            }

            try
            {
                lblPoruka.Text = "";
                Korisnik k = AppState.AuthService.Login(txtKorisnickoIme.Text, txtLozinka.Text);
                AppState.TrenutniKorisnik = k;

                // Otvaranje glavne forme i skrivanje trenutne
                FormMain formMain = new FormMain();
                formMain.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                lblPoruka.Text = ex.Message;
            }
        }

        private void BtnOdustani_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }
}
