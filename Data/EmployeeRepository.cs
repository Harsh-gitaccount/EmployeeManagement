using EmployeeManagement.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace EmployeeManagement.Data
{
    public class EmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Get All Employees
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetAllEmployees", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            employees.Add(new Employee
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Email = reader.GetString("Email"),
                                Picture = reader.IsDBNull(reader.GetOrdinal("Picture")) ? null : reader.GetString("Picture"),
                                Qualification = reader.GetString("Qualification")
                            });
                        }
                    }
                }
            }

            return employees;
        }

        // Get Employee By ID
        public Employee GetEmployeeById(int id)
        {
            Employee employee = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_GetEmployeeById", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ID", id);
                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            employee = new Employee
                            {
                                ID = reader.GetInt32("ID"),
                                Name = reader.GetString("Name"),
                                Email = reader.GetString("Email"),
                                Picture = reader.IsDBNull(reader.GetOrdinal("Picture")) ? null : reader.GetString("Picture"),
                                Qualification = reader.GetString("Qualification")
                            };
                        }
                    }
                }
            }

            return employee;
        }

        // Add Employee
        public int AddEmployee(Employee employee)
        {
            int newId = 0;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_AddEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_Name", employee.Name);
                    cmd.Parameters.AddWithValue("@p_Email", employee.Email);
                    cmd.Parameters.AddWithValue("@p_Picture", employee.Picture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_Qualification", employee.Qualification);
                    
                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    newId = Convert.ToInt32(result);
                }
            }

            return newId;
        }

        // Update Employee
        public void UpdateEmployee(Employee employee)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateEmployee", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ID", employee.ID);
                    cmd.Parameters.AddWithValue("@p_Name", employee.Name);
                    cmd.Parameters.AddWithValue("@p_Email", employee.Email);
                    cmd.Parameters.AddWithValue("@p_Picture", employee.Picture ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_Qualification", employee.Qualification);
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        // Delete Employee
public void DeleteEmployee(int id)
{
    using (MySqlConnection conn = new MySqlConnection(_connectionString))
    {
        using (MySqlCommand cmd = new MySqlCommand("sp_DeleteEmployee", conn))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@p_ID", id);
            
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

    }
}
