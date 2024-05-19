﻿using IdentityProvider.Constants;

namespace IdentityProvider;
public static class FormHelper
{
  public static IEnumerable<string> GetFilteredKeys(IFormCollection formCollection)
  {
    var valuesToIgnore = new[] { AntiForgeryConstants.AntiForgeryField };
    return formCollection.Keys.Where(x => !valuesToIgnore.Contains(x));
  }
}