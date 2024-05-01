using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace CourseWorkWindowsFormsApp
{
    public partial class Form1 : Form
    {

        private DataContext _context; // Контекст бази даних Entity Framework
        private LinkedList<Employee> employees; // Список співробітників
        public Form1()
        {
            InitializeComponent();
            employees = new LinkedList<Employee>();
            _context = new DataContext(); // Ініціалізація контексту бази даних
            _context.Database.EnsureCreated(); // Перевірка і створення бази даних якщо не існує

            saveButton.Visible = false;
            LoadEmployees(); // Завантаження співробітників при завантаженні форми
        }

        // Метод для завантаження співробітників у listBox
        private void LoadEmployees()
        {
            listBox.Items.Clear();
            foreach (var employee in _context.Employees)
            {
                listBox.Items.Add($"{employee.LastName} {employee.FirstName} {employee.Patronymic}, Посада - {employee.Position}, Зарплата - {employee.CalculateTotalSalary()} грн.");
            }
        }

        // Обробник події вибору посади в ComboBox
        private void postComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (postComboBox.SelectedItem != null)
            {
                switch (postComboBox.SelectedItem.ToString())
                {
                    case "Викладач":
                    case "Доцент":
                    case "Професор":
                        salaryNumericUpDown.Enabled = true;
                        salaryEmployeeTextBox.Enabled = false;
                        timeNumericUpDown.Enabled = true;
                        break;
                    case "Методист":
                    case "Електрик":
                    case "Ректор":
                        salaryNumericUpDown.Enabled = false;
                        salaryEmployeeTextBox.Enabled = true;
                        timeNumericUpDown.Enabled = false;
                        break;
                }
            }
        }

        // Обробник події додавання нового співробітника
        private void addButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(surnameTextBox.Text) || string.IsNullOrEmpty(nameTextBox.Text))
            {
                MessageBox.Show("Будь ласка, заповніть прізвище та ім'я співробітника.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Employee newEmployee;

            // Створення нового співробітника в залежності від вибраної посади
            if (postComboBox.SelectedItem.ToString() == "Викладач" ||
                postComboBox.SelectedItem.ToString() == "Доцент" ||
                postComboBox.SelectedItem.ToString() == "Професор")
            {
                HourlyEmployee hourlyEmployee = new HourlyEmployee();
                hourlyEmployee.WorkedHours = (int)timeNumericUpDown.Value;
                hourlyEmployee.HourlyRate = (double)salaryNumericUpDown.Value;
                hourlyEmployee.Salary = hourlyEmployee.CalculateTotalSalary();
                newEmployee = hourlyEmployee;
            }
            else
            {
                if (string.IsNullOrEmpty(salaryEmployeeTextBox.Text))
                {
                    MessageBox.Show("Введіть зарплату.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SalariedEmployee salariedEmployee = new SalariedEmployee();
                salariedEmployee.Salary = double.Parse(salaryEmployeeTextBox.Text);
                newEmployee = salariedEmployee;
            }

            newEmployee.LastName = surnameTextBox.Text;
            newEmployee.FirstName = nameTextBox.Text;
            newEmployee.Patronymic = patronymicTexBox.Text;
            newEmployee.Position = postComboBox.SelectedItem.ToString();

            listBox.Items.Add($"{newEmployee.LastName} {newEmployee.FirstName} {newEmployee.Patronymic}, Посада - {newEmployee.Position}, Зарплата - {newEmployee.CalculateTotalSalary()} грн.");

            _context.Employees.Add(newEmployee);
            _context.SaveChanges();

            MessageBox.Show("Співробітник успішно доданий.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Очистка полів для введення нового співробітника
            surnameTextBox.Text = "";
            nameTextBox.Text = "";
            patronymicTexBox.Text = "";
            salaryEmployeeTextBox.Text = "";
            postComboBox.SelectedItem = null;
            salaryNumericUpDown.Value = salaryNumericUpDown.Minimum;
            salaryEmployeeTextBox.Text = "";
            timeNumericUpDown.Value = timeNumericUpDown.Minimum;
        }

        // Обробник події редагування співробітника
        private void editButton_Click(object sender, EventArgs eventArgs)
        {
            addButton.Enabled = false;
            if (listBox.SelectedIndex != -1)
            {
                var selectedEmployee = _context.Employees.Skip(listBox.SelectedIndex).FirstOrDefault();

                surnameTextBox.Text = selectedEmployee.LastName;
                nameTextBox.Text = selectedEmployee.FirstName;
                patronymicTexBox.Text = selectedEmployee.Patronymic;
                postComboBox.SelectedItem = selectedEmployee.Position;

                if (selectedEmployee is HourlyEmployee hourlyEmployee)
                {
                    timeNumericUpDown.Value = hourlyEmployee.WorkedHours;
                    salaryNumericUpDown.Value = (decimal)hourlyEmployee.HourlyRate;
                }
                else if (selectedEmployee is SalariedEmployee salariedEmployee)
                {
                    salaryEmployeeTextBox.Text = salariedEmployee.Salary.ToString();
                }

                selectedEmployee = _context.Employees.Skip(listBox.SelectedIndex).FirstOrDefault();
            }
            else
            {
                MessageBox.Show("Спочатку виберіть співробітника для редагування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            saveButton.Visible = true;
        }

        // Обробник події видалення співробітника
        private void deleteButton_Click(object sender, EventArgs eventArgs)
        {
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("Виберіть співробітника для видалення.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string selectedEmployeeString = listBox.SelectedItem.ToString();

            string employeeName = selectedEmployeeString.Split(',')[0].Trim();

            var employee = _context.Employees.FirstOrDefault(e => (e.LastName + " " + e.FirstName + " " + e.Patronymic).Trim() == employeeName);
            if (employee == null)
            {
                MessageBox.Show("Обраний співробітник не знайдений у базі даних.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            listBox.Items.Remove(selectedEmployeeString);

            MessageBox.Show("Співробітника успішно видалено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Обробник події закриття програми
        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Метод для відображення інформації про першого співробітника
        private void firstButton_Click(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = 0;

                string selectedEmployeeInfo = listBox.SelectedItem.ToString();

                MessageBox.Show(selectedEmployeeInfo, "Інформація про першого співробітника", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Список співробітників порожній.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для відображення інформації про попереднього співробітника
        private void previousButton_Click(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                if (listBox.SelectedIndex > 0)
                {
                    listBox.SelectedIndex--;

                    string selectedEmployeeInfo = listBox.SelectedItem.ToString();

                    MessageBox.Show(selectedEmployeeInfo, "Інформація про попереднього співробітника", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ви вже перебуваєте на першому співробітнику.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Список співробітників порожній.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для відображення інформації про наступного співробітника
        private void nextButton_Click(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                if (listBox.SelectedIndex < listBox.Items.Count - 1)
                {
                    listBox.SelectedIndex++;

                    string selectedEmployeeInfo = listBox.SelectedItem.ToString();

                    MessageBox.Show(selectedEmployeeInfo, "Інформація про наступного співробітника", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ви вже перебуваєте на останньому співробітнику.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Список співробітників порожній.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для відображення інформації про останнього співробітника
        private void lastButton_Click(object sender, EventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
                string selectedEmployeeInfo = listBox.SelectedItem.ToString();
                MessageBox.Show(selectedEmployeeInfo, "Інформація про останнього співробітника", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Список співробітників порожній.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події збереження змін
        private void saveButton_Click(object sender, EventArgs eventArgs)
        {
            addButton.Enabled = true;
            saveButton.Visible = false;

            if (listBox.SelectedIndex != -1)
            {
                var selectedEmployee = _context.Employees.Skip(listBox.SelectedIndex).FirstOrDefault();

                if (selectedEmployee != null)
                {
                    selectedEmployee.LastName = surnameTextBox.Text;
                    selectedEmployee.FirstName = nameTextBox.Text;
                    selectedEmployee.Patronymic = patronymicTexBox.Text;
                    selectedEmployee.Position = postComboBox.SelectedItem.ToString();

                    if (selectedEmployee is HourlyEmployee hourlyEmployee)
                    {
                        if (!int.TryParse(timeNumericUpDown.Text, out int workedHours) ||
                            !double.TryParse(salaryNumericUpDown.Text, out double hourlyRate))
                        {
                            MessageBox.Show("Некоректні значення для часового робітника.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        hourlyEmployee.WorkedHours = workedHours;
                        hourlyEmployee.HourlyRate = hourlyRate;
                    }
                    else if (selectedEmployee is SalariedEmployee salariedEmployee)
                    {
                        if (!double.TryParse(salaryEmployeeTextBox.Text, out double salary))
                        {
                            MessageBox.Show("Некоректне значення для місячного робітника.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (salary < 0)
                        {
                            MessageBox.Show("Зарплата не може бути від'ємною.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        salariedEmployee.Salary = salary;
                    }

                    _context.SaveChanges();
                    LoadEmployees();

                    // Очистка полів для введення нового співробітника
                    surnameTextBox.Text = "";
                    nameTextBox.Text = "";
                    patronymicTexBox.Text = "";
                    salaryEmployeeTextBox.Text = "";
                    postComboBox.SelectedItem = null;
                    salaryNumericUpDown.Value = salaryNumericUpDown.Minimum;
                    salaryEmployeeTextBox.Text = "";
                    timeNumericUpDown.Value = timeNumericUpDown.Minimum;

                    MessageBox.Show("Зміни збережено.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Спочатку виберіть співробітника для редагування.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool isSortedBySalaryDescending = false;
        // Обробник події вибору сортування за зарплатою
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isSortedBySalaryDescending = checkBox1.Checked;
            SortEmployeesBySalary();
        }
        // Метод сортування співробітників за зарплатою
        private void SortEmployeesBySalary()
        {
            var sortedEmployees = _context.Employees.ToList();

            if (isSortedBySalaryDescending)
            {
                sortedEmployees = sortedEmployees.OrderByDescending(emp => emp.CalculateTotalSalary()).ToList();
            }
            else
            {
                sortedEmployees = sortedEmployees.OrderBy(emp => emp.CalculateTotalSalary()).ToList();
            }

            listBox.Items.Clear();
            foreach (var employee in sortedEmployees)
            {
                listBox.Items.Add($"{employee.LastName} {employee.FirstName} {employee.Patronymic}, Посада - {employee.Position}, Зарплата - {employee.CalculateTotalSalary()} грн.");
            }
        }

        private bool isSortedAlphabetically = false;
        // Обробник події вибору алфавітного сортування
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            isSortedAlphabetically = checkBox2.Checked;
            SortEmployees();
        }
        // Метод сортування співробітників за прізвищем
        private void SortEmployees()
        {
            var sortedEmployees = _context.Employees.ToList();
            if (isSortedAlphabetically)
            {
                listBox.Sorted = true;
            }
            else
            {
                listBox.Sorted = false;
                if (checkBox1.Checked)
                {
                    SortEmployeesBySalary();
                }
                else
                {
                    listBox.Items.Clear();
                    foreach (var employee in sortedEmployees)
                    {
                        listBox.Items.Add($"{employee.LastName} {employee.FirstName} {employee.Patronymic}, Посада - {employee.Position}, Зарплата - {employee.CalculateTotalSalary()} грн.");
                    }
                }
            }
        }
    }
}