using WapcGuest;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DemoApp
{

  // A Simple class that describes the messages given by the waPC
  // host via one of its exposed functions.
  public class HostInfo
  {
    [JsonPropertyName("realname")]
    public string? RealName { get; set; }
    [JsonPropertyName("username")]
    public string? UserName { get; set; }
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }
  }

  public class Program
  {
    // This is needed to ensure the Mono runtime is properly initialized,
    // plus we need to register the waPC functions we want to expose.
    public static void Main()
    {
      var wapc = new Wapc();
      wapc.RegisterFunction("hello", hello);
    }

    // Call a waPC function exposed by the host under the
    // `demo.system.info address`
    // The function doesn't take any input payload.
    // The host returns a JSON encoded response containing a HostInfo object
    private static string GetUsername()
    {
      var binding = "demo";
      var wapcNamespace = "system";
      var operation = "info";
      var payloadGuest = new byte[0];

      var responseBytes = Wapc.HostCall(binding, wapcNamespace, operation, payloadGuest);
      var response = Encoding.UTF8.GetString(responseBytes);

      try
      {
        HostInfo? hostInfo =
                JsonSerializer.Deserialize<HostInfo>(responseBytes);

        return hostInfo?.UserName ?? "Not disclosed";
      }
      catch
      {
        return $"UNKNOWN - invalid JSON, got {response}";
      }
    }

    // Define a new "hello" function takes as an input a greeting message
    // encoded using UTF8.
    // Returns a UTF8 string that contains the greeting message, plus the name
    // of the user who is running the program. The username is requested to the
    // waPC host.
    private static byte[] hello(byte[] payload)
    {
      var username = GetUsername();

      var greeting = Encoding.UTF8.GetString(payload);

      string responseString = $"{greeting} {username}!";
      var responseBytes = Encoding.UTF8.GetBytes(responseString);

      return responseBytes;
    }
  }
}