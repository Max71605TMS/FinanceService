namespace UserService.Domain
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string PasswordHash { get; set; }

		public User(string name, string passwordHash)
		{
			Name = name;
			PasswordHash = passwordHash;
		}
	}
}
