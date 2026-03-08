# MusicPlayerMcp

`MusicPlayerMcp` is a simple **Model Context Protocol (MCP) server** that exposes a stdio-based interface for controlling local music playback. It is intended to be used by MCP‑aware AI assistants (such as GitHub Copilot) or any client that can speak the protocol over standard input/output.

The tool set is intentionally small and focused: play a track (by Youtube URL or search query) and stop playback.  There is no extra configuration or user interface; the server runs in the background and interacts via MCP messages.

Nuget package repo: https://www.nuget.org/packages/MusicPlayerMcpServer/

Docker package repo: https://hub.docker.com/r/eduardorezende01/musicplayermcp

---

## Available MCP Tools

All interactions happen through MCP requests on stdin/stdout. This server exposes two stdio‑based tools that Copilot (or any MCP‑aware client) can invoke directly:

### `search_and_play_song`
This tool **MUST** be used whenever the user asks to play or listen to music. It accepts either a YouTube URL or a natural‑language song/artist query; if the input isn’t a URL the server will automatically search YouTube. The tool resolves the query, downloads the audio, and begins local playback.

- **Input:** a single string containing the song query or YouTube URL.
- **Behavior:** resolves the query, downloads audio, and starts playback. A new request replaces any existing audio.
- **Response:** confirmation that playback has started or an error message.

**Examples of valid prompts:**
- `play Let It Happen by Tame Impala`
- `please play Bohemian Rhapsody`
- `play https://youtube.com/...`

### `stop_song`
This tool **MUST** be used whenever the user asks to stop or cancel music. It halts any current playback immediately.

- **Input:** none.
- **Behavior:** stops playback.
- **Response:** confirmation that the song was stopped.

**Examples of valid prompts:**
- `Stop the music`
- `please cancel the song`
- `stop the audio`

> Only one track can play at a time; issuing `search_and_play_song` while music is already playing will replace the current track.

---

## Usage

- 1° Option - copy and paste into your favorites .mcp servers (eg. VS Code, Claude Desktop)
```
{
  "servers": {
    "MusicPlayerMcpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": ["MusicPlayerMcpServer@0.1.0-beta", "--yes"]
    }
  }
}
```

2° Option - copy and paste into your favorites .mcp servers (eg. VS Code, Claude Desktop)
```
{
  "servers": {
    "MusicPlayerMcpDocker": {
      "command": "docker",
      "args": [
       "run",
        "-i",
        "--rm",
        "eduardorezende01/musicplayermcp"
      ]
    }
  }
}
```

From an MCP‑capable client (such as Copilot), the assistant simply invokes the tool by name. Example prompts:

- _"Play 'Bohemian Rhapsody' for me."_ → triggers `search_and_play_song` with query `Bohemian Rhapsody`.
- _"Stop the music."_ → triggers `stop_song`.

---

## Notes

- The server uses youtube.com as the source for audio; it does not require the user to install or configure a browser.
- It is a **stdio tool** and has no GUI.
- For **"dnx"** commands the .NET 10 runtime must be installed: https://dotnet.microsoft.com/en-us/download/dotnet/10.0
- Designed for quick control via natural language; implementation details are intentionally hidden from users.
- The audio files are downloaded into Temp folder (eg. windows -> C:\Users\x\AppData\Local\Temp\MusicPlayerMcp)

Enjoy seamless, AI‑driven local music playback!

