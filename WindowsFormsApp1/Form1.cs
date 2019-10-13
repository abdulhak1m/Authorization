using Login;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lblError.Visible = false;
            // обращаемся к методу передвижения формы
            Panel_Top();
            // кнопка закрытия формы
            btn_Close.Click += (s, e) => { Close(); };
            // забыли пароль?
            ForgotYourPassword.Click += (s, e) => { Panel(pnl2_confirm_username); };
            //  в окно авторизации с панели подтверждения имени пользователя
            linklb_back.Click += (s, e) => { Panel(pnl1_autho); };
            // в окно авторизации с панели сброса пароля
            linklbPasswordBack.Click += (s, e) => { Panel(pnl1_autho); };
            linklb_panel_autho.Click += (s, e) => { Panel(pnl1_autho); };

            linklb_back_Autho.Click += (s, e) => { Panel(pnl1_autho); };
            link_back_return.Click += (s, e) => { Panel(pnl2_confirm_username); };
            // панель сбора пароля
            #region Reset Password
            // скрывать пароль при вводе в текстовое поле
            txt_old_password.TextChanged += (s, e) => { TextBox(txt_old_password, true); Prov(); }; // так же моментально проверяем пароль на сопдение 
            txt_new_password.TextChanged += (s, e) => { TextBox(txt_new_password, true); Prov(); };// при введении его в текстовое поле
            txt_confirm_password.TextChanged += (s, e) => { TextBox(txt_confirm_password, true); Prov(); };// если не совпадает, кнопка 'готово' недоступна
            // показать пароль при нажатии на кнопку, и скрытие
            btn_reset_show0.MouseDown += (s, e) => { TextBox(txt_old_password, false); };
            btn_reset_show0.MouseUp += (s, e) => { TextBox(txt_old_password, true); };
            btn_reset_show1.MouseDown += (s, e) => { TextBox(txt_new_password, false); };
            btn_reset_show1.MouseUp += (s, e) => { TextBox(txt_new_password, true); };
            btn_reset_show2.MouseDown += (s, e) => { TextBox(txt_confirm_password, false); };
            btn_reset_show2.MouseUp += (s, e) => { TextBox(txt_confirm_password, true); };
            #endregion
            // панель авторизации
            #region PasswordBox 
            txt_password.TextChanged += (s, e) => { TextBox(txt_password, true); };
            btn_Show.MouseDown += (s, e) => { TextBox(txt_password, false); };
            btn_Show.MouseUp += (s, e) => { TextBox(txt_password, true); };
            #endregion
        }
        // метод, обращения к текстовому полю и передача булевого значения
        void TextBox(TextBox text, bool boolean)
        {
            text.UseSystemPasswordChar = boolean;
        }
        // метод передвижения формы
        void Panel_Top()
        {
            int move = 0, moveX = 0, moveY = 0;
            pnl_Top.MouseDown += (s, e) => { move = 1; moveX = e.X; moveY = e.Y; };
            pnl_Top.MouseMove += (s, e) => { if (move == 1) SetDesktopLocation(MousePosition.X - moveX, MousePosition.Y - moveY); };
            pnl_Top.MouseUp += (s, e) => { move = 0; };
        }
        // метод переключения между панелями
        void Panel(Panel panel) { panel.BringToFront(); }
        // метод проврки пароль на совпадение
        void Prov()
        {
            if (txt_new_password.Text != txt_confirm_password.Text)
            {
                lblError.Visible = true;
                lblError.Text = "Внимание, пароли не совпадают!";
                linklbDone.Enabled = false;
            }
            else
            {
                lblError.Text = "";
                lblError.Visible = false;
                linklbDone.Enabled = true;
            }
        }
        // переход в другое окно
        void LinklabelThis_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ActiveForm.Hide();
            Form2 form2 = new Form2();
            form2.ShowDialog();
            Close();
        }
        // строка подключения к Базе Данных
        static string myConnection = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        // событие клик, после нажатия происходит авторизация пользователя
        void Btn_login_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_username.Text) && !string.IsNullOrEmpty(txt_password.Text))
                {
                    using (SqlConnection connection = new SqlConnection(myConnection))
                    {
                        connection.Open();
                        // ищем в базу имя пользователя, совпадающего с веденным именем пользователя и пароля
                        SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM db_my WHERE [Имя_пользователя] = '" + txt_username.Text.Trim() + "' and [Пароль]= '" + txt_password.Text.Trim() + "'", connection);
                        DataTable data = new DataTable();
                        sda.Fill(data);
                        // в случае успешной авторизации, осуществляем переход на другую форму
                        if (data.Rows.Count == 1)
                        {
                            ActiveForm.Hide();
                            Form3 form3 = new Form3();
                            form3.ShowDialog();
                            Close();
                        }
                        else
                            MessageBox.Show("Неверное имя пользователя или пароль!", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Заполните поля!", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        // верификация пользователя, по имени пользователя
        void Linklb_Next_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    // ищем пользователя, по введенному имени пользователя
                    connection.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM db_my WHERE [Имя_пользователя] = '" + txt_reset_username.Text.Trim() + "'", connection);
                    DataTable data = new DataTable();
                    sda.Fill(data);
                    if (data.Rows.Count == 1)
                    {
                        Panel(pnl3_reset_password);
                    }
                    else
                        MessageBox.Show("Такой пользователь не найден", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        // метод редактирования пароля
        public new bool Update()
        {
            bool IsSuccess = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(myConnection))
                {
                    connection.Open();
                    string query = $"UPDATE db_my SET [Пароль] = '{txt_new_password.Text}' WHERE Id ='{int.Parse(txt_ID.Text)}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    int row = command.ExecuteNonQuery();
                    IsSuccess = row > 0 ? true : false;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return IsSuccess;
        }
        // событие сброса пароля
        private void LinklbDone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txt_old_password.Text) && !string.IsNullOrEmpty(txt_new_password.Text)
                    && !string.IsNullOrEmpty(txt_confirm_password.Text) && !string.IsNullOrEmpty(txt_ID.Text))
                {
                    // идет проверка паролей на совпадение введённых в текстовые поля
                    Prov();
                    using (SqlConnection connection = new SqlConnection(myConnection))
                    {
                        connection.Open();
                        SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM db_my WHERE [Пароль] = '" + txt_old_password.Text.Trim() + "'", connection);
                        DataTable data = new DataTable();
                        sda.Fill(data);
                        // если условие врено, то есть, старый пароль введён верно, идёт сброс пароля
                        if (data.Rows.Count == 1)
                        {
                            bool success = Update();
                            Panel(success ? pnl_done_reser_password : pnl_reset_password_error);
                        }
                        else
                            MessageBox.Show("Вы ввели неверно старый пароль!", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Заполните поля!", "Уведомление системы!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
