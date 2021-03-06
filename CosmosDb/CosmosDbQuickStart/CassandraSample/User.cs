namespace CassandraSample
{
    public class User
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public string user_bcity { get; set; }

        public User(int user_id, string user_name, string user_bcity)
        {
            this.user_id = user_id;
            this.user_name = user_name;
            this.user_bcity = user_bcity;
        }

        public override string ToString()
        {
            return string.Format(" {0} | {1} | {2} ", user_id, user_name, user_bcity);
        }
    }
}
