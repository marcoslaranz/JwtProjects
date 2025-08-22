using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace JwtSample.Models;
 
public class JwtConfig
{
  // public required -- avoid warings of compiler
  // [Required] throws error during validation process
  // The init; keeps it unchangeble 
  
  [Required] public required string Issuer { get; init; }
  [Required] public required string Audience  { get; init; }
  [Required] public required string Key { get; init; }
  [Required] public int TokenValidityMins { get; init; }
  [Required] public int RefreshTokenValidityMins { get; init; }
}

