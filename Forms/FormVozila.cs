using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Repositories;

namespace TaxiPrevoz.Forms
{
    public class FormVozila : Form
    {
        private Label lblNaslov;
        private DataGridView dgvVozila;

        private Label lblRegistracija;
        private TextBox txtRegistracija;
        private Label lblMarka;
        private TextBox txtMarka;
        private Label lblModel;
        private TextBox txtModel;
        private Label lblGodiste;
        private TextBox txtGodiste;
        private Label lblBoja;
        private TextBox txtBoja;
        private CheckBox chkAktivno;

        private Button btnDodaj;
        private Button btnIzmeni;
        private Button btnOcisti;

        public FormVozila()
        {
            InitializeComponent();
            UcitajVozila();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Vozni Park";
            this.Size = new Size(950, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Upravljanje voznim parkom",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(400, 30)
            };
            this.Controls.Add(lblNaslov);

            // DataGridView (levo)
            dgvVozila = new DataGridView
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
            dgvVozila.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvVozila.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVozila.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvVozila.EnableHeadersVisualStyles = false;
            dgvVozila.SelectionChanged += DgvVozila_SelectionChanged;
            this.Controls.Add(dgvVozila);

            // Panel za unos (desno)
            Panel pnlInputs = new Panel
            {
                Location = new Point(580, 60),
                Size = new Size(330, 420),
                BackColor = Color.FromArgb(55, 65, 81),
                Padding = new Padding(15)
            };

            lblRegistracija = KreirajLabel("Registracija:", 15, pnlInputs);
            txtRegistracija = KreirajTextBox(35, pnlInputs);

            lblMarka = KreirajLabel("Marka vozila:", 80, pnlInputs);
            txtMarka = KreirajTextBox(100, pnlInputs);

            lblModel = KreirajLabel("Model vozila:", 145, pnlInputs);
            txtModel = KreirajTextBox(165, pnlInputs);

            lblGodiste = KreirajLabel("Godina proizvodnje:", 210, pnlInputs);
            txtGodiste = KreirajTextBox(230, pnlInputs);

            lblBoja = KreirajLabel("Boja vozila:", 275, pnlInputs);
            txtBoja = KreirajTextBox(295, pnlInputs);

            chkAktivno = new CheckBox
            {
                Text = "Aktivno vozilo (operativno)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 345),
                Size = new Size(300, 25),
                Checked = true
            };
            pnlInputs.Controls.Add(chkAktivno);

            this.Controls.Add(pnlInputs);

            // Akciona dugmad na dnu (desno)
            btnDodaj = new Button { Text = "DODAJ VOZILO", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(580, 490), Size = new Size(130, 32), Cursor = Cursors.Hand };
            btnDodaj.FlatAppearance.BorderSize = 0;
            btnDodaj.Click += BtnDodaj_Click;
            this.Controls.Add(btnDodaj);

            btnIzmeni = new Button { Text = "IZMENI VOZILO", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(780, 490), Size = new Size(130, 32), Enabled = false, Cursor = Cursors.Hand };
            btnIzmeni.FlatAppearance.BorderSize = 0;
            btnIzmeni.Click += BtnIzmeni_Click;
            this.Controls.Add(btnIzmeni);

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

        private void UcitajVozila()
        {
            var vozila = AppState.VoziloRepo.GetAll();
            dgvVozila.DataSource = null;
            dgvVozila.Columns.Clear();

            var displayList = vozila.Select(v => new
            {
                ID = v.VoziloID,
                Registracija = v.Registracija,
                Marka = v.Marka,
                Model = v.Model,
                Godiste = v.Godiste,
                Boja = v.Boja,
                Aktivno = v.Aktivno ? "Da" : "Ne"
            }).ToList();

            dgvVozila.DataSource = displayList;

            if (this.IsHandleCreated)
            {
                PodesiSirineKolona();
            }
        }

        private void PodesiSirineKolona()
        {
            if (dgvVozila.Columns.Count > 0)
            {
                if (dgvVozila.Columns["ID"] != null) dgvVozila.Columns["ID"].Width = 50;
                if (dgvVozila.Columns["Godiste"] != null) dgvVozila.Columns["Godiste"].Width = 65;
                if (dgvVozila.Columns["Aktivno"] != null) dgvVozila.Columns["Aktivno"].Width = 70;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PodesiSirineKolona();
        }

        private void DgvVozila_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVozila.SelectedRows.Count == 0)
            {
                btnIzmeni.Enabled = false;
                return;
            }

            int id = int.Parse(dgvVozila.SelectedRows[0].Cells["ID"].Value.ToString());
            var vozila = AppState.VoziloRepo.GetAll();
            var vozilo = vozila.FirstOrDefault(v => v.VoziloID == id);

            if (vozilo != null)
            {
                txtRegistracija.Text = vozilo.Registracija;
                txtMarka.Text = vozilo.Marka;
                txtModel.Text = vozilo.Model;
                txtGodiste.Text = vozilo.Godiste.ToString();
                txtBoja.Text = vozilo.Boja;
                chkAktivno.Checked = vozilo.Aktivno;

                btnDodaj.Enabled = false;
                btnIzmeni.Enabled = true;
            }
        }

        private void ResetujFormu()
        {
            txtRegistracija.Text = "";
            txtMarka.Text = "";
            txtModel.Text = "";
            txtGodiste.Text = "";
            txtBoja.Text = "";
            chkAktivno.Checked = true;

            btnDodaj.Enabled = true;
            btnIzmeni.Enabled = false;
            dgvVozila.ClearSelection();
        }

        private void BtnDodaj_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRegistracija.Text) || 
                string.IsNullOrWhiteSpace(txtMarka.Text) || 
                string.IsNullOrWhiteSpace(txtModel.Text) || 
                string.IsNullOrWhiteSpace(txtGodiste.Text) || 
                string.IsNullOrWhiteSpace(txtBoja.Text))
            {
                MessageBox.Show("Sva polja su obavezna.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int god = int.Parse(txtGodiste.Text);
                if (god < 1900 || god > DateTime.Now.Year + 1)
                    throw new Exception("Godiste vozila nije ispravno.");

                // Generisanje ID-a za vozilo
                var voznjaRepo = new VoznjaRepository();
                int noviId = voznjaRepo.GenerisiNoviVoziloID();

                var novoVozilo = new Vozilo
                {
                    VoziloID = noviId,
                    Registracija = txtRegistracija.Text.Trim(),
                    Marka = txtMarka.Text.Trim(),
                    Model = txtModel.Text.Trim(),
                    Godiste = god,
                    Boja = txtBoja.Text.Trim(),
                    Aktivno = chkAktivno.Checked
                };

                var vozila = AppState.VoziloRepo.GetAll();
                vozila.Add(novoVozilo);
                AppState.VoziloRepo.SaveAll(vozila);

                MessageBox.Show("Novo vozilo je uspesno dodano u vozni park.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetujFormu();
                UcitajVozila();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIzmeni_Click(object sender, EventArgs e)
        {
            if (dgvVozila.SelectedRows.Count == 0) return;
            int id = int.Parse(dgvVozila.SelectedRows[0].Cells["ID"].Value.ToString());

            if (string.IsNullOrWhiteSpace(txtRegistracija.Text) || 
                string.IsNullOrWhiteSpace(txtMarka.Text) || 
                string.IsNullOrWhiteSpace(txtModel.Text) || 
                string.IsNullOrWhiteSpace(txtGodiste.Text) || 
                string.IsNullOrWhiteSpace(txtBoja.Text))
            {
                MessageBox.Show("Sva polja moraju biti popunjena.", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int god = int.Parse(txtGodiste.Text);
                if (god < 1900 || god > DateTime.Now.Year + 1)
                    throw new Exception("Godiste vozila nije ispravno.");

                var vozila = AppState.VoziloRepo.GetAll();
                var vozilo = vozila.FirstOrDefault(v => v.VoziloID == id);

                if (vozilo != null)
                {
                    vozilo.Registracija = txtRegistracija.Text.Trim();
                    vozilo.Marka = txtMarka.Text.Trim();
                    vozilo.Model = txtModel.Text.Trim();
                    vozilo.Godiste = god;
                    vozilo.Boja = txtBoja.Text.Trim();
                    vozilo.Aktivno = chkAktivno.Checked;

                    AppState.VoziloRepo.SaveAll(vozila);
                    MessageBox.Show("Podaci o vozilu su uspesno azurirani.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetujFormu();
                    UcitajVozila();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska: " + ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
