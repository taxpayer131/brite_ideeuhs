using System.Data;
using Microsoft.Extensions.Options;
using BrightIdeas.Models;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace BrightIdeas.Factory
{
    public class PostFactory
    {
        private readonly string connectionString;
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(connectionString);
            }
        }
        public PostFactory(IOptions<MySqlConfig> options)
        {
            connectionString = options.Value.ConnectionString;
        }
        

        public void Add(Post newPost){
            using(IDbConnection cnx = Connection)
            {
                string query = "INSERT INTO posts (content, user_id, created_at) VALUES (@content, @user_id, NOW());";
                cnx.Open();
                cnx.Execute(query, newPost);
            }
        }
        public List<Post> GetAllPosts(){
            using(IDbConnection cnx = Connection)
            {
                cnx.Open();
                string query = "SELECT * FROM posts JOIN users ON posts.user_id WHERE posts.user_id = users.id; SELECT * FROM likes JOIN users ON likes.user_id WHERE likes.user_id = users.id;";
                using (var multi = cnx.QueryMultiple(query, null))
                {
                    var posts = multi.Read<Post, User, Post>((post, user) => { post.user = user; return post; }).ToList();
                    var likes = multi.Read<Like, User, Like>((like, user) => { like.user = user; return like; }).ToList();
                    List<Post> Posts = posts.GroupJoin(likes, 
                            post => post.id,
                            like => like.post_id,
                            (Post, Like) =>
                            {
                                Post.likes = Like.ToList();
                                return Post;
                            }).OrderByDescending(post => post.likes.Count).ToList();
                    return Posts;
                }
                
            }
        }    

        public Post GetPostById(int post_id){
            using(IDbConnection cnx = Connection)
            {
                string query = $"SELECT * FROM posts JOIN users ON users.id = posts.user_id WHERE posts.id = {post_id}";
                cnx.Open();
                return cnx.Query<Post,User,Post>(query, (post, user) => {post.user = user; return post;}).SingleOrDefault();   
            }
        }

        public List<Post> GetPostsByUser(int user_id){
            using(IDbConnection cnx = Connection){
                string query = $"SELECT * FROM posts WHERE user_id = {user_id}";
                cnx.Open();
                return cnx.Query<Post>(query).ToList();
            }
        }

        
    }
}
