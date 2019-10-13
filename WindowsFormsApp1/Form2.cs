using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace Login
{
    public partial class Form2 : Form
    {
        // метод переключения между панелями
        void Panel(Panel panel) { panel.BringToFront(); }
        public Form2()
        {
            InitializeComponent();
            lblError.Visible = false;
            // вызываем метод передвижения формы
            Panel_Top();
            // кнопка закрытия формы
            btn_Close.Click += (s, e) => { Close(); };
            // вернуться в панель регистрации
            linklb_return.Click += (s, e) => { Panel(pnl_registration); };
            // назад в окно авторизации из панели, неудачной регистрации
            linklb_Autho1.Click += (s, e) => { Back_Autho(); };
            // назад в окно авторизации из панели успешной регистрации
            linklb_autho.Click += (s, e) => { Back_Autho(); };

            #region PasswordBox
            txt_newPassword.TextChanged += (s, e) => { txt_newPassword.UseSystemPasswordChar = true; Prov(); };
            btn_Show1.MouseDown += (s, e) => { txt_newPassword.UseSystemPasswordChar = false; };
            btn_Show1.MouseUp += (s, e) => { txt_newPassword.UseSystemPasswordChar = true; };

            txt_confirmPassword.TextChanged += (s, e) => { txt_confirmPassword.UseSystemPasswordChar = true; Prov(); };
            btn_Show2.MouseDown += (s, e) => { txt_confirmPassword.UseSystemPasswordChar = false; };
            btn_Show2.MouseUp += (s, e) => { txt_confirmPassword.UseSystemPasswordChar = true; };
            #endregion
        }
        // строка подключения
        static string myConnection = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        // метод передвижения формы
        void Panel_Top()
        {
            int move = 0, moveX = 0, moveY = 0;
            pnl_Top.MouseDown += (s, e) => { move = 1; moveX = e.X; moveY = e.Y; };
            pnl_Top.MouseMove += (s, e) => { if (move == 1) SetDesktopLocation(MousePosition.X - moveX, MousePosition.Y - moveY); };
            pnl_Top.MouseUp += (s, e) => { move = 0; };
        }
        // метод регистрации (добавление данных в Базу Данных)
        public bool Insert()
        {
            bool IsSuccess = false;
            try
            {
                string query = $"INSERT INTO db_my ([Имя], [Фамилия], [Пол], [Email], [Имя_пользователя], [Пароль]) VALUES ('{txt_name.Text}', '{txt_surname.Text}', '{cmb_gender.Text}', '{txt_email.Text}', '{txt_username.Text}', '{txt_newPassword.Text}')";
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    int row = command.ExecuteNonQuery();
                    IsSuccess = row > 0 ? true : false;
                    Reset();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return IsSuccess;
        }
        // событие клик, вызывающее метод регистрации
        void Btn_done_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_name.Text) && !string.IsNullOrEmpty(txt_surname.Text)
                && !string.IsNullOrEmpty(cmb_gender.Text) && !string.IsNullOrEmpty(txt_email.Text)
                && !string.IsNullOrEmpty(txt_username.Text) && !string.IsNullOrEmpty(txt_newPassword.Text)
                && !string.IsNullOrEmpty(txt_confirmPassword.Text))
            {
                Prov();
                bool success = Insert();
                Panel(success ? pnl_success : pnl_error);
            }
            else
                MessageBox.Show("Заполните поля!", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        // назад в окно авторизации
        void Back_Autho()
        {
            ActiveForm.Hide();
            Form1 form1 = new Form1();
            form1.ShowDialog();
            Close();
        }
        // очистка полей
        void Reset()
        {
            txt_name.Text = "";
            txt_surname.Text = "";
            cmb_gender.Text = "";
            txt_email.Text = "";
            txt_username.Text = "";
            txt_newPassword.Text = "";
            txt_confirmPassword.Text = "";
        }
        // проверка пароля на совпадение
        void Prov()
        {
            if (txt_newPassword.Text != txt_confirmPassword.Text)
            {
                lblError.Visible = true;
                lblError.Text = "Вниманиме, пароль не совпадает!";
                btn_done.Enabled = false;
            }
            else
            {
                lblError.Visible = false;
                btn_done.Enabled = true;
            }
        }
        // событие загрузки формы
        private void Form2_Load(object sender, EventArgs e) => Panel(pnl_registration);
    }
}
