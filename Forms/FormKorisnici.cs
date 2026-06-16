using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Forms
{
    public class FormKorisnici : Form
    {
        private Label lblNaslov;
        private DataGridView dgvKorisnici;

        private Label lblKorisnickoIme;
        private TextBox txtKorisnickoIme;
        private Label lblLozinka;
        private TextBox txtLozinka;
        private Label lblUloga;
        private ComboBox cbUloge;
        private Label lblDodatniID;
        private TextBox txtDodatniID;

        private Button btnDodaj;
        private Button btnIzmeni;
        private Button btnObrisi;
        private Button btnOcisti;

        public FormKorisnici()
        {
            InitializeComponent();
            UcitajKorisnike();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Korisnicki Nalozi";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Upravljanje korisnickim nalozima",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(500, 30)
            };
            this.Controls.Add(lblNaslov);

            // DataGridView (levo)
            dgvKorisnici = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(540, 420),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.Black,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvKorisnici.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvKorisnici.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKorisnici.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvKorisnici.EnableHeadersVisualStyles = false;
            dgvKorisnici.SelectionChanged += DgvKorisnici_SelectionChanged;
            this.Controls.Add(dgvKorisnici);

            // Panel za unos (desno)
            Panel pnlInputs = new Panel
            {
                Location = new Point(580, 60),
                Size = new Size(330, 420),
                BackColor = Color.FromArgb(55, 65, 81),
                Padding = new Padding(15)
            };

            lblKorisnickoIme = KreirajLabel("Korisnicko ime:", 15, pnlInputs);
            txtKorisnickoIme = KreirajTextBox(35, pnlInputs);

            lblLozinka = KreirajLabel("Lozinka (ostavite prazno ako ne menjate):", 80, pnlInputs);
            txtLozinka = KreirajTextBox(100, pnlInputs);
            txtLozinka.UseSystemPasswordChar = true;

            lblUloga = KreirajLabel("Uloga korisnika:", 145, pnlInputs);
            cbUloge = new ComboBox
            {
                Location = new Point(15, 165),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cbUloge.Items.Add("Administrator");
            cbUloge.Items.Add("Vozac");
            cbUloge.Items.Add("Putnik");
            cbUloge.SelectedIndex = 0;
            cbUloge.SelectedIndexChanged += CbUloge_SelectedIndexChanged;
            pnlInputs.Controls.Add(cbUloge);

            lblDodatniID = KreirajLabel("Dodatni ID (VozacID / PutnikID):", 210, pnlInputs);
            txtDodatniID = KreirajTextBox(230, pnlInputs);

            this.Controls.Add(pnlInputs);

            // Akciona dugmad na dnu (desno)
            btnDodaj = new Button { Text = "DODAJ", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(580, 490), Size = new Size(100, 32), Cursor = Cursors.Hand };
            btnDodaj.FlatAppearance.BorderSize = 0;
            btnDodaj.Click += BtnDodaj_Click;
            this.Controls.Add(btnDodaj);

            btnIzmeni = new Button { Text = "IZMENI", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(695, 490), Size = new Size(100, 32), Enabled = false, Cursor = Cursors.Hand };
            btnIzmeni.FlatAppearance.BorderSize = 0;
            btnIzmeni.Click += BtnIzmeni_Click;
            this.Controls.Add(btnIzmeni);

            btnObrisi = new Button { Text = "OBRISI", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(239, 68, 68), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(810, 490), Size = new Size(100, 32), Enabled = false, Cursor = Cursors.Hand };
            btnObrisi.FlatAppearance.BorderSize = 0;
            btnObrisi.Click += BtnObrisi_Click;
            this.Controls.Add(btnObrisi);

            // Dugme zatvori levo
            Button btnZatvori = new Button
            {
                Text = "Zatvori",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(20, 490),
                Size = new Size(140, 32),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);

            btnOcisti = new Button
            {
                Text = "Ocisti unos",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(460, 490),
                Size = new Size(100, 32),
                Cursor = Cursors.Hand
            };
            btnOcisti.FlatAppearance.BorderSize = 0;
            btnOcisti.Click += (s, e) => ResetujFormu();
            this.Controls.Add(btnOcisti);
        }

        private Label KreirajLabel(string text, int top, Panel parent)
        {
            var lbl = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Location = new Point(15, top),
                Size = new Size(300, 20)
            };
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox KreirajTextBox(int top, Panel parent)
        {
            var txt = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(15, top),
                Size = new Size(300, 25)
            };
            parent.Controls.Add(txt);
            return txt;
        }

        private void CbUloge_SelectedIndexChanged(object sender, EventArgs e)
        {
            string uloga = cbUloge.SelectedItem.ToString();
            if (uloga == "Administrator")
            {
                txtDodatniID.Enabled = false;
                txtDodatniID.Text = "";
            }
            else
            {
                txtDodatniID.Enabled = true;
            }
        }

        private void UcitajKorisnike()
        {
            var korisnici = AppState.KorisnikRepo.GetAll();
            dgvKorisnici.DataSource = null;
            dgvKorisnici.Columns.Clear();

            var displayList = korisnici.Select(k => {
                string dodatniId = "";
                if (k is Vozac v) dodatniId = v.VozacID;
                else if (k is Putnik p) dodatniId = p.PutnikID;

                return new
                {
                    KorisnickoIme = k.KorisnickoIme,
                    Uloga = k.Uloga,
                    DodatniID = dodatniId
                };
            }).ToList();

            dgvKorisnici.DataSource = displayList;
        }

        private void DgvKorisnici_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvKorisnici.SelectedRows.Count == 0)
            {
                btnIzmeni.Enabled = false;
                btnObrisi.Enabled = false;
                return;
            }

            string username = dgvKorisnici.SelectedRows[0].Cells["KorisnickoIme"].Value.ToString();
            var korisnici = AppState.KorisnikRepo.GetAll();
            var k = korisnici.FirstOrDefault(x => x.KorisnickoIme == username);

            if (k != null)
            {
                txtKorisnickoIme.Text = k.KorisnickoIme;
                txtKorisnickoIme.ReadOnly = true;
                txtLozinka.Text = ""; // Ne prikazujemo heš
                
                for (int i = 0; i < cbUloge.Items.Count; i++)
                {
                    if (cbUloge.Items[i].ToString() == k.Uloga)
                    {
                        cbUloge.SelectedIndex = i;
                        break;
                    }
                }

                string dodatniId = "";
                if (k is Vozac v) dodatniId = v.VozacID;
                else if (k is Putnik p) dodatniId = p.PutnikID;
                txtDodatniID.Text = dodatniId;

                btnDodaj.Enabled = false;
                btnIzmeni.Enabled = true;
                // Ne dozvoljavamo brisanje sopstvenog naloga radi bezbednosti
                btnObrisi.Enabled = (k.KorisnickoIme != AppState.TrenutniKorisnik.KorisnickoIme);
            }
        }

        private void ResetujFormu()
        {
            txtKorisnickoIme.Text = "";
            txtKorisnickoIme.ReadOnly = false;
            txtLozinka.Text = "";
            cbUloge.SelectedIndex = 0;
            txtDodatniID.Text = "";
            txtDodatniID.Enabled = false;

            btnDodaj.Enabled = true;
            btnIzmeni.Enabled = false;
            btnObrisi.Enabled = false;
            dgvKorisnici.ClearSelection();
        }

        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKorisnickoIme.Text) || string.IsNullOrWhiteSpace(txtLozinka.Text))
            {
                MessageBox.Show("Korisnicko ime i lozinka su obavezni.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = txtKorisnickoIme.Text.Trim();
            var korisnici = AppState.KorisnikRepo.GetAll();
            if (korisnici.Any(x => x.KorisnickoIme.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Korisnicko ime vec postoji.", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string uloga = cbUloge.SelectedItem.ToString();
            string dodatniId = txtDodatniID.Text.Trim();

            try
            {
                Korisnik novi = null;

                if (uloga == "Administrator")
                {
                    novi = new Admin
                    {
                        KorisnickoIme = username,
                        PasswordHash = Helpers.HashHelper.Hash(txtLozinka.Text),
                        Uloga = uloga
                    };
                }
                else if (uloga == "Vozac")
                {
                    if (string.IsNullOrEmpty(dodatniId))
                    {
                        // Ako admin ne unese ID, automatski generišemo
                        var tempRepo = new VoznjaRepository();
                        dodatniId = tempRepo.GenerisiNoviVozacID();
                    }

                    // Provera da li već postoji vozač sa tim detaljima u vozaci.txt
                    var vozaci = AppState.VozacRepo.GetAll();
                    var vData = vozaci.FirstOrDefault(x => x.VozacID == dodatniId);

                    novi = new Vozac
                    {
                        KorisnickoIme = username,
                        PasswordHash = Helpers.HashHelper.Hash(txtLozinka.Text),
                        Uloga = uloga,
                        VozacID = dodatniId,
                        ImePrezime = vData?.ImePrezime ?? username,
                        BrojLicence = vData?.BrojLicence ?? "",
                        VoziloID = vData?.VoziloID ?? 0,
                        Dostupan = vData?.Dostupan ?? true,
                        ProsecnaOcena = vData?.ProsecnaOcena ?? 0,
                        BrojOcena = vData?.BrojOcena ?? 0
                    };
                }
                else if (uloga == "Putnik")
                {
                    if (string.IsNullOrEmpty(dodatniId))
                    {
                        var tempRepo = new VoznjaRepository();
                        dodatniId = tempRepo.GenerisiNoviPutnikID();
                    }

                    // Automatski kreiramo zapis u putnici.txt ako ne postoji
                    var putnici = AppState.PutnikRepo.GetAll();
                    var pData = putnici.FirstOrDefault(x => x.PutnikID == dodatniId);
                    if (pData == null)
                    {
                        var noviPutnikDetails = new Putnik
                        {
                            PutnikID = dodatniId,
                            ImePrezime = username,
                            Telefon = "",
                            Email = ""
                        };
                        putnici.Add(noviPutnikDetails);
                        AppState.PutnikRepo.SaveAll(putnici);
                        pData = noviPutnikDetails;
                    }

                    novi = new Putnik
                    {
                        KorisnickoIme = username,
                        PasswordHash = Helpers.HashHelper.Hash(txtLozinka.Text),
                        Uloga = uloga,
                        PutnikID = dodatniId,
                        ImePrezime = pData.ImePrezime,
                        Telefon = pData.Telefon,
                        Email = pData.Email
                    };
                }

                korisnici.Add(novi);
                AppState.KorisnikRepo.SaveAll(korisnici);

                MessageBox.Show("Nalog je uspesno kreiran.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetujFormu();
                UcitajKorisnike();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIzmeni_Click(object sender, EventArgs e)
        {
            if (dgvKorisnici.SelectedRows.Count == 0) return;
            string username = dgvKorisnici.SelectedRows[0].Cells["KorisnickoIme"].Value.ToString();

            var korisnici = AppState.KorisnikRepo.GetAll();
            var k = korisnici.FirstOrDefault(x => x.KorisnickoIme == username);

            if (k != null)
            {
                try
                {
                    // Lozinku menjamo samo ako je uneta
                    if (!string.IsNullOrWhiteSpace(txtLozinka.Text))
                    {
                        k.PasswordHash = Helpers.HashHelper.Hash(txtLozinka.Text);
                    }

                    // Dodatni ID se menja zavisno od uloge
                    string noviId = txtDodatniID.Text.Trim();
                    if (k is Vozac v)
                    {
                        v.VozacID = noviId;
                    }
                    else if (k is Putnik p)
                    {
                        p.PutnikID = noviId;
                    }

                    AppState.KorisnikRepo.SaveAll(korisnici);

                    MessageBox.Show("Podaci naloga su uspesno azurirani.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetujFormu();
                    UcitajKorisnike();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnObrisi_Click(object sender, EventArgs e)
        {
            if (dgvKorisnici.SelectedRows.Count == 0) return;
            string username = dgvKorisnici.SelectedRows[0].Cells["KorisnickoIme"].Value.ToString();

            var rez = MessageBox.Show($"Da li ste sigurni da zelite da obrisete nalog {username}?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    var korisnici = AppState.KorisnikRepo.GetAll();
                    var k = korisnici.FirstOrDefault(x => x.KorisnickoIme == username);
                    if (k != null)
                    {
                        korisnici.Remove(k);
                        AppState.KorisnikRepo.SaveAll(korisnici);
                        MessageBox.Show("Nalog je uspesno obrisan.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetujFormu();
                        UcitajKorisnike();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
