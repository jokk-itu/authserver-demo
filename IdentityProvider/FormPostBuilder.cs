using System.Text;

namespace IdentityProvider;
public static class FormPostBuilder
{
  private const string _end = @"
</form>
</body>
</html>";

  public static string BuildAuthorizationCodeResponse(string redirectUri, string state, string code, string iss)
  {
    var formBuilder = new StringBuilder();
    formBuilder.Append(AddStart(redirectUri));
    formBuilder.Append(AddInput("state", state));
    formBuilder.Append(AddInput("code", code));
    formBuilder.Append(AddInput("issuer", iss));
    formBuilder.Append(_end);
    return formBuilder.ToString();
  }

  public static string BuildErrorResponse(string redirectUri, string state, string? error, string? errorDescription, string iss)
  {
    var formBuilder = new StringBuilder();
    formBuilder.Append(AddStart(redirectUri));
    formBuilder.Append(AddInput("state", state));
    if (!string.IsNullOrWhiteSpace(error))
    {
      formBuilder.Append(AddInput("error", error));
    }

    if (!string.IsNullOrWhiteSpace(errorDescription))
    {
      formBuilder.Append(AddInput("error_description", errorDescription));
    }
    formBuilder.Append(AddInput("issuer", iss));
    formBuilder.Append(_end);
    return formBuilder.ToString();
  }
  
  private static string AddStart(string redirectUri)
  {
    return $@"
<!DOCTYPE html>
<html>
<head>
<meta charset=""utf-8"" />
<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
<title>Submit</title>
<link rel=""stylesheet"" href=""/css/loader.css"" />
</head>
<body onload=""javascript:document.forms[0].submit()"">
<p>Redirecting to client</p>
<div class=""lds-ring"">
  <div></div>
  <div></div>
  <div></div>
  <div></div>
</div>
<form method=""post"" action=""{redirectUri}"">
";
  }

  private static string AddInput(string name, string value)
  {
    return $@"<input name=""{name}"" type=""hidden"" value=""{value}"" />{Environment.NewLine}";
  }
}