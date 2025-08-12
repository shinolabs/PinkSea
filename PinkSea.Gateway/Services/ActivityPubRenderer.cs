using Microsoft.Extensions.Options;
using PinkSea.Gateway.ActivityStreams;
using PinkSea.Gateway.Models;

namespace PinkSea.Gateway.Services;

public class ActivityPubRenderer(
    PinkSeaQuery query,
    IOptions<GatewaySettings> options)
{
    public async Task<Note?> RenderNoteForOekaki(string did, string rkey)
    {
        var oekakiResponse = await query.GetOekaki(did, rkey);
        if (oekakiResponse is null)
            return null;

        var maybeParent = await query.GetPossibleParentForOekaki(did, rkey);
        var parentLink = maybeParent is not null
            ? $"{options.Value.FrontEndEndpoint}/ap/note.json?did={maybeParent.AuthorDid}&rkey={maybeParent.RecordKey}"
            : null;
        
        return new Note
        {
            Id = $"{options.Value.FrontEndEndpoint}/ap/note.json?did={did}&rkey={rkey}",
            PublishedAt = oekakiResponse.Parent.CreationTime,
            Content = oekakiResponse.Parent.Alt ?? "",
            Attachments = [
                new Document
                {
                    MediaType = "image/png",
                    Href = oekakiResponse!.Parent.ImageLink,
                    Name = oekakiResponse.Parent.Alt
                }
            ],
            Sensitive = oekakiResponse.Parent.Nsfw,
            AttributedTo = $"{options.Value.FrontEndEndpoint}/ap/actor.json?did={did}",
            InReplyTo = parentLink,
            Url = $"{options.Value.FrontEndEndpoint}/{did}/oekaki/{rkey}"
        };
    }

    public async Task<Actor?> RenderActorForProfile(string did)
    {
        var profileResponse = await query.GetProfile(did);
        if (profileResponse is null)
            return null;

        return new Actor
        {
            Id = $"{options.Value.FrontEndEndpoint}/ap/actor.json?did={did}",
            Name = profileResponse.Nickname,
            PreferredUsername = profileResponse.Handle,
            Icon = new Image
            {
                Url = profileResponse.Avatar ?? $"{options.Value.FrontEndEndpoint}/assets/img/blank_avatar.png"
            },
            Bio = profileResponse.Description ?? "This user has no description.",
            Url = $"{options.Value.FrontEndEndpoint}/{did}"
        };
    }
}