using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

class Program
{
    static readonly HttpClient client = new HttpClient();
    static string? token;
    static string? refreshToken;

    static async Task Main()
    {
        while (true)
        {
            if (string.IsNullOrEmpty(token))
            {
                await Authenticate();
            }

            var success = await CallWeatherForecast();

            if (!success)
            {
                var refreshed = await RefreshToken();
                if (!refreshed)
                {
                    Console.WriteLine("❌ Failed to refresh token. Re-authenticating...");
                    token = null;
                    continue;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(40));
        }
    }

    static async Task Authenticate()
    {
        var credentials = new
        {
            Username = "James",
            Password = "MinhaCasaTemBatata"
        };

        var response = await client.PostAsync("http://localhost:5151/auth/login",
            new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(json);
            token = result?.AccessToken;
            refreshToken = result?.RefreshToken;
            Console.WriteLine("✅ Authenticated");
        }
        else
        {
            Console.WriteLine($"❌ Login failed: {response.StatusCode}");
        }
    }

    static async Task<bool> CallWeatherForecast()
	{
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	
		var response = await client.GetAsync("http://localhost:5151/weatherforecast");
	
		if (response.IsSuccessStatusCode)
		{
			var data = await response.Content.ReadAsStringAsync();
			Console.WriteLine($"🌤️ Weather: {data}");
			return true;
		}
	
		var error = await response.Content.ReadAsStringAsync();
	
		try
		{
			var errorObj = JsonSerializer.Deserialize<ServerError>(error);
			if (errorObj?.error == "TokenExpired")
			{
				Console.WriteLine("🔁 Token expired");
				return false;
			}
			else if (errorObj?.error == "InvalidToken")
			{
				Console.WriteLine("❌ Invalid token. Re-authenticating...");
				token = null; // Force re-authentication
				return true;  // Don't try to refresh, just re-auth
			}
		}
		catch
		{
			// Not JSON? log it
		}
	
		Console.WriteLine($"⚠️ Unexpected error: {response.StatusCode} - {error}");
		return true; // Don't retry except for expired token
	}
	
	class ServerError
	{
		public string? error { get; set; }
		public string? message { get; set; }
	}


    static async Task<bool> RefreshToken()
    {
        var payload = new { RefreshToken = refreshToken };

        var response = await client.PostAsync("http://localhost:5151/auth/refreshtoken",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(json);
            token = result?.AccessToken;
            refreshToken = result?.RefreshToken;
            Console.WriteLine("🔄 Token refreshed");
            return true;
        }

        Console.WriteLine($"❌ Refresh failed: {response.StatusCode}");
        return false;
    }

    class AuthResponse
    {
        public string? AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
    }
}