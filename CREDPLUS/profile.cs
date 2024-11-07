using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;

namespace CREDPLUS
{
    public partial class profile : Form
    {
         connection connecting = new connection();
        private int idAdministrador;
         private string nomeAdministrador;

        public profile()
        {
            InitializeComponent();
            gunaLabel4.Text = Class1.uname;
           
        }

        private void profile_Load(object sender, EventArgs e)
        {

        }

        private void gunaTextBox2_Click(object sender, EventArgs e)
        {
            gunaTextBox2.ReadOnly = false;
        }

        private void gunaLabel4_Click(object sender, EventArgs e)
        {

        }

        private void gunaTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void gunaCirclePictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;

                gunaCirclePictureBox1.Image = Image.FromFile(imagePath);

                SalvarImagemNoBanco(imagePath);
            }
        }

       
      
        private void SalvarImagemNoBanco(string imagePath)
        {
            byte[] imageBytes;
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                imageBytes = new byte[fs.Length];
                fs.Read(imageBytes, 0, Convert.ToInt32(fs.Length));
            }

            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "UPDATE admin SET foto = @foto WHERE idADM = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", idAdministrador);
                command.Parameters.AddWithValue("@foto", imageBytes);

                command.ExecuteNonQuery();
            }
        }

        private int ObterIdAdministrador(string nome)
        {
            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT idADM FROM admin WHERE name = @nome";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nome", nome);

                object result = command.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }

                return -1; // Retornar um valor que indica que o ID não foi encontrado
            }
        }

        private void CarregarDadosAdministrador()
        {
            idAdministrador = ObterIdAdministrador(nomeAdministrador);

            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT name, password FROM admin WHERE idADM = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", idAdministrador);

                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    gunaTextBox1.Text = reader["name"].ToString();
                    gunaTextBox2.Text = reader["password"].ToString();
                }

                reader.Close();
            }
        }

        private void gunaButton1_Click(object sender, EventArgs e)
        {
            // Obtém os dados do novo administrador das TextBoxes
            string novoNome = gunaTextBox1.Text;
            string novaSenha = gunaTextBox2.Text;

            // Verifica se os campos obrigatórios estão preenchidos
            if (string.IsNullOrEmpty(novoNome) || string.IsNullOrEmpty(novaSenha))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Converte a imagem para bytes, se existir
            byte[] imageBytes = null;
            if (gunaCirclePictureBox1.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    gunaCirclePictureBox1.Image.Save(ms, gunaCirclePictureBox1.Image.RawFormat);
                    imageBytes = ms.ToArray();
                }
            }

            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                // Insere um novo administrador
                string insertQuery = "INSERT INTO admin (name, password, foto) VALUES (@name, @password, @foto)";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, connection);
                insertCommand.Parameters.AddWithValue("@name", novoNome);
                insertCommand.Parameters.AddWithValue("@password", novaSenha);
                insertCommand.Parameters.AddWithValue("@foto", imageBytes ?? (object)DBNull.Value); // Evita problema com valor nulo

                int rowsAffected = insertCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Novo administrador cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpa os campos após o cadastro
                    LimparCampos();

                    // Recarrega os dados do administrador recém-cadastrado
                    CarregarDadosAdministrador();
                }
                else
                {
                    MessageBox.Show("Erro ao cadastrar novo administrador.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LimparCampos()
        {
            gunaTextBox1.Text = string.Empty;
            gunaTextBox2.Text = string.Empty;
            gunaCirclePictureBox1.Image = null;
        }

        private void gunaAdvenceButton1_Click(object sender, EventArgs e)
        {
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
            this.Hide();
        }

        private void gunaTextBox3_Click(object sender, EventArgs e)
        {

        }

        private void gunaButton2_Click(object sender, EventArgs e)
        {
            // Obtém os dados do administrador das TextBoxes
            string nome = gunaTextBox1.Text;
            string novaSenha = gunaTextBox2.Text;
            string senhaAtualizacao = gunaTextBox3.Text; // Senha atual para validação

            // Verifica se os campos obrigatórios estão preenchidos
            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(senhaAtualizacao))
            {
                MessageBox.Show("Por favor, preencha todos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verifica se a senha atual fornecida está correta
            if (ValidarSenhaAtual(nome, senhaAtualizacao))
            {
                // Atualiza a senha do administrador
                AtualizarSenha(nome, novaSenha);
                MessageBox.Show("Senha atualizada com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpa os campos após a atualização
                LimparCampos();

                // Recarrega os dados do administrador recém-atualizado
                CarregarDadosAdministrador();
            }
            else
            {
                MessageBox.Show("Senha atual incorreta. Tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarSenhaAtual(string nome, string senhaAtual)
        {
            // Obtém o nome do administrador atualmente logado
            string nomeLogado = Class1.uname;

            // Verifica se a senha atual fornecida está correta
            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string query = "SELECT COUNT(*) FROM admin WHERE name = @nome AND password = @senhaAtual";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nome", nomeLogado); // Usa o nome do administrador atualmente logado
                    command.Parameters.AddWithValue("@senhaAtual", senhaAtual);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count > 0; // Se o count for maior que 0, a senha está correta
                }
            }
        }

        private void AtualizarSenha(string nome, string novaSenha)
        {
            // Obtém o nome do administrador atualmente logado
            string nomeLogado = Class1.uname;

            // Atualiza a senha do administrador
            using (MySqlConnection connection = connecting.GetConnection())
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                string updateQuery = "UPDATE admin SET password = @novaSenha WHERE name = @nome";
                using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@nome", nomeLogado); // Usa o nome do administrador atualmente logado
                    updateCommand.Parameters.AddWithValue("@novaSenha", novaSenha);

                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        private void gunaLabel5_Click(object sender, EventArgs e)
        {

        }

        private void gunaLabel10_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Admin novoFormulario = new Admin();
            novoFormulario.Show();
            this.Hide();
        }
    }
    }
    


