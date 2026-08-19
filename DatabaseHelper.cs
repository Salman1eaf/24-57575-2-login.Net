using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace _24_57575_2_login
{
    public class UserRecord
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(cs))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in App.config");
            _connectionString = cs;
        }

        public bool TestConnection(out string? error)
        {
            try
            {
                using var c = new SqlConnection(_connectionString);
                c.Open();
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public bool UsernameExists(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            conn.Open();
            var res = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
            return res > 0;
        }

        public void CreateUser(string username, string passwordHash, string? email, string? fullName)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("INSERT INTO dbo.Users (Username, PasswordHash, Email, FullName) VALUES (@u, @p, @e, @f)", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", passwordHash);
            cmd.Parameters.AddWithValue("@e", (object?)email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@f", (object?)fullName ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public UserRecord? GetUserByUsername(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT UserID, Username, PasswordHash, Email, FullName, CreatedAt FROM dbo.Users WHERE Username = @u", conn);
            cmd.Parameters.AddWithValue("@u", username);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                return new UserRecord
                {
                    UserID = rdr.GetInt32(0),
                    Username = rdr.GetString(1),
                    PasswordHash = rdr.GetString(2),
                    Email = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                    FullName = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                    CreatedAt = rdr.GetDateTime(5)
                };
            }
            return null;
        }

        public DataTable GetUsersDataTable()
        {
            using var conn = new SqlConnection(_connectionString);
            using var da = new SqlDataAdapter("SELECT UserID, Username, Email, CreatedAt FROM dbo.Users", conn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static string ComputeSha256Hash(string raw)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(raw);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
