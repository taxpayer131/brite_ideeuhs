using System.Data;
using Microsoft.Extensions.Options;
using BrightIdeas.Models;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace BrightIdeas.Factory
{
    public class UserFactory
    {
        private readonly string connectionString;
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
        public UserFactory(IOptions<MySqlConfig> options)
        {
            connectionString = options.Value.ConnectionString;
        }

        public void Add(User newUser)
        {
            using(IDbConnection cnx = Connection){
                string query = "INSERT INTO users (name, alias, email, password, created_at, updated_at) VALUES (@name, @alias, @email, @password, NOW(), NOW());";
                cnx.Open();
                cnx.Execute(query, newUser);
            }
        }

        public User GetLatestUser()
        {
            using(IDbConnection cnx = Connection){
                string query = "SELECT * FROM users ORDER BY id DESC LIMIT 1";
                cnx.Open();
                return cnx.QuerySingleOrDefault<User>(query);
            }
        }

        public List<User> GetAllUsers(){
            using(IDbConnection cnx = Connection){
                string query = "SELECT * FROM Users;";
                cnx.Open();
                return cnx.Query<User>(query).ToList();
            }
        }
        
        public User GetUserById(int user_id)
        {
            using(IDbConnection cnx = Connection){
                string query = $"SELECT * FROM Users WHERE id = {user_id};";
                cnx.Open();
                return cnx.QuerySingleOrDefault<User>(query);
            }
        }
        public User GetUserByEmail(string email)
        {
            using(IDbConnection cnx = Connection)
            {
                string query = $"SELECT * FROM users WHERE email = '{email}'";
                cnx.Open();
                return cnx.QuerySingleOrDefault<User>(query);
            }
        }
        public List<User> GetUserByPost(int post_id)
        {
            using(IDbConnection cnx = Connection){
                string query = $"SELECT DISTINCT id, name, alias FROM users JOIN likes ON users.id = likes.user_id WHERE post_id = {post_id}";
                cnx.Open();
                return cnx.Query<User>(query).ToList();
            }
        }
        public void Delete(int post_id){
            using(IDbConnection cnx = Connection){
                string query = $"DELETE FROM likes WHERE post_id = {post_id}; DELETE FROM posts WHERE id = {post_id};";
                cnx.Open();
                cnx.Execute(query);
            }
        }
        
    }
}
