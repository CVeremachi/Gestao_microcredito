using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CREDPLUS
{
    public partial class view : Form
    {

        connection cn0 = new connection();
        public view()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            PreencherDataGridViewPagamentos();
        }

        private const int cGrip = 16;
        private const int cCaption = 32;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;

                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void PreencherDataGridViewPagamentos()
        {
            try
            {
                using (MySqlConnection conn = cn0.GetConnection())
                {
                    cn0.Open();

                    if (conn.State != ConnectionState.Open)
                    {
                        MessageBox.Show("Não foi possível abrir a conexão com o banco de dados.");
                        return;
                    }

                    string sql = "SELECT pagamentos.data AS DataPagamento, pagamentos.pay AS ValorPagamento, pagamentos.name " +
                                 "FROM pagamentos " +
                                 "LEFT JOIN user ON pagamentos.idUser = user.idUser"; // Adicione a condição de junção adequada

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Verifica se existem linhas antes de preencher o DataGridView
                            if (reader.HasRows)
                            {
                                // Configura o DataGridView para aceitar a adição de linhas programaticamente
                                dataGridView1.Rows.Clear();

                                // Itera sobre as linhas do resultado da consulta
                                while (reader.Read())
                                {
                                    // Adiciona uma nova linha ao DataGridView e preenche os dados
                                    dataGridView1.Rows.Add(
                                        reader["DataPagamento"],
                                        reader["ValorPagamento"],
                                        reader["name"]
                                    );
                                }
                            }
                            else
                            {
                                MessageBox.Show("Não foram encontrados dados para exibir.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao preencher o DataGridView de pagamentos. Detalhes: " + ex.Message);
            }
        }





        private void view_Load(object sender, EventArgs e)
        {

        }

        private void gunaLabel1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gunaLabel2_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
            this.Hide();
        }
    }
}
