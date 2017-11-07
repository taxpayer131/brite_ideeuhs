using System.Data;
using Microsoft.Extensions.Options;
using BrightIdeas.Models;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace BrightIdeas.Factory
{
    public class LikeFactory
    {
        private readonly string connectionString;
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
        public LikeFactory(IOptions<MySqlConfig> options)
        {
            connectionString = options.Value.ConnectionString;
        }

        public void Add(int user_id, int post_id)
        {
            using(IDbConnection cnx = Connection){
                string query = $"INSERT INTO likes (user_id, post_id) VALUES ({user_id}, {post_id});";
                cnx.Open();
                cnx.Execute(query);
            }
        }

        public List<Like> GetLikesByUser(int user_id){
            using(IDbConnection cnx = Connection){
                string query = $"SELECT * FROM likes WHERE user_id = {user_id}";
                cnx.Open();
                return cnx.Query<Like>(query).ToList();
            }
        }
        
    }
}