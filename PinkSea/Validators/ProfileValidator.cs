using PinkSea.Lexicons.Records;

namespace PinkSea.Validators;

public class ProfileValidator
{
    public bool Validate(Profile profile)
    {
        if (profile.Nickname is { Length: > 64 })
            return false;

        if (profile.Bio is { Length: > 240 })
            return false;

        if (profile.Links is not null)
        {
            if (profile.Links is { Count: > 5 })
                return false;
            
            foreach (var link in profile.Links)
            {
                if (link.Name is { Length: > 50 })
                    return false;

                if (!Uri.TryCreate(link.Link, UriKind.RelativeOrAbsolute, out _))
                    return false;
            }
        }

        return true;
    }
}