
<p align="center">
	<img width="200" src="Meta/logo.svg"><br>
	<b>PinkSea</b><br>
	<span>poniko's house</span>
</p>

<hr>

![An image of PinkSea's frontend displaying an oekaki post.](Meta/screenshot.png)

An oekaki BBS running as an [AT Protocol AppView](https://atproto.com/guides/glossary#app-view).

# Running

## Backend (AppView)

In order to run the AppView, you need the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) and the Entity Framework tools package, which you can install through `dotnet tools -g install dotnet-ef`.

### First-time run

1. Run `docker compose up -d` inside of the root folder to start PostgreSQL (if you do not have PostgreSQL installed yet.)
2. Open the `PinkSea` folder
3. Copy the sample `appsettings.example.json` file to `appsettings.json`
	
	1. Modify the `PostgresConfig` to point to your PostgreSQL settings. If you're using the supplied docker compose, you do not need to change this.
	2. Point the `AppViewConfig` URL to where your AppView will reside, this is the URL that will be used to identify your PinkSea instance. (As an example, for the official PinkSea instance, it's `https://api.pinksea.art`)
	3. (OPTIONAL) Point the `FrontendConfig` to point to where your PinkSea frontend resides. This is used for BlueSky cross-posting with a link. Leaving it blank will disallow cross-posting. (Again, as an example, for the official PinkSea instance it's `https://pinksea.art`)

4. Perform `dotnet ef database update` to run the neccessary migrations.
5. Run `dotnet run --configuration Release` to start the PinkSea AppView.

That's it! Your server now is connected to the ATmosphere and is ready to start cooperating with other PinkSea AppViews.

### Updating

1. Navigate to the `PinkSea` folder.
2. Run `dotnet ef database update` to run the migrations.
3. Once again, run `dotnet run --configuration Release` to start the server in Release mode.

## Frontend (Client app)

In order to run the client app locally, you need a fairly modern [Node.js](https://nodejs.org/en) version. Anything above Node 20 works, but I personally recommend the latest LTS version.

### Configuration

1. Navigate to `PinkSea.Frontend`.
2. Run `npm i` to download the required packages.
3. Navigate to the `src/api/atproto` folder.
3. Point the `serviceEndpoint` value inside of `client.ts` to your AppView instance. (For example, for the official PinkSea instance, the endpoint is `https://api.pinksea.art`)

### Running a local server

1. Navigate to `PinkSea.Frontend`
2. Run `npm i` to update the packages.
3. Finally, execute `npm run dev`, to start a local development server with code reloading.

### Building

1. Navigate to `PinkSea.Frontend`
2. Run `npm i` to update the packages.
3. Finally, execute `npm run build`, to build and minify the client app.

The built app will be inside of the `dist` folder.

# Acknowledgments

This is a list of people I'd love to extend my most sincere gratitude to!!

* [GlitchyPSI](https://bsky.app/profile/glitchypsi.xyz) for drawing the "post is missing" image visible whenever we see a post that does not exist! Thank you so much!!!

# License

The PinkSea code (with the exception of PinkSea.AtProto and PinkSea.AtProto.Server) is licensed under the European Union Public License-1.2. PinkSea.AtProto is released into the public domain as specified by the Unlicense. The appropriate licenses are in each folder's LICENSE files.
