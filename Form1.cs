using Npgsql;
using System;
using System.Data;
using System.Windows.Forms;

namespace FAQ
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection connection;
        private NpgsqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private int currentQuestionIndex;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = "Host=localhost;Port=5432;Database=postgres;User Id=postgres;Password=faith010304;";
            connection = new NpgsqlConnection(connectionString);
            dataAdapter = new NpgsqlDataAdapter("SELECT password FROM table_user", connection);
            dataTable = new DataTable();
            currentQuestionIndex = 0;
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            try
            {
                dataTable.Clear();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow currentQuestionRow = dataTable.Rows[currentQuestionIndex];
                    string question = currentQuestionRow["password"].ToString();
                    textBox1.Text = question;
                }
                else
                {
                    textBox1.Text = "No questions found.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                DataRow currentQuestionRow = dataTable.Rows[currentQuestionIndex];
                currentQuestionRow["password"] = textBox1.Text;
                UpdateDatabase(currentQuestionRow);

                currentQuestionIndex++; // Menambahkan indeks pertanyaan
                if (currentQuestionIndex >= dataTable.Rows.Count)
                    currentQuestionIndex = 0;

                LoadQuestion();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (dataTable.Rows.Count > 0)
            {
                DataRow currentQuestionRow = dataTable.Rows[currentQuestionIndex];
                currentQuestionRow["password"] = textBox1.Text;
            }
        }

        private void UpdateDatabase(DataRow row)
        {
            try
            {
                using (NpgsqlCommand updateCommand = new NpgsqlCommand("UPDATE table_user SET password = @password WHERE id = @id", connection))
                {
                    updateCommand.Parameters.AddWithValue("@password", row["password"]);
                    updateCommand.Parameters.AddWithValue("@id", currentQuestionIndex + 1);
                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating database: " + ex.Message);
            }
        }
    }
}
