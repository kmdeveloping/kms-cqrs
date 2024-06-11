namespace CqrsFramework.Common;

public static class TypeExtension
{
  public static string GetFriendlyName(this Type type)
  {
    string friendlyName = type.Name;
    if (type.IsGenericType)
    {
      int iBackTick = friendlyName.IndexOf('`');
      if (iBackTick > 0)
        friendlyName = friendlyName.Remove(iBackTick);
      friendlyName += "<";
      Type[] typeParameters = type.GetGenericArguments();
      for (int i = 0; i < typeParameters.Length; i++)
      {
        string typeParamName = GetFriendlyName(typeParameters[i]);
        friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
      }

      friendlyName += ">";
    }

    return friendlyName;
  }
}