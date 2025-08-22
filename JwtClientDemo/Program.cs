using System.Net.Http;
using System.Text;
using System.Text.Json;

string baseUrl = "http://localhost:5151";
string accessToken = "";
string refreshToken = "";

// Use using statement for proper disposal
using HttpClient client = new HttpClient();

// Login and get tokens
async Task<bool> LoginAsync() 
{
    try 
    {
        var payload = new {
          Username = "James",
          Password = "MinhaCasaTemBatata"
        };
        
        var response = await client.PostAsync($"{baseUrl}/auth/login",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        
        if (!response.IsSuccessStatusCode) 
        {
            Console.WriteLine($"Login failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return false;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Login response: {json}"); // Debug: see actual response
        
        // Use JsonSerializerOptions for case-insensitive matching
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<LoginResponse>(json, options);
        
        if (data == null || string.IsNullOrEmpty(data.AccessToken) || string.IsNullOrEmpty(data.RefreshToken))
        { 
            Console.WriteLine("Error: Invalid response format from login endpoint!");
            return false;
        }
        
        accessToken = data.AccessToken;
        refreshToken = data.RefreshToken;
        Console.WriteLine("Login successful!");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Login error: {ex.Message}");
        return false;
    }
}

// Refresh token logic
async Task<bool> RefreshTokenAsync() 
{
    try 
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            Console.WriteLine("No refresh token available");
            return false;
        }
        
        var payload = new { token = refreshToken };
        
        var response = await client.PostAsync($"{baseUrl}/auth/refresh",
            new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        
        if (!response.IsSuccessStatusCode) 
        {
            //Console.WriteLine($"Refresh failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            // With this to see more details:
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Refresh failed: {response.StatusCode}");
            Console.WriteLine($"Error details: {errorContent}");
            Console.WriteLine($"Using refresh token: {refreshToken.Substring(0, 20)}..."); // First 20 chars

            return false;
        }
        
        var json = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Refresh response: {json}"); // Debug: see actual response
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var data = JsonSerializer.Deserialize<RefreshResponse>(json, options);
        
        if (data == null || string.IsNullOrEmpty(data.AccessToken))
        {
            Console.WriteLine("Error: Invalid response format from refresh endpoint!");
            return false;
        } 
        
        accessToken = data.AccessToken;
        Console.WriteLine("Token refreshed successfully!");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Refresh error: {ex.Message}");
        return false;
    }
}

// Call API with token - improved with retry limit
async Task<bool> CallProtectedEndpointAsync(int retryCount = 0) 
{
    const int maxRetries = 1; // Prevent infinite recursion
    
    try 
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Console.WriteLine("No access token available");
            return false;
        }
        
        // Set authorization header
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        var response = await client.GetAsync($"{baseUrl}/weatherforecast");
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && retryCount < maxRetries) 
        {
            Console.WriteLine("Token expired. Attempting refresh...");
            if (await RefreshTokenAsync()) 
            {
                return await CallProtectedEndpointAsync(retryCount + 1); // Retry with increment
            } 
            else 
            {
                Console.WriteLine("Refresh failed. Please login again.");
                return false;
            }
        } 
        else if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response: " + content);
            return true;
        }
        else 
        {
            Console.WriteLine($"API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"API call error: {ex.Message}");
        return false;
    }
}

// Main execution with continuous loop
try 
{
    Console.WriteLine("Starting JWT authentication demo...");
    
    if (!await LoginAsync())
    {
        Console.WriteLine("Failed to login. Exiting.");
        return;
    }

    Console.WriteLine("\n=== JWT Token Demo - Press 'q' to quit ===");
    Console.WriteLine("Commands:");
    Console.WriteLine("  ENTER - Call protected endpoint");
    Console.WriteLine("  'r' - Manually refresh token");
    Console.WriteLine("  'q' - Quit");
    Console.WriteLine("=========================================\n");

    while (true)
    {
        Console.Write("Press ENTER to call API, 'r' to refresh, 'q' to quit: ");
        var input = Console.ReadLine()?.ToLower().Trim();
        
        if (input == "q")
        {
            Console.WriteLine("Exiting...");
            break;
        }
        else if (input == "r")
        {
            Console.WriteLine("\n--- Manual Token Refresh ---");
            await RefreshTokenAsync();
            Console.WriteLine("--- End Manual Refresh ---\n");
        }
        else
        {
            Console.WriteLine("\n--- Calling Protected Endpoint ---");
            await CallProtectedEndpointAsync();
            Console.WriteLine("--- End API Call ---\n");
        }
        
        // Add a small delay to make output clearer
        await Task.Delay(500);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}

// DTOs for proper deserialization
public class LoginResponse
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public int ExpiresIn { get; set; } // This is typically a number (seconds)
}

public class RefreshResponse
{
    public string AccessToken { get; set; } = "";
    public int ExpiresIn { get; set; } // This is typically a number (seconds)
}